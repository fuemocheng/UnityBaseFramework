using Lockstep.Math;
using System.Collections.Generic;
using UnityBaseFramework.Runtime;
using UnityEngine;

namespace XGame
{
    public class EntityLogicPlayer : EntityLogicBase
    {
        private CEntity m_CEntity;
        private Player m_PlayerEntity;

        private ECamp m_Camp => m_PlayerEntity == null ? ECamp.Default : m_PlayerEntity.Camp;

        private List<Lockstep.Collision2D.ColliderProxy> m_ColliderProxy = new();

        private Renderer[] m_Renderers;
        private MaterialPropertyBlock m_MaterialPropertyBlock;
        private Color m_ColorBlack = new Color(70 / 255f, 70 / 255f, 70 / 255f);
        private Color m_ColorWhite = new Color(255 / 255f, 255 / 255f, 255 / 255f);

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);

            m_Renderers = GetComponentsInChildren<Renderer>();
            m_MaterialPropertyBlock = new MaterialPropertyBlock();
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

        public override void BindLSEntity(BaseEntity baseEntity)
        {
            base.BindLSEntity(baseEntity);

            baseEntity.EntityLogicBase = this;
            m_CEntity = baseEntity as CEntity;
            m_PlayerEntity = m_CEntity as Player;

            //ChangeColor
            if (m_PlayerEntity.Camp == ECamp.Black)
            {
                m_MaterialPropertyBlock.SetColor("_Color", m_ColorBlack);
                for (int i = 0; i < m_Renderers.Length; i++)
                {
                    m_Renderers[i].SetPropertyBlock(m_MaterialPropertyBlock);
                }
            }
            else if (m_PlayerEntity.Camp == ECamp.White)
            {
                m_MaterialPropertyBlock.SetColor("_Color", m_ColorWhite);
                for (int i = 0; i < m_Renderers.Length; i++)
                {
                    m_Renderers[i].SetPropertyBlock(m_MaterialPropertyBlock);
                }
            }
        }

        #endregion

        #region Trigger

        public override void OnLPTriggerEnter(Lockstep.Collision2D.ColliderProxy other)
        {
            //Log.Info("OnLPTriggerEnter");
            base.OnLPTriggerEnter(other);

            m_ColliderProxy.Add(other);
            if (other.LayerType == (int)m_Camp)
            {
                m_CEntity.EntityLogicBase.Visible = false;
            }
        }

        public override void OnLPTriggerStay(Lockstep.Collision2D.ColliderProxy other)
        {
            //Log.Info("OnLPTriggerStay");
            base.OnLPTriggerStay(other);

            if (other.LayerType == (int)m_Camp)
            {
                m_CEntity.EntityLogicBase.Visible = false;
            }
            else
            {
                m_CEntity.EntityLogicBase.Visible = true;
            }
        }

        public override void OnLPTriggerExit(Lockstep.Collision2D.ColliderProxy other)
        {
            //Log.Info("OnLPTriggerExit");
            base.OnLPTriggerExit(other);

            m_ColliderProxy.Remove(other);
            if (other.LayerType == (int)m_Camp)
            {
                //解决相邻同色块跨越的时候会闪现的问题；
                if(m_ColliderProxy.Count == 0)
                {
                    m_CEntity.EntityLogicBase.Visible = true;
                }
            }
        }

        #endregion
    }
}
