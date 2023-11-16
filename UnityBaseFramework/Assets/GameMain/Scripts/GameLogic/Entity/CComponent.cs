using System;
using Lockstep;
using Lockstep.Collision2D;
using Lockstep.Math;

namespace XGame
{
    [Serializable]
    [NoBackup]
    public partial class CComponent : IComponent
    {
        public BaseEntity baseEntity
        {
            get;
            private set;
        }

        public CEntity entity
        {
            get
            {
                return (CEntity)baseEntity;
            }
        }

        public CTransform2D transform
        {
            get;
            private set;
        }

        public virtual void BindEntity(BaseEntity entity)
        {
            this.baseEntity = entity;
            transform = entity.CTransform;
        }

        public virtual void Awake()
        {
        }

        public virtual void Start()
        {
        }

        public virtual void Update(LFloat deltaTime)
        {
        }

        public virtual void Destroy()
        {
        }
    }
}