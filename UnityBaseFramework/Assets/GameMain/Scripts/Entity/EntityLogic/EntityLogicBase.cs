using BaseFramework;
using UnityEngine;
using UnityBaseFramework.Runtime;
using Lockstep.Math;

namespace XGame
{
    public abstract class EntityLogicBase : EntityLogic
    {
        [SerializeField]
        private EntityData m_EntityData = null;

        public int Id
        {
            get
            {
                return Entity.Id;
            }
        }

        public Animation CachedAnimation
        {
            get;
            private set;
        }

        /// <summary>
        /// LockStep Entity。
        /// </summary>
        private BaseEntity m_BaseEntity;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            CachedAnimation = GetComponent<Animation>();
        }

        protected override void OnRecycle()
        {
            base.OnRecycle();
        }

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);

            m_EntityData = userData as EntityData;
            if (m_EntityData == null)
            {
                Log.Error("Entity data is invalid.");
                return;
            }

            Name = Utility.Text.Format("[Entity {0}]", Id);
            CachedTransform.localPosition = m_EntityData.Position;
            CachedTransform.localRotation = m_EntityData.Rotation;
            CachedTransform.localScale = Vector3.one;
        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
        }

        protected override void OnAttached(EntityLogic childEntity, Transform parentTransform, object userData)
        {
            base.OnAttached(childEntity, parentTransform, userData);
        }

        protected override void OnDetached(EntityLogic childEntity, object userData)
        {
            base.OnDetached(childEntity, userData);
        }

        protected override void OnAttachTo(EntityLogic parentEntity, Transform parentTransform, object userData)
        {
            base.OnAttachTo(parentEntity, parentTransform, userData);
        }

        protected override void OnDetachFrom(EntityLogic parentEntity, object userData)
        {
            base.OnDetachFrom(parentEntity, userData);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
        }

        public virtual void BindLSEntity(BaseEntity baseEntity, BaseEntity oldEntity = null)
        {
            baseEntity.EntityLogicBase = this;
            this.m_BaseEntity = baseEntity;

            BaseEntity updateEntity = oldEntity ?? baseEntity;
            transform.position = updateEntity.transform.Pos3.ToVector3();
            transform.rotation = Quaternion.Euler(0, updateEntity.transform.deg.ToFloat(), 0);
        }

        public virtual void OnRollbackDestroy()
        {
            //回收
        }
    }
}
