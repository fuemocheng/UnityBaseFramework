using System;
using Lockstep;
using Lockstep.Collision2D;
using Lockstep.Math;

namespace XGame
{
    [Serializable]
    public partial class Player : CEntity
    {
        public int localId;

        public CMover mover = new CMover();
        public CAnimator animator = new CAnimator();
        //public CSkillBox skillBox = new CSkillBox();

        [NonSerialized]
        public ECamp Camp = ECamp.Default;

        public GameProto.Input input = new GameProto.Input();

        //public int curHealth;
        //public int maxHealth = 100;
        //public int damage = 10;

        //public bool isInvincible;
        //public bool isFire;

        //public bool isDead => curHealth <= 0;

        protected override void BindRef()
        {
            base.BindRef();
            RegisterComponent(mover);
            RegisterComponent(animator);
            //RegisterComponent(skillBox);

            //TODO:读表
            moveSpd = (LFloat)2.5f;
            turnSpd = 360;
            //curHealth = maxHealth;
            colliderData.radius = (0.2f).ToLFloat();
            //TODO:rigidbody
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Update(LFloat deltaTime)
        {
            base.Update(deltaTime);

            if (transform != null && input != null && input.IsFire)
            {
                Fire();
            }
        }

        public void Fire()
        {
            int bulletId = 100001;
            Bullet bullet = GameEntry.Service.GetService<GameStateService>().CreateEntity<Bullet>(bulletId, transform.Pos3);
            bullet.transform.Pos3 = transform.Pos3;
            bullet.Dir = (new LVector2(new LFloat(true, input.MousePosX), new LFloat(true, input.MousePosY)) - transform.pos).normalized;
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
