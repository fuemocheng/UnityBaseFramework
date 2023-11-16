using System;

namespace XGame
{
    [Serializable]
    public partial class Enemy : CEntity
    {
        public CBrain brain = new CBrain();

        protected override void BindRef()
        {
            base.BindRef();
            RegisterComponent(brain);
            MoveSpd = 2;
            TurnSpd = 150;
        }
    }
}