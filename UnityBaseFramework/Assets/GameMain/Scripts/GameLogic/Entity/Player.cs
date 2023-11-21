using System;
using Lockstep;
using Lockstep.Collision2D;
using Lockstep.Math;
using UnityBaseFramework.Runtime;

namespace XGame
{
    [Serializable]
    public partial class Player : CEntity
    {
        public int LocalId;

        public CMover Mover = new CMover();
        public CAnimator Animator = new CAnimator();
        //public CSkillBox skillBox = new CSkillBox();

        [NonSerialized]
        public ECamp Camp = ECamp.Default;

        public GameProto.Input Input = new GameProto.Input();

        public bool IsFire;

        //public int curHealth;
        //public int maxHealth = 100;
        //public int damage = 10;

        //public bool isInvincible;

        //public bool isDead => curHealth <= 0;

        protected override void BindRef()
        {
            base.BindRef();
            RegisterComponent(Mover);
            RegisterComponent(Animator);
            //RegisterComponent(skillBox);

            //TODO:读表
            MoveSpd = (LFloat)2.5f;
            TurnSpd = 360;
            //curHealth = maxHealth;
            ColliderData.radius = (LFloat)0.8f;
            //TODO:rigidbody
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Update(LFloat deltaTime)
        {
            base.Update(deltaTime);

            ////FireCD Temp;
            //if(m_CurrFireCd >= (LFloat)0.5f)
            //{
            //    if (transform != null && input != null && input.IsFire)
            //    {
            //        Fire();
            //        m_CurrFireCd = (LFloat)0f;
            //    }
            //}
            //else
            //{
            //    m_CurrFireCd += deltaTime;
            //}

            IsFire = Input.IsFire;
            if (CTransform != null && IsFire)
            {
                Fire();
            }
        }

        public void Fire()
        {
            //Log.Error($"Tick{World.Instance.Tick} Entity:{EntityId} Fire");
            int bulletId = 100001;
            Bullet bullet = GameEntry.Service.GetService<GameStateService>().CreateEntity<Bullet>(bulletId, CTransform.Pos3);
            bullet.CTransform.Pos3 = CTransform.Pos3;
            bullet.Dir = (new LVector2(new LFloat(true, Input.MousePosX), new LFloat(true, Input.MousePosY)) - CTransform.pos).normalized;
        }

        //public bool Fire(int idx = 0)
        //{
        //    //return skillBox.Fire(idx - 1);
        //    return false;
        //}

        //public void StopSkill(int idx = -1)
        //{
        //    //skillBox.ForceStop(idx);
        //}

        //public virtual void TakeDamage(BaseEntity atker, int amount, LVector3 hitPoint)
        //{
        //    if (isInvincible || isDead) return;
        //    Log.Info($"{atker.EntityId} attack {EntityId}  damage:{amount} hitPos:{hitPoint}");
        //    curHealth -= amount;
        //    //EntityView?.OnTakeDamage(amount, hitPoint);
        //    OnTakeDamage(amount, hitPoint);
        //    if (isDead)
        //    {
        //        OnDead();
        //    }
        //}

        //protected virtual void OnTakeDamage(int amount, LVector3 hitPoint) { }

        //protected virtual void OnDead()
        //{
        //    //EntityView?.OnDead();
        //    PhysicSystem.Instance.RemoveCollider(this);
        //    //GameStateService.DestroyEntity(this);
        //}
    }
}
