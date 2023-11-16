using Lockstep.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGame
{
    public class EntityLogicBullet : EntityLogicBase
    {
        private CEntity m_CEntity;
        private Bullet m_BulletEntity;

        private TrailRenderer m_TrailRenderer;
        private bool m_IsFirstFrame = true;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            m_TrailRenderer = GetComponentInChildren<TrailRenderer>();
            m_IsFirstFrame = true;
        }

        protected override void OnRecycle()
        {
            base.OnRecycle();
        }

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            m_IsFirstFrame = true;
            m_TrailRenderer.enabled = false;
        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
            m_TrailRenderer.enabled = false;
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            if (m_CEntity == null)
            {
                return;
            }
            //if (m_IsFirstFrame)
            //{
            //    m_IsFirstFrame = false;
            //    var pos = m_CEntity.transform.Pos3.ToVector3();
            //    transform.position = pos;
            //    var deg = m_CEntity.transform.deg.ToFloat();
            //    transform.rotation = Quaternion.Euler(0, deg, 0);
            //    m_TrailRenderer.enabled = false;
            //}
            //else
            //{
            //    m_TrailRenderer.enabled = true;
            //    //更新位置。
            //    var pos = m_CEntity.transform.Pos3.ToVector3();
            //    transform.position = Vector3.Lerp(transform.position, pos, 0.3f);
            //    var deg = m_CEntity.transform.deg.ToFloat();
            //    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, deg, 0), 0.3f);
            //}

            //更新位置。
            var pos = m_CEntity.CTransform.Pos3.ToVector3();
            transform.position = Vector3.Lerp(transform.position, pos, 0.3f);
            var deg = m_CEntity.CTransform.deg.ToFloat();
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, deg, 0), 0.3f);

            //Debug.LogError($"BulletLogicPos:{transform.position}");
        }

        public override void OnRollbackDestroy()
        {
            base.OnRollbackDestroy();
            m_CEntity = null;
            m_BulletEntity = null;
        }

        #region Bind

        public override void BindLSEntity(BaseEntity baseEntity)
        {
            base.BindLSEntity(baseEntity);

            baseEntity.EntityLogicBase = this;
            m_CEntity = baseEntity as CEntity;
            m_BulletEntity = m_CEntity as Bullet;
        }

        #endregion

        #region Trigger

        public override void OnLPTriggerEnter(Lockstep.Collision2D.ColliderProxy other)
        {
            //Log.Info("OnLPTriggerEnter");
            base.OnLPTriggerEnter(other);
        }

        public override void OnLPTriggerStay(Lockstep.Collision2D.ColliderProxy other)
        {
            //Log.Info("OnLPTriggerStay");
            base.OnLPTriggerStay(other);
        }

        public override void OnLPTriggerExit(Lockstep.Collision2D.ColliderProxy other)
        {
            //Log.Info("OnLPTriggerExit");
            base.OnLPTriggerExit(other);
        }

        #endregion
    }
}
