using System;
using Lockstep;
using Lockstep.Collision2D;
using Lockstep.Game;
using Lockstep.Math;
using UnityBaseFramework.Runtime;

namespace XGame
{
    [Serializable]
    [NoBackup]
    public partial class CEntity : BaseEntity
    {
        public CRigidbody rigidbody = new CRigidbody();
        public ColliderData colliderData = new ColliderData();
        public CAnimator animator = new CAnimator();
        public CSkillBox skillBox = new CSkillBox();

        public LFloat moveSpd = 5;
        public LFloat turnSpd = 360;
        public int curHealth;
        public int maxHealth = 100;
        public int damage = 10;

        public bool isInvincible;
        public bool isFire;

        public bool isDead => curHealth <= 0;


        protected override void BindRef()
        {
            base.BindRef();
            RegisterComponent(animator);
            RegisterComponent(skillBox);
            rigidbody.BindRef(transform);
        }

        public override void Start()
        {
            base.Start();
            rigidbody.DoStart();
            curHealth = maxHealth;
        }

        public override void Update(LFloat deltaTime)
        {
            rigidbody.DoUpdate(deltaTime);
            base.Update(deltaTime);
        }

        public bool Fire(int idx = 0)
        {
            return skillBox.Fire(idx - 1);
        }

        public void StopSkill(int idx = -1)
        {
            skillBox.ForceStop(idx);
        }

        public virtual void TakeDamage(BaseEntity atker, int amount, LVector3 hitPoint)
        {
            if (isInvincible || isDead) return;
            Log.Info($"{atker.EntityId} attack {EntityId}  damage:{amount} hitPos:{hitPoint}");
            curHealth -= amount;
            //EntityView?.OnTakeDamage(amount, hitPoint);
            OnTakeDamage(amount, hitPoint);
            if (isDead)
            {
                OnDead();
            }
        }

        protected virtual void OnTakeDamage(int amount, LVector3 hitPoint) { }

        protected virtual void OnDead()
        {
            //EntityView?.OnDead();
            PhysicSystem.Instance.RemoveCollider(this);
            //GameStateService.DestroyEntity(this);
        }
    }
}