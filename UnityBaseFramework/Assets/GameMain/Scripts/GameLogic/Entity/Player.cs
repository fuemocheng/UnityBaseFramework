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

        [NonSerialized]
        public ECamp Camp = ECamp.Default;

        protected override void BindRef()
        {
            base.BindRef();
            RegisterComponent(mover);
        }

        public override void Update(LFloat deltaTime)
        {
            base.Update(deltaTime);
        }
    }
}
