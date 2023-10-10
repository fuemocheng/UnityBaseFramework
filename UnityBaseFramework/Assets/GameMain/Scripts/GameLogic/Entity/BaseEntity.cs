using System;
using System.Collections.Generic;
using Lockstep;
using Lockstep.Collision2D;
using Lockstep.Game;
using Lockstep.Math;

namespace XGame
{
    [Serializable]
    [NoBackup]
    public partial class BaseEntity : IEntity, ILPTriggerEventHandler
    {
        public int EntityId;
        public int PrefabId;
        public CTransform2D transform = new CTransform2D();
        [NoBackup] public object engineTransform;

        protected List<CComponent> m_AllComponents;

        [ReRefBackup] public IEntityView EntityView;

        public void DoBindRef()
        {
            BindRef();
        }

        public virtual void OnRollbackDestroy()
        {
            EntityView?.OnRollbackDestroy();
            EntityView = null;
            engineTransform = null;
        }

        protected virtual void BindRef()
        {
            m_AllComponents?.Clear();
        }

        protected void RegisterComponent(CComponent comp)
        {
            if (m_AllComponents == null)
            {
                m_AllComponents = new List<CComponent>();
            }
            m_AllComponents.Add(comp);
            comp.BindEntity(this);
        }

        public virtual void Awake()
        {
            if (m_AllComponents == null) return;
            foreach (var comp in m_AllComponents)
            {
                comp.Awake();
            }
        }

        public virtual void Start()
        {
            if (m_AllComponents == null) return;
            foreach (var comp in m_AllComponents)
            {
                comp.Start();
            }
        }

        public virtual void Update(LFloat deltaTime)
        {
            if (m_AllComponents == null) return;
            foreach (var comp in m_AllComponents)
            {
                comp.Update(deltaTime);
            }
        }

        public virtual void Destroy()
        {
            if (m_AllComponents == null) return;
            foreach (var comp in m_AllComponents)
            {
                comp.Destroy();
            }
        }

        public virtual void OnLPTriggerEnter(ColliderProxy other)
        {
        }

        public virtual void OnLPTriggerStay(ColliderProxy other)
        {
        }

        public virtual void OnLPTriggerExit(ColliderProxy other)
        {
        }
    }
}