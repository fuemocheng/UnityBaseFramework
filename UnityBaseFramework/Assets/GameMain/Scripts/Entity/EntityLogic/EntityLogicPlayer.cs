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

        private GameProto.Input m_Input => m_PlayerEntity == null ? null : m_PlayerEntity.Input;

        private List<Lockstep.Collision2D.ColliderProxy> m_ColliderProxy = new();

        private Animator m_Animator;

        private Renderer[] m_Renderers;
        private MaterialPropertyBlock m_MaterialPropertyBlock;
        private Color m_ColorBlack = new Color(70 / 255f, 70 / 255f, 70 / 255f);
        private Color m_ColorWhite = new Color(255 / 255f, 255 / 255f, 255 / 255f);

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            m_Animator = GetComponentInChildren<Animator>();
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
            UpdateMove();
        }

        protected bool IsMyCamp()
        {
            return m_Camp == World.MyPlayer.Camp;
        }

        protected void UpdateMove()
        {
            if (m_CEntity == null)
            {
                return;
            }
            //更新位置。
            var pos = m_CEntity.CTransform.Pos3.ToVector3();
            transform.position = Vector3.Lerp(transform.position, pos, 0.3f);
            var deg = m_CEntity.CTransform.deg.ToFloat();
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, deg, 0), 0.3f);

            UpdateMoveAnimation();
            UpdateOtherAnimation();
        }

        protected void UpdateMoveAnimation()
        {
            if (m_Input == null)
                return;

            //此帧方向输入
            LVector2 inputUV = new LVector2(new LFloat(true, m_Input.InputH), new LFloat(true, m_Input.InputV));
            //朝向 deg，朝向鼠标的方向
            LVector2 mousePos = new LVector2(new LFloat(true, m_Input.MousePosX), new LFloat(true, m_Input.MousePosY));

            bool isMoving = inputUV.sqrMagnitude > new LFloat(true, 10);
            if (!isMoving)
            {
                //静止
                m_Animator?.SetFloat("MovingX", 0);
                m_Animator?.SetFloat("MovingY", 0);
                return;
            }

            //到这里播放跑或者走的动画。
            //运动方向与朝向的夹角，用于行走动画的表现。
            //faceDir 与 moveDir 的夹角。
            LFloat fmAngle;
            //是否加速。
            bool isSpeedUp = m_Input.IsSpeedUp;

            LVector2 faceDir = mousePos - m_PlayerEntity.CTransform.pos;
            LVector2 moveDir = inputUV;

            LFloat faceDeg = LMath.Atan2(faceDir.y, faceDir.x) * LMath.Rad2Deg;
            LFloat dirDeg = LMath.Atan2(moveDir.y, moveDir.x) * LMath.Rad2Deg;

            //计算夹角，并置于[0, 360]
            fmAngle = faceDeg - dirDeg;
            if (fmAngle < 0)
            {
                fmAngle += (LFloat)360;
            }

            if ((fmAngle >= 0 && fmAngle <= 22.5f) || fmAngle >= 337.5f)
            {
                //Forward
                if (isSpeedUp)
                {
                    m_Animator?.SetFloat("MovingX", 0);
                    m_Animator?.SetFloat("MovingY", 1);
                }
                else
                {
                    m_Animator?.SetFloat("MovingX", 0);
                    m_Animator?.SetFloat("MovingY", 0.5f);
                }
            }
            else if (fmAngle > 22.5f && fmAngle <= 67.5f)
            {
                //Forward Right
                if (isSpeedUp)
                {
                    m_Animator?.SetFloat("MovingX", 1);
                    m_Animator?.SetFloat("MovingY", 1);
                }
                else
                {
                    m_Animator?.SetFloat("MovingX", 0.5f);
                    m_Animator?.SetFloat("MovingY", 0.5f);
                }
            }
            else if (fmAngle > 67.5 && fmAngle <= 112.5f)
            {
                //Right
                if (isSpeedUp)
                {
                    m_Animator?.SetFloat("MovingX", 1);
                    m_Animator?.SetFloat("MovingY", 0);
                }
                else
                {
                    m_Animator?.SetFloat("MovingX", 0.5f);
                    m_Animator?.SetFloat("MovingY", 0);
                }
            }
            else if (fmAngle > 112.5 && fmAngle <= 157.5f)
            {
                //Backward Right
                if (isSpeedUp)
                {
                    m_Animator?.SetFloat("MovingX", 1);
                    m_Animator?.SetFloat("MovingY", -1);
                }
                else
                {
                    m_Animator?.SetFloat("MovingX", 0.5f);
                    m_Animator?.SetFloat("MovingY", -0.5f);
                }
            }
            else if (fmAngle > 157.5 && fmAngle <= 202.5f)
            {
                //Backward
                if (isSpeedUp)
                {
                    m_Animator?.SetFloat("MovingX", 0);
                    m_Animator?.SetFloat("MovingY", -1);
                }
                else
                {
                    m_Animator?.SetFloat("MovingX", 0);
                    m_Animator?.SetFloat("MovingY", -0.5f);
                }
            }
            else if (fmAngle > 202.5f && fmAngle <= 247.5f)
            {
                //Backward Left
                if (isSpeedUp)
                {
                    m_Animator?.SetFloat("MovingX", -1);
                    m_Animator?.SetFloat("MovingY", -1);
                }
                else
                {
                    m_Animator?.SetFloat("MovingX", -0.5f);
                    m_Animator?.SetFloat("MovingY", -0.5f);
                }
            }
            else if (fmAngle > 247.5f && fmAngle <= 292.5f)
            {
                //Left
                if (isSpeedUp)
                {
                    m_Animator?.SetFloat("MovingX", -1);
                    m_Animator?.SetFloat("MovingY", 0);
                }
                else
                {
                    m_Animator?.SetFloat("MovingX", -0.5f);
                    m_Animator?.SetFloat("MovingY", 0);
                }
            }
            else if (fmAngle > 292.5f && fmAngle < 337.5f)
            {
                //Forward Left
                if (isSpeedUp)
                {
                    m_Animator?.SetFloat("MovingX", -1);
                    m_Animator?.SetFloat("MovingY", 1);
                }
                else
                {
                    m_Animator?.SetFloat("MovingX", -0.5f);
                    m_Animator?.SetFloat("MovingY", 0.5f);
                }
            }
        }

        protected void UpdateOtherAnimation()
        {
            if (m_Input != null && m_Input.IsFire)
            {
                //射击动作
                m_Animator?.SetTrigger("FireFast");
            }
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
            //Log.Error($"OnLPTriggerEnter - Player");
            base.OnLPTriggerEnter(other);

            m_ColliderProxy.Add(other);
            if (other.LayerType == (int)m_Camp)
            {
                if (!IsMyCamp())
                {
                    m_CEntity.EntityLogicBase.Visible = false;
                }
            }

            if (other.LayerType == (int)EColliderLayer.Enemy)
            {
                //被击中，做被击动作
                m_Animator?.SetTrigger("BeHit");
                //TODO:计算伤害等等。
            }
        }

        public override void OnLPTriggerStay(Lockstep.Collision2D.ColliderProxy other)
        {
            //Log.Info("OnLPTriggerStay");
            base.OnLPTriggerStay(other);

            if (other.LayerType == (int)m_Camp)
            {
                if (!IsMyCamp())
                {
                    m_CEntity.EntityLogicBase.Visible = false;
                }
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
                if (m_ColliderProxy.Count == 0)
                {
                    m_CEntity.EntityLogicBase.Visible = true;
                }
            }
        }

        #endregion
    }
}
