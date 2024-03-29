using Lockstep.Math;
using UnityBaseFramework.Runtime;

namespace XGame
{
    public partial class FrameBuffer
    {
        public class PredictCountHelper
        {
            private Simulator m_Simulator;
            private FrameBuffer m_FrameBuffer;

            public int MissTick = -1;
            public int NextCheckMissTick = 0;
            public bool HasMissTick;

            private float m_Timer;
            private float m_CheckInterval = 0.5f;
            private float m_IncPercent = 0.3f;

            private float m_TargetPreSendTick;
            private float m_OldPercent = 0.6f;


            public PredictCountHelper(Simulator simulator, FrameBuffer frameBuffer)
            {
                m_Simulator = simulator;
                m_FrameBuffer = frameBuffer;
            }


            public void Update(float deltaTime)
            {
                m_Timer += deltaTime;
                if (m_Timer > m_CheckInterval)
                {
                    m_Timer = 0;
                    if (!HasMissTick)
                    {
                        float preSend = m_FrameBuffer.m_MaxPing * 1.0f / CommonDefinitions.UpdateDeltatime;
                        m_TargetPreSendTick = m_TargetPreSendTick * m_OldPercent + preSend * (1 - m_OldPercent);

                        int targetPreSendTick = LMath.Clamp((int)System.Math.Ceiling(m_TargetPreSendTick), 1, 60);
#if UNITY_EDITOR
                        if (targetPreSendTick != m_Simulator.PreSendInputCount)
                        {
                            //缩小预发送缓冲；
                            Log.Warning($"Shrink preSend buffer old:{m_Simulator.PreSendInputCount} new:{m_TargetPreSendTick} " +
                                $"PING: min:{m_FrameBuffer.m_MinPing} max:{m_FrameBuffer.m_MaxPing} avg:{m_FrameBuffer.PingVal}");
                        }
#endif
                        m_Simulator.PreSendInputCount = targetPreSendTick;
                    }

                    HasMissTick = false;
                }

                if (MissTick != -1)
                {
                    //目标帧与丢帧之间的延迟帧数。
                    int delayTick = m_Simulator.TargetTick - MissTick;
                    //在有丢帧的情况下，预测的预发送帧数量将增加当前延迟帧数的30%。
                    int targetPreSendTick = m_Simulator.PreSendInputCount + (int)System.Math.Ceiling(delayTick * m_IncPercent);
                    targetPreSendTick = LMath.Clamp(targetPreSendTick, 1, 60);
#if UNITY_EDITOR
                    Log.Warning($"Expend preSend buffer old:{m_Simulator.PreSendInputCount} new:{targetPreSendTick}");
#endif
                    m_Simulator.PreSendInputCount = targetPreSendTick;
                    NextCheckMissTick = m_Simulator.TargetTick;
                    MissTick = -1;
                    HasMissTick = true;
                }
            }
        }
    }
}
