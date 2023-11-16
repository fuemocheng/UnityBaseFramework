using System;
using Lockstep;
using Lockstep.Collision2D;
using Lockstep.Math;

namespace XGame
{
    [Serializable]
    public partial class Bullet : CEntity
    {
        public LVector2 Dir;        //normalized;
        public LFloat MaxTime;
        public LFloat CurrTime;

        protected override void BindRef()
        {
            base.BindRef();
            //TODO:读表。
            MoveSpd = (LFloat)40f;
            MaxTime = (LFloat)2f;
            ColliderData.radius = (0.1f).ToLFloat();
        }

        public override void Awake()
        {
            base.Awake();
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Update(LFloat deltaTime)
        {
            if (CurrTime > MaxTime)
            {
                return;
            }
            base.Update(deltaTime);
            CurrTime += deltaTime;
            CTransform.pos = CTransform.pos + Dir * MoveSpd * deltaTime;

            if (CurrTime >= MaxTime)
            {
                GameEntry.Service.GetService<GameStateService>().DestroyEntity(this);
            }
        }

        public override void Destroy()
        {
            base.Destroy();
        }
    }
}
