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
                        float preSend = m_FrameBuffer._maxPing * 1.0f / CommonDefinitions.UpdateDeltatime;
                        m_TargetPreSendTick = m_TargetPreSendTick * m_OldPercent + preSend * (1 - m_OldPercent);

                        int targetPreSendTick = LMath.Clamp((int)System.Math.Ceiling(m_TargetPreSendTick), 1, 60);
#if UNITY_EDITOR
                        //if (targetPreSendTick != m_Simulator.PreSendInputCount) 
                        {
                            Log.Warning($"Shrink preSend buffer old:{m_Simulator.PreSendInputCount} new:{m_TargetPreSendTick} " +
                                $"PING: min:{m_FrameBuffer._minPing} max:{m_FrameBuffer._maxPing} avg:{m_FrameBuffer.PingVal}");
                        }
#endif
                        m_Simulator.PreSendInputCount = targetPreSendTick;
                    }

                    HasMissTick = false;
                }

                if (MissTick != -1)
                {
                    int delayTick = m_Simulator.TargetTick - MissTick;
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
