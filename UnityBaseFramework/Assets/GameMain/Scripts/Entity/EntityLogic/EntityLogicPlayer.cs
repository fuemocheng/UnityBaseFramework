using Lockstep.Math;
using UnityEngine;

namespace XGame
{
    public class EntityLogicPlayer : EntityLogicBase
    {
        private CEntity m_CEntity;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
        }

        protected override void OnRecycle()
        {
            base.OnRecycle();
        }

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            //更新位置。
            var pos = m_CEntity.transform.Pos3.ToVector3();
            transform.position = Vector3.Lerp(transform.position, pos, 0.3f);
            var deg = m_CEntity.transform.deg.ToFloat();
            //deg = Mathf.Lerp(transform.rotation.eulerAngles.y, deg, 0.3f);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, deg, 0), 0.3f);
        }

        public override void BindLSEntity(BaseEntity baseEntity, BaseEntity oldEntity = null)
        {
            base.BindLSEntity(baseEntity, oldEntity);
            baseEntity.EntityLogicBase = this;
            m_CEntity = baseEntity as CEntity;
        }
    }
}
