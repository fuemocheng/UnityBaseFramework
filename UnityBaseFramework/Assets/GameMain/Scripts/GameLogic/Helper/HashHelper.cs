using BaseFramework;
using BaseFramework.Network;
using GameProto;
using Lockstep.Game;
using Lockstep.Math;
using System.Collections.Generic;
using UnityBaseFramework.Runtime;

namespace XGame
{
    public class HashHelper
    {
        private int Tick => m_World.Tick;

        private World m_World;

        private FrameBuffer m_FrameBuffer;

        private int m_FirstHashTick = 0;

        private List<int> m_WaitToSendHashCodes = new List<int>();

        private Dictionary<int, int> m_AllHashCodes = new Dictionary<int, int>();

        public HashHelper(World world, FrameBuffer frameBuffer)
        {
            m_World = world;
            m_FrameBuffer = frameBuffer;
        }

        public int CalculateHash(bool isNeedTrace = false)
        {
            int idx = 0;
            return CalculateHash(ref idx, isNeedTrace);
        }

        private int CalculateHash(ref int idx, bool isNeedTrace)
        {
            int hashIdx = 0;
            int hashCode = 0;
            foreach (var svc in GameEntry.Service.GetAllServices())
            {
                if (svc is IHashCode hashSvc)
                {
                    hashCode += hashSvc.GetHash(ref hashIdx) * PrimerLUT.GetPrimer(hashIdx++);
                }
            }

            return hashCode;
        }

        public void SetHash(int tick, int hash)
        {
            if (tick < m_FirstHashTick)
            {
                return;
            }

            int count = tick - m_FirstHashTick;
            if (m_WaitToSendHashCodes.Count <= count)
            {
                for (int i = 0; i < count + 1; i++)
                {
                    m_WaitToSendHashCodes.Add(0);
                }
            }

            m_WaitToSendHashCodes[count] = hash;
            m_AllHashCodes[Tick] = hash;
        }

        public bool TryGetHash(int tick, out int hash)
        {
            return m_AllHashCodes.TryGetValue(tick, out hash);
        }

        public void CheckAndSendHashCodes()
        {
            // 本地Check过的帧。
            if (m_FrameBuffer.NextTickToCheck > m_FirstHashTick)
            {
                var count = LMath.Min(m_WaitToSendHashCodes.Count, (int)(m_FrameBuffer.NextTickToCheck - m_FirstHashTick), (480 / 4));
                if (count > 0)
                {
                    SendHashCodes(m_FirstHashTick, m_WaitToSendHashCodes, 0, count);

                    m_FirstHashTick = m_FirstHashTick + count;
                    m_WaitToSendHashCodes.RemoveRange(0, count);
                }
            }
        }

        private void SendHashCodes(int firstHashTick, List<int> hashCodes, int startIndex, int count)
        {
            INetworkChannel tcpChannel = GameEntry.NetworkExtended.TcpChannel;
            if (tcpChannel == null)
            {
                Log.Error("Cannot Ready, tcpChannel is null.");
                return;
            }

            CSHashCode csHashCode = ReferencePool.Acquire<CSHashCode>();
            csHashCode.StartTick = firstHashTick;
            for (int i = startIndex; i < count; i++)
            {
                csHashCode.HashCodes.Add(hashCodes[i]);
            }
            tcpChannel.Send(csHashCode);
        }
    }
}
