using BaseFramework;
using BaseFramework.Event;
using System;
using System.Collections.Generic;
using UnityBaseFramework.Runtime;

namespace XGame 
{
    public class EntityLoader : IReference
    {
        private Dictionary<int, Action<Entity>> m_DicCallback;
        private Dictionary<int, Entity> m_DicSerialId2Entity;

        private List<int> tempList;

        public object Owner
        {
            get;
            private set;
        }

        public EntityLoader()
        {
            m_DicSerialId2Entity = new Dictionary<int, Entity>();
            m_DicCallback = new Dictionary<int, Action<Entity>>();
            tempList = new List<int>();
            Owner = null;
        }

        public static EntityLoader Create(object owner)
        {
            EntityLoader entityLoader = ReferencePool.Acquire<EntityLoader>();
            entityLoader.Owner = owner;
            GameEntry.Event.Subscribe(ShowEntitySuccessEventArgs.EventId, entityLoader.OnShowEntitySuccess);
            GameEntry.Event.Subscribe(ShowEntityFailureEventArgs.EventId, entityLoader.OnShowEntityFail);

            return entityLoader;
        }

        public void Clear()
        {
            Owner = null;
            m_DicSerialId2Entity.Clear();
            m_DicCallback.Clear();
            GameEntry.Event.Unsubscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
            GameEntry.Event.Unsubscribe(ShowEntityFailureEventArgs.EventId, OnShowEntityFail);
        }

        public int ShowEntity(int entityId, Type entityLogicType, Action<Entity> onShowSuccess, object userData = null)
        {
            int serialId = GameEntry.Entity.GenerateSerialId();
            m_DicCallback.Add(serialId, onShowSuccess);
            GameEntry.Entity.ShowEntity(serialId, entityId, entityLogicType, userData);
            return serialId;
        }

        public int ShowEntity<T>(int entityId, Action<Entity> onShowSuccess, object userData = null) where T : EntityLogic
        {
            int serialId = GameEntry.Entity.GenerateSerialId();
            m_DicCallback.Add(serialId, onShowSuccess);
            GameEntry.Entity.ShowEntity<T>(serialId, entityId, userData);
            return serialId;
        }

        public bool HasEntity(int serialId)
        {
            return GetEntity(serialId) != null;
        }

        public Entity GetEntity(int serialId)
        {
            if (m_DicSerialId2Entity.ContainsKey(serialId))
            {
                return m_DicSerialId2Entity[serialId];
            }

            return null;
        }

        public IEnumerable<Entity> GetAllEntities()
        {
            return m_DicSerialId2Entity.Values;
        }

        public void HideEntity(int serialId)
        {
            Entity entity = null;
            if (!m_DicSerialId2Entity.TryGetValue(serialId, out entity))
            {
                Log.Error("Can find entity('serial id:{0}') ", serialId);
            }

            m_DicSerialId2Entity.Remove(serialId);
            m_DicCallback.Remove(serialId);

            Entity[] entities = GameEntry.Entity.GetChildEntities(entity);
            if (entities != null)
            {
                foreach (var item in entities)
                {
                    //若Child Entity由这个Loader对象托管，则由此Loader释放
                    if (m_DicSerialId2Entity.ContainsKey(item.Id))
                    {
                        HideEntity(item);
                    }
                    else//若Child Entity不由这个Loader对象托管，则从Parent Entity脱离
                        GameEntry.Entity.DetachEntity(item);
                }
            }

            GameEntry.Entity.HideEntity(entity);
        }

        public void HideEntity(Entity entity)
        {
            if (entity == null)
                return;

            HideEntity(entity.Id);
        }

        public void HideAllEntity()
        {
            tempList.Clear();

            foreach (var entity in m_DicSerialId2Entity.Values)
            {
                Entity parentEntity = GameEntry.Entity.GetParentEntity(entity);
                //有ParentEntity
                if (parentEntity != null)
                {
                    //若Parent Entity由这个Loader对象托管，则把这个Child Entity从数据中移除，在隐藏Parent Entity，GF内部会处理Child Entity
                    if (m_DicSerialId2Entity.ContainsKey(parentEntity.Id))
                    {
                        m_DicSerialId2Entity.Remove(entity.Id);
                        m_DicCallback.Remove(entity.Id);
                    }
                    //若Parent Entity不由这个Loader对象托管，则从Parent Entity脱离
                    else
                    {
                        GameEntry.Entity.DetachEntity(entity);
                    }
                }
            }

            foreach (var serialId in m_DicSerialId2Entity.Keys)
            {
                tempList.Add(serialId);
            }

            foreach (var serialId in tempList)
            {
                HideEntity(serialId);
            }

            m_DicSerialId2Entity.Clear();
            m_DicCallback.Clear();
        }

        private void OnShowEntitySuccess(object sender, GameEventArgs e)
        {
            ShowEntitySuccessEventArgs ne = (ShowEntitySuccessEventArgs)e;
            if (ne == null)
            {
                return;
            }

            Action<Entity> callback = null;
            if (!m_DicCallback.TryGetValue(ne.Entity.Id, out callback))
            {
                return;
            }

            m_DicSerialId2Entity.Add(ne.Entity.Id, ne.Entity);
            callback?.Invoke(ne.Entity);
        }

        private void OnShowEntityFail(object sender, GameEventArgs e)
        {
            ShowEntityFailureEventArgs ne = (ShowEntityFailureEventArgs)e;
            if (ne == null)
            {
                return;
            }

            if (m_DicCallback.ContainsKey(ne.EntityId))
            {
                m_DicCallback.Remove(ne.EntityId);
                Log.Warning("{0} Show entity failure with error message '{1}'.", Owner.ToString(), ne.ErrorMessage);
            }
        }
    }
}