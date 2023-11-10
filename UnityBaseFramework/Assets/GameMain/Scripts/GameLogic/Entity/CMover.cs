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

            var needChase = inputUV.sqrMagnitude > new LFloat(true, 10);
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

            //运动方向与朝向的夹角，用于行走动画的表现，-1表示Idle
            //计算 moveDir 与 faceDir 的夹角
            if (needChase)
            {
                LVector2 faceDir = mousePos - transform.pos;
                LVector2 moveDir = inputUV;

                LFloat faceDeg = LMath.Atan2(faceDir.y, faceDir.x) * LMath.Rad2Deg;
                LFloat dirDeg = LMath.Atan2(moveDir.y, moveDir.x) * LMath.Rad2Deg;

                //计算夹角，并置于 [0, 360]
                LFloat diffDeg = faceDeg - dirDeg;
                if (diffDeg < 0)
                {
                    diffDeg += (LFloat)360;
                }
                player.FMAngle = diffDeg;
            }
            else
            {
                player.FMAngle = -1;
            }
            player.IsSpeedUp = input.IsSpeedUp;
        }
    }
}