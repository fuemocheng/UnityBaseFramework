using System;
using System.Collections;
using System.Collections.Generic;
using Lockstep.Logging;
using Lockstep.Math;
using Lockstep.Game;
using Lockstep.Serialization;
using Lockstep.Util;
using UnityBaseFramework.Runtime;
using BaseFramework.DataTable;
using BaseFramework;

namespace XGame
{
    public partial class GameStateService : IService, ITimeMachine
    {
        public int CurTick { get; set; }

        private GameState m_CurGameState;
        private Dictionary<int, GameState> m_Tick2State = new Dictionary<int, GameState>();
        private Dictionary<int, Serializer> m_Tick2Backup = new Dictionary<int, Serializer>();

        //EntityId <--> Entity
        private Dictionary<int, BaseEntity> m_Entities = new Dictionary<int, BaseEntity>();
        //EntityType <--> EntityList
        private Dictionary<Type, IList> m_TypeEntities = new Dictionary<Type, IList>();

        private IdService m_IdService => GameEntry.Service.GetService<IdService>();

        public T CreateEntity<T>(int configId, LVector3 position) where T : BaseEntity, new()
        {
            T baseEntity = new T();

            //TODO:Config 根据ConfigId读表设置属性；

            baseEntity.EntityId = m_IdService.GenId();
            baseEntity.ConfigId = configId;
            baseEntity.transform.Pos3 = position;
            Log.Info($"CreateEntity:{configId} Pos:{position} EntityId:{baseEntity.EntityId}");

            baseEntity.DoBindRef();

            baseEntity.Awake();
            baseEntity.Start();

            GameEntry.Service.GetService<GameViewService>().BindView(
                baseEntity,
                (bEntity) =>
                {
                    if (bEntity is CEntity cEntity)
                    {
                        PhysicSystem.Instance.RegisterEntity(configId, cEntity);
                    }
                }
            );

            AddEntity(baseEntity);
            return baseEntity;
        }

        private void AddEntity<T>(T entity) where T : BaseEntity
        {
            Type t = entity.GetType();
            if (m_TypeEntities.TryGetValue(t, out IList outList))
            {
                List<T> entityList = outList as List<T>;
                entityList.Add(entity);
            }
            else
            {
                List<T> entityList = new List<T>();
                m_TypeEntities.Add(t, entityList);
                entityList.Add(entity);
            }

            m_Entities[entity.EntityId] = entity;
        }

        public void DestroyEntity(BaseEntity entity)
        {
            entity.Destroy();
            GameEntry.Service.GetService<GameViewService>().UnbindView(entity);
            RemoveEntity(entity);
        }

        private void RemoveEntity<T>(T entity) where T : BaseEntity
        {
            Type t = entity.GetType();
            if (m_TypeEntities.TryGetValue(t, out IList outList))
            {
                outList.Remove(entity);
                m_Entities.Remove(entity.EntityId);
            }
            else
            {
                Debug.LogError("Try remove a deleted Entity:{0}, EntityId:{1}", entity, entity.EntityId);
            }
        }

        public BaseEntity GetEntity(int id)
        {
            if (m_Entities.TryGetValue(id, out BaseEntity entity))
            {
                return entity;
            }

            return null;
        }

        private List<T> GetEntities<T>()
        {
            Type t = typeof(T);
            if (m_TypeEntities.TryGetValue(t, out IList outList))
            {
                return outList as List<T>;
            }
            else
            {
                List<T> entityList = new List<T>();
                m_TypeEntities.Add(t, entityList);
                return entityList;
            }
        }

        public Player[] GetPlayers()
        {
            return GetEntities<Player>().ToArray();
        }

        public Enemy[] GetEnemies()
        {
            return GetEntities<Enemy>().ToArray();
        }

        public Spawner[] GetSpawners()
        {
            return GetEntities<Spawner>().ToArray(); //TODO Cache
        }

        public Bullet[] GetBullets()
        {
            return GetEntities<Bullet>().ToArray();
        }

        public void Backup(int tick)
        {
            m_Tick2State[tick] = m_CurGameState;

            Serializer writer = new Serializer();
            writer.Write(GameEntry.Service.GetService<CommonStateService>().Hash);

            BackUpEntities(GetPlayers(), writer);
            BackUpEntities(GetBullets(), writer);

            m_Tick2Backup[tick] = writer;
        }

        private void BackUpEntities<T>(T[] entityList, Serializer writer) where T : BaseEntity, IBackup, new()
        {
            writer.Write(entityList.Length);
            foreach (IBackup item in entityList)
            {
                item.WriteBackup(writer);
            }
        }

        public void RollbackTo(int tick)
        {
            m_CurGameState = m_Tick2State[tick];
            if (m_Tick2Backup.TryGetValue(tick, out var backupData))
            {
                var reader = new Deserializer(backupData.Data);

                var hash = reader.ReadInt32();
                GameEntry.Service.GetService<CommonStateService>().Hash = hash;

                RecoverEntities<Player>(reader);
                RecoverEntities<Bullet>(reader);
            }
            else
            {
                Log.Error($"Miss backup data cannot rollback! {tick}");
            }
        }

        private void RecoverEntities<T>(Deserializer reader) where T : BaseEntity, IBackup, new()
        {
            int count = reader.ReadInt32();

            List<int> recoveredEntities = new List<int>();
            for (int i = 0; i < count; i++)
            {
                int entityId = reader.ReadInt32WithNoMovingPosition();
                if (m_Entities.TryGetValue(entityId, out BaseEntity outEntity))
                {
                    //已存在的Entity, 不用重建，将数据读取到已存在Entity对象中。
                    ((IBackup)outEntity)?.ReadBackup(reader);
                }
                else
                {
                    //此Entity已经被删除销毁，Recover这一帧的时候重新创建。
                    T entity = new T();
                    entity.ReadBackup(reader);

                    //走一遍创建流程。
                    entity.DoBindRef();
                    entity.Awake();
                    entity.Start();

                    GameEntry.Service.GetService<GameViewService>().BindView(
                        entity,
                        (bEntity) =>
                        {
                            if (bEntity is CEntity cEntity)
                            {
                                PhysicSystem.Instance.RegisterEntity(entity.ConfigId, cEntity);
                            }
                        }
                    );

                    AddEntity(entity);
                }

                recoveredEntities.Add(entityId);
            }

            //如果 entity 不在 recoveredEntities 中，但在 m_Entities 里，说明是新建的，Recover这一帧的时候恢复为不存在，即删除掉。
            List<BaseEntity> needRemove = new List<BaseEntity>();
            foreach (KeyValuePair<int, BaseEntity> kvp in m_Entities)
            {
                if (!recoveredEntities.Contains(kvp.Key))
                {
                    needRemove.Add(kvp.Value);
                }
            }
            for (int i = 0; i < needRemove.Count; i++)
            {
                DestroyEntity(needRemove[i]);
            }
        }

        public void Clean(int maxVerifiedTick)
        {
            //TODO:防止内存占用过多，删除已经校验的帧数据。

        }

        public struct GameState
        {
            public LFloat RemainTime;
            public LFloat DeltaTime;
            public int MaxEnemyCount;
            public int CurEnemyCount;
            public int CurEnemyId;

            public int GetHash(ref int idx)
            {
                int hash = 1;
                hash += CurEnemyCount.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
                hash += MaxEnemyCount.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
                hash += CurEnemyId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
                return hash;
            }
        }

        public LFloat RemainTime
        {
            get => m_CurGameState.RemainTime;
            set => m_CurGameState.RemainTime = value;
        }

        public LFloat DeltaTime
        {
            get => m_CurGameState.DeltaTime;
            set => m_CurGameState.DeltaTime = value;
        }

        public int MaxEnemyCount
        {
            get => m_CurGameState.MaxEnemyCount;
            set => m_CurGameState.MaxEnemyCount = value;
        }

        public int CurEnemyCount
        {
            get => m_CurGameState.CurEnemyCount;
            set => m_CurGameState.CurEnemyCount = value;
        }

        public int CurEnemyId
        {
            get => m_CurGameState.CurEnemyId;
            set => m_CurGameState.CurEnemyId = value;
        }
    }
}