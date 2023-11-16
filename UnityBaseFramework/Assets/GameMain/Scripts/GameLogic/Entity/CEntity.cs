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
        public CRigidbody Rigidbody = new CRigidbody();
        public ColliderData ColliderData = new ColliderData() { radius = (0.1f).ToLFloat() };

        public LFloat MoveSpd = (LFloat)2.5f;
        public LFloat TurnSpd = (LFloat)360f;

        protected override void BindRef()
        {
            base.BindRef();
            Rigidbody.BindRef(CTransform);
        }

        public override void Awake()
        {
            base.Awake();
        }

        public override void Start()
        {
            base.Start();
            Rigidbody.DoStart();
        }

        public override void Update(LFloat deltaTime)
        {
            Rigidbody.DoUpdate(deltaTime);
            base.Update(deltaTime);
        }

        public override void Destroy()
        {
            base.Destroy();
            PhysicSystem.Instance.RemoveCollider(this);
        }
    }
}