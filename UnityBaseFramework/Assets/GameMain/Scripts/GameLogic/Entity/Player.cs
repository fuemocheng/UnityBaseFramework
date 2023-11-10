using System;
using Lockstep;
using Lockstep.Math;

namespace XGame
{
    [Serializable]
    public partial class Player : CEntity
    {
        public int localId;
        public GameProto.Input input = new GameProto.Input();
        public CMover mover = new CMover();
        public CAnimator animator = new CAnimator();
        public float FMAngle = -1;
        public bool IsSpeedUp = false;

        [NonSerialized]
        public ECamp Camp = ECamp.Default;

        protected override void BindRef()
        {
            base.BindRef();
            RegisterComponent(mover);
            RegisterComponent(animator);
        }

        public override void Update(LFloat deltaTime)
        {
            base.Update(deltaTime);
        }
    }
}
