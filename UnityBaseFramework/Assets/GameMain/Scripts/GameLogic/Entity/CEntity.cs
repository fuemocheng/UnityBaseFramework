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
        public ColliderData colliderData = new ColliderData() { radius = (0.1f).ToLFloat() };

        public LFloat moveSpd = (LFloat)2.5f;
        public LFloat turnSpd = (LFloat)360f;

        protected override void BindRef()
        {
            base.BindRef();
            rigidbody.BindRef(transform);
        }

        public override void Awake()
        {
            base.Awake();
        }

        public override void Start()
        {
            base.Start();
            rigidbody.DoStart();
        }

        public override void Update(LFloat deltaTime)
        {
            rigidbody.DoUpdate(deltaTime);
            base.Update(deltaTime);
        }

        public override void Destroy()
        {
            base.Destroy();
            PhysicSystem.Instance.RemoveCollider(this);
        }
    }
}