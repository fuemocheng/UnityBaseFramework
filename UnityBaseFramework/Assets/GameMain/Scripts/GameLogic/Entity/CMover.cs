using System;
using Lockstep.Collision2D;
using Lockstep.Math;
using UnityBaseFramework.Runtime;

namespace XGame
{
    [Serializable]
    public partial class CMover : CComponent
    {
        public Player player => (Player)baseEntity;
        public GameProto.Input input => player.input;

        static LFloat _sqrStopDist = new LFloat(true, 40);
        public LFloat speed => player.moveSpd;
        public bool hasReachTarget = false;
        public bool needMove = true;

        public override void Update(LFloat deltaTime)
        {
            if (!entity.rigidbody.isOnFloor)
            {
                return;
            }

            //input.InputV = 1000;
            //input.InputH = 1000;

            LVector2 inputUV = new LVector2(new LFloat(true, input.InputH), new LFloat(true, input.InputV));

            var needChase = inputUV.sqrMagnitude > new LFloat(true, 10);
            if (needChase)
            {
                //Log.Error("needChase" + inputUV.x + " " + inputUV.y);
                var dir = inputUV.normalized;
                transform.pos = transform.pos + dir * speed * deltaTime;
                var targetDeg = dir.ToDeg();
                transform.deg = CTransform2D.TurnToward(targetDeg, transform.deg, player.turnSpd * deltaTime, out var hasReachDeg);
            }

            hasReachTarget = !needChase;
        }
    }
}