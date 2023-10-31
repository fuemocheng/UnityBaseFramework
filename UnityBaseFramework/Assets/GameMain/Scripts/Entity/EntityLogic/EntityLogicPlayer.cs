using Lockstep.Math;
using UnityBaseFramework.Runtime;
using UnityEngine;

namespace XGame
{
    public class EntityLogicPlayer : EntityLogicBase
    {
        private CEntity m_CEntity;
        private Player m_PlayerEntity;

        private ECamp m_Camp => m_PlayerEntity == null ? ECamp.Default : m_PlayerEntity.Camp;

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
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, deg, 0), 0.3f);
        }

        #region Bind

        public override void BindLSEntity(BaseEntity baseEntity, BaseEntity oldEntity = null)
        {
            base.BindLSEntity(baseEntity, oldEntity);
            baseEntity.EntityLogicBase = this;
            m_CEntity = baseEntity as CEntity;
            m_PlayerEntity = m_CEntity as Player;
        }

        #endregion

        #region Trigger

        public override void OnLPTriggerEnter(Lockstep.Collision2D.ColliderProxy other)
        {
            base.OnLPTriggerEnter(other);
            if (other.LayerType == (int)m_Camp)
            {
                Log.Error("OnLPTriggerEnter");
                m_CEntity.EntityLogicBase.Visible = false;
            }
        }

        public override void OnLPTriggerStay(Lockstep.Collision2D.ColliderProxy other)
        {
            base.OnLPTriggerStay(other);
            if (other.LayerType == (int)m_Camp)
            {
                //Log.Error("OnLPTriggerStay");
                m_CEntity.EntityLogicBase.Visible = false;
            }
        }

        public override void OnLPTriggerExit(Lockstep.Collision2D.ColliderProxy other)
        {
            base.OnLPTriggerExit(other);
            if (other.LayerType == (int)m_Camp)
            {
                Log.Error("OnLPTriggerExit");
                m_CEntity.EntityLogicBase.Visible = true;
            }
        }

        #endregion
    }
}
