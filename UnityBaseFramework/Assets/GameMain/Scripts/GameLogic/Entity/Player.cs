using System;
using Lockstep;
using Lockstep.Math;

namespace XGame
{
    [Serializable]
    public partial class Player : CEntity
    {
        public int localId;
        //public PlayerInput input = new PlayerInput();
        public CMover mover = new CMover();

        protected override void BindRef()
        {
            base.BindRef();
            RegisterComponent(mover);
        }
        public override void Update(LFloat deltaTime)
        {
            base.Update(deltaTime);
            //if (input.skillId != 0)
            //{
            //    Fire(input.skillId);
            //}
        }
    }
}