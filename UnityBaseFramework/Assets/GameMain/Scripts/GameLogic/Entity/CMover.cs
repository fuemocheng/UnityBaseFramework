using System;
using Lockstep.Collision2D;
using Lockstep.Math;
using UnityBaseFramework.Runtime;
using UnityEngine;

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

            //此帧速度，是否加速
            LFloat currSpeed = input.IsSpeedUp ? (speed * (LFloat)2) : speed;
            //此帧方向输入
            LVector2 inputUV = new LVector2(new LFloat(true, input.InputH), new LFloat(true, input.InputV));

            bool needChase = inputUV.sqrMagnitude > new LFloat(true, 10);
            if (needChase)
            {
                LVector2 dir = inputUV.normalized;
                transform.pos = transform.pos + dir * currSpeed * deltaTime;

                //LFloat targetDeg = dir.ToDeg();
                //transform.deg = CTransform2D.TurnToward(targetDeg, transform.deg, player.turnSpd * deltaTime, out var hasReachDeg);
            }

            hasReachTarget = !needChase;

            //朝向 deg，朝向鼠标的方向
            LVector2 mousePos = new LVector2(new LFloat(true, input.MousePosX), new LFloat(true, input.MousePosY));
            if (!mousePos.Equals(LVector2.zero))
            {
                LFloat targetDeg = (mousePos - transform.pos).ToDeg();
                transform.deg = CTransform2D.TurnToward(targetDeg, transform.deg, player.turnSpd * deltaTime, out var hasReachDeg);
            }
            else
            {
                LVector2 dir = inputUV.normalized;
                LFloat targetDeg = dir.ToDeg();
                transform.deg = CTransform2D.TurnToward(targetDeg, transform.deg, player.turnSpd * deltaTime, out var hasReachDeg);
            }

        }
    }
}