using System;
using System.Collections;
using System.Collections.Generic;
using Lockstep.Logging;
using Lockstep.Math;
using Lockstep.Game;
using Lockstep.Serialization;
using Lockstep.Util;
using UnityBaseFramework.Runtime;

namespace XGame
{
    public partial class GameStateService : IService, ITimeMachine
    {
        public int CurTick { get; set; }

        private Dictionary<int, GameState> _tick2State = new Dictionary<int, GameState>();
        private GameState _curGameState;
        private Dictionary<Type, IList> _type2Entities = new Dictionary<Type, IList>();
        private Dictionary<int, BaseEntity> _id2Entities = new Dictionary<int, BaseEntity>();
        private Dictionary<int, Serializer> _tick2Backup = new Dictionary<int, Serializer>();

        private IdService m_IdService => GameEntry.Service.GetService<IdService>();

        private void AddEntity<T>(T e) where T : BaseEntity
        {
            if (typeof(T) == typeof(Player))
            {
                int i = 0;
                Debug.Log("Add Player");
            }

            var t = e.GetType();
            if (_type2Entities.TryGetValue(t, out var lstObj))
            {
                var lst = lstObj as List<T>;
                lst.Add(e);
            }
            else
            {
                var lst = new List<T>();
                _type2Entities.Add(t, lst);
                lst.Add(e);
            }

            _id2Entities[e.EntityId] = e;
        }

        private void RemoveEntity<T>(T e) where T : BaseEntity
        {
            var t = e.GetType();
            if (_type2Entities.TryGetValue(t, out var lstObj))
            {
                lstObj.Remove(e);
                _id2Entities.Remove(e.EntityId);
            }
            else
            {
                Debug.LogError("Try remove a deleted Entity" + e);
            }
        }

        private List<T> GetEntities<T>()
        {
            var t = typeof(T);
            if (_type2Entities.TryGetValue(t, out var lstObj))
            {
                return lstObj as List<T>;
            }
            else
            {
                var lst = new List<T>();
                _type2Entities.Add(t, lst);
                return lst;
            }
        }

        public Enemy[] GetEnemies()
        {
            return GetEntities<Enemy>().ToArray();
        }

        public Player[] GetPlayers()
        {
            return GetEntities<Player>().ToArray();
        }

        public Spawner[] GetSpawners()
        {
            return GetEntities<Spawner>().ToArray(); //TODO Cache
        }

        public object GetEntity(int id)
        {
            if (_id2Entities.TryGetValue(id, out var val))
            {
                return val;
            }

            return null;
        }

        public T CreateEntity<T>(int prefabId, LVector3 position) where T : BaseEntity, new()
        {
            T baseEntity = new T();

            //TODO:Config
            //_gameConfigService.GetEntityConfig(prefabId)?.CopyTo(baseEntity);

            baseEntity.EntityId = m_IdService.GenId();
            baseEntity.PrefabId = prefabId;
            baseEntity.transform.Pos3 = position;
            Log.Info($"CreateEntity {prefabId} pos {prefabId} entityId:{baseEntity.EntityId}");

            baseEntity.DoBindRef();
            if (baseEntity is CEntity cEntity)
            {
                PhysicSystem.Instance.RegisterEntity(prefabId, cEntity);
            }

            baseEntity.Awake();
            baseEntity.Start();
            GameEntry.Service.GetService<GameViewService>().BindView(baseEntity);
            AddEntity(baseEntity);
            return baseEntity;
        }

        public void DestroyEntity(BaseEntity entity)
        {
            RemoveEntity(entity);
        }


        public void Backup(int tick)
        {
            _tick2State[tick] = _curGameState;
            Serializer writer = new Serializer();
            writer.Write(GameEntry.Service.GetService<CommonStateService>().Hash); //hash
            BackUpEntities(GetPlayers(), writer);
            _tick2Backup[tick] = writer;
        }

        public void RollbackTo(int tick)
        {
            _curGameState = _tick2State[tick];
            if (_tick2Backup.TryGetValue(tick, out var backupData))
            {
                //TODO reduce the unnecessary create and destroy 
                var reader = new Deserializer(backupData.Data);
                var hash = reader.ReadInt32();
                GameEntry.Service.GetService<CommonStateService>().Hash = hash;

                var oldId2Entity = _id2Entities;
                _id2Entities = new Dictionary<int, BaseEntity>();
                _type2Entities.Clear();

                // Recover Entities
                RecoverEntities(new List<Player>(), reader);

                // Rebind Ref
                foreach (var entity in _id2Entities.Values)
                {
                    entity.DoBindRef();
                }

                // Rebind Views 
                foreach (var pair in _id2Entities)
                {
                    BaseEntity oldEntity = null;
                    if (oldId2Entity.TryGetValue(pair.Key, out var poldEntity))
                    {
                        oldEntity = poldEntity;
                        oldId2Entity.Remove(pair.Key);
                    }
                    GameEntry.Service.GetService<GameViewService>().BindView(pair.Value, oldEntity);
                }

                // Unbind Entity views
                foreach (var pair in oldId2Entity)
                {
                    GameEntry.Service.GetService<GameViewService>().UnbindView(pair.Value);
                }
            }
            else
            {
                Debug.LogError($"Miss backup data  cannot rollback! {tick}");
            }
        }


        public void Clean(int maxVerifiedTick)
        {

        }

        void BackUpEntities<T>(T[] lst, Serializer writer) where T : BaseEntity, IBackup, new()
        {
            writer.Write(lst.Length);
            foreach (var item in lst)
            {
                item.WriteBackup(writer);
            }
        }

        List<T> RecoverEntities<T>(List<T> lst, Deserializer reader) where T : BaseEntity, IBackup, new()
        {
            var count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var t = new T();
                lst.Add(t);
                t.ReadBackup(reader);
            }

            _type2Entities[typeof(T)] = lst;
            foreach (var e in lst)
            {
                _id2Entities[e.EntityId] = e;
            }

            return lst;
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
            get => _curGameState.RemainTime;
            set => _curGameState.RemainTime = value;
        }

        public LFloat DeltaTime
        {
            get => _curGameState.DeltaTime;
            set => _curGameState.DeltaTime = value;
        }

        public int MaxEnemyCount
        {
            get => _curGameState.MaxEnemyCount;
            set => _curGameState.MaxEnemyCount = value;
        }

        public int CurEnemyCount
        {
            get => _curGameState.CurEnemyCount;
            set => _curGameState.CurEnemyCount = value;
        }

        public int CurEnemyId
        {
            get => _curGameState.CurEnemyId;
            set => _curGameState.CurEnemyId = value;
        }
    }
}