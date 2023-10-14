using BaseFramework;
using BaseFramework.Network;
using GameProto;
using Lockstep.Math;
using Lockstep.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityBaseFramework.Runtime;

namespace XGame
{
    public partial class FrameBuffer
    {
        private Simulator m_Simulator;
        private PredictCountHelper m_PredictCountHelper;

        //buffers
        private int m_MaxClientPredictFrameCount;
        private int m_BufferSize;
        private int m_SpaceRollbackNeed;
        private int m_MaxServerOverFrameCount;

        private ServerFrame[] m_ServerBuffer;
        private ServerFrame[] m_ClientBuffer;

        //ping 
        public int PingVal { get; private set; }
        private long m_GuessServerStartTimestamp = Int64.MaxValue;
        private long m_MinPing = Int64.MaxValue;
        private long m_MaxPing = Int64.MinValue;
        private long m_HistoryMinPing = Int64.MaxValue;
        private float m_PingTimer;
        private List<long> m_Pings = new List<long>();
        public int DelayVal { get; private set; }
        private List<long> m_Delays = new List<long>();
        private Dictionary<int, long> m_Tick2SendTimestamp = new Dictionary<int, long>();

        // the tick client need run in next update
        private int m_NextClientTick;

        public int CurTickInServer { get; private set; }
        public int NextTickToCheck { get; private set; }
        public int MaxServerTickInBuffer { get; private set; } = -1;
        public bool IsNeedRollback { get; private set; }
        public int MaxContinueServerTick { get; private set; }

        public int LocalId;

        public FrameBuffer(Simulator simulator, int bufferSize, int snapshotFrameInterval, int maxClientPredictFrameCount)
        {
            m_Simulator = simulator;
            m_PredictCountHelper = new PredictCountHelper(m_Simulator, this);

            m_BufferSize = bufferSize;
            m_MaxClientPredictFrameCount = maxClientPredictFrameCount;
            m_SpaceRollbackNeed = snapshotFrameInterval * 2;
            m_MaxServerOverFrameCount = bufferSize - m_SpaceRollbackNeed;
            m_ServerBuffer = new ServerFrame[bufferSize];
            m_ClientBuffer = new ServerFrame[bufferSize];
        }

        public void SetClientTick(int tick)
        {
            m_NextClientTick = tick + 1;
        }

        public void OnPing(SCPingEventArgs scPingEventArgs)
        {
            long ping = GameTime.CurrTimeStamp - scPingEventArgs.SendTimestamp;
            m_Pings.Add(ping);
            if (ping > m_MaxPing)
            {
                m_MaxPing = ping;
            }
            if (ping < m_MinPing)
            {
                m_MinPing = ping;

                // 推测服务器开始的时间戳。
                m_GuessServerStartTimestamp = GameTime.CurrTimeStamp - scPingEventArgs.TimeSinceServerStart - m_MinPing / 2;
            }
        }

        public void PushLocalFrame(ServerFrame frame)
        {
            var sIdx = frame.Tick % m_BufferSize;
            //if (m_ClientBuffer[sIdx] == null || m_ClientBuffer[sIdx].Tick <= frame.Tick)
            //{
            //    Log.Error("Push local frame error!");
            //}
            m_ClientBuffer[sIdx] = frame;
        }

        public void PushMissServerFrames(ServerFrame[] frames, bool isNeedDebugCheck = true)
        {
            PushServerFrames(frames, isNeedDebugCheck);

            //不断发送，追帧。
            Log.Error($"SendReqMissFrame: {MaxContinueServerTick}");
            SendReqMissFrame(MaxContinueServerTick + 1);
        }

        public void PushServerFrames(ServerFrame[] frames, bool isNeedDebugCheck = true)
        {
            var count = frames.Length;
            for (int i = 0; i < count; i++)
            {
                var data = frames[i];

                if (m_Tick2SendTimestamp.TryGetValue(data.Tick, out var sendTick))
                {
                    var delay = GameTime.CurrTimeStamp - sendTick;
                    m_Delays.Add(delay);
                    m_Tick2SendTimestamp.Remove(data.Tick);
                }

                if (data.Tick < NextTickToCheck)
                {
                    //the frame is already checked
                    continue;
                }

                if (data.Tick > CurTickInServer)
                {
                    CurTickInServer = data.Tick;
                }

                if (data.Tick >= NextTickToCheck + m_MaxServerOverFrameCount - 1)
                {
                    //to avoid ringBuffer override the frame that have not been checked
                    continue;
                }

                if (data.Tick > MaxServerTickInBuffer)
                {
                    MaxServerTickInBuffer = data.Tick;
                }

                var targetIdx = data.Tick % m_BufferSize;
                if (m_ServerBuffer[targetIdx] == null || m_ServerBuffer[targetIdx].Tick != data.Tick)
                {
                    m_ServerBuffer[targetIdx] = data;

                    //Log.Error($"PushServerFrame: {targetIdx}");

                    if (data.Tick > m_PredictCountHelper.NextCheckMissTick &&
                        data.InputFrames[LocalId].IsMiss &&
                        m_PredictCountHelper.MissTick == -1)
                    {
                        m_PredictCountHelper.MissTick = data.Tick;
                    }
                }
            }
        }

        public void Update(float deltaTime)
        {
            SendPing();

            m_PredictCountHelper.Update(deltaTime);
            int worldTick = m_Simulator.World.Tick;

            UpdatePing(deltaTime);

            //Debug.Assert(nextTickToCheck <= nextClientTick, "localServerTick <= localClientTick ");
            //Confirm frames
            IsNeedRollback = false;
            while (NextTickToCheck <= MaxServerTickInBuffer && NextTickToCheck < worldTick)
            {
                var sIdx = NextTickToCheck % m_BufferSize;
                var cFrame = m_ClientBuffer[sIdx];
                var sFrame = m_ServerBuffer[sIdx];
                if (cFrame == null || cFrame.Tick != NextTickToCheck ||
                    sFrame == null || sFrame.Tick != NextTickToCheck)
                {
                    break;
                }
                //Check client guess input match the real input
                if (object.ReferenceEquals(sFrame, cFrame) || sFrame.Equals(cFrame))
                {
                    NextTickToCheck++;
                }
                else
                {
                    IsNeedRollback = true;
                    break;
                }
            }

            //Request miss frame data
            int tick = NextTickToCheck;
            for (; tick <= MaxServerTickInBuffer; tick++)
            {
                var idx = tick % m_BufferSize;
                if (m_ServerBuffer[idx] == null || m_ServerBuffer[idx].Tick != tick)
                {
                    break;
                }
            }

            MaxContinueServerTick = tick - 1;
            if (MaxContinueServerTick <= 0)
            {
                return;
            }

            // has some middle frame pack was lost, or client has predict too much.
            if (MaxContinueServerTick < CurTickInServer || m_NextClientTick > MaxContinueServerTick + (m_MaxClientPredictFrameCount - 3))
            {
                Log.Error($"SendReqMissFrame: {MaxContinueServerTick}");
                SendReqMissFrame(MaxContinueServerTick);
            }
        }

        private void UpdatePing(float deltaTime)
        {
            m_PingTimer += deltaTime;
            if (m_PingTimer > 0.5f)
            {
                m_PingTimer = 0;

                DelayVal = (int)(m_Delays.Sum() / LMath.Max(m_Delays.Count, 1));
                m_Delays.Clear();

                PingVal = (int)(m_Pings.Sum() / LMath.Max(m_Pings.Count, 1));
                m_Pings.Clear();

                if (m_MinPing < m_HistoryMinPing && m_Simulator.GameStartTimestampMs != -1)
                {
                    m_HistoryMinPing = m_MinPing;

                    //服务器开始时间要减去延迟执行的帧。
                    long guessClientStartTimestampMs = m_GuessServerStartTimestamp - CommonDefinitions.UpdateDeltatime * CommonDefinitions.ServerDelayTick;

                    //有可能断线重连。
                    m_Simulator.GameStartTimestampMs = LMath.Min(guessClientStartTimestampMs, m_Simulator.GameStartTimestampMs);
                    //Log.Error($"Recalculate m_GameStartTimestampMs {m_Simulator.GameStartTimestampMs} m_GuessServerStartTimestamp:{m_GuessServerStartTimestamp}");
                }

                m_MinPing = Int64.MaxValue;
                m_MaxPing = Int64.MinValue;
            }
        }

        private void SendPing()
        {
            INetworkChannel tcpChannel = GameEntry.NetworkExtended.TcpChannel;
            if (tcpChannel == null)
            {
                Log.Error("Cannot SendPing, tcpChannel is null.");
                return;
            }
            CSPing csPing = ReferencePool.Acquire<CSPing>();
            csPing.LocalId = LocalId;
            csPing.SendTimestamp = GameTime.CurrTimeStamp;
            tcpChannel.Send(csPing);
        }

        public void SendInput(CSInputFrame csInputFrame)
        {
            m_Tick2SendTimestamp[csInputFrame.InputFrame.Tick] = GameTime.CurrTimeStamp;

#if DEBUG_SHOW_INPUT
            var cmd = input.Commands[0];
            var playerInput = new Deserializer(cmd.content).Parse<Lockstep.Game. PlayerInput>();
            if (playerInput.inputUV != LVector2.zero) {
                Debug.Log($"SendInput tick:{input.Tick} uv:{playerInput.inputUV}");
            }
#endif
            //_networkService.SendInput(input);
            INetworkChannel tcpChannel = GameEntry.NetworkExtended.TcpChannel;
            if (tcpChannel == null)
            {
                Log.Error("Cannot SendInput, tcpChannel is null.");
                return;
            }
            //发送Input
            tcpChannel.Send(csInputFrame);
        }

        public void SendReqMissFrame(int startTick)
        {
            INetworkChannel tcpChannel = GameEntry.NetworkExtended.TcpChannel;
            if (tcpChannel == null)
            {
                Log.Error("Cannot SendReqMissFrame, tcpChannel is null.");
                return;
            }
            CSReqMissFrame csReqMissFrame = ReferencePool.Acquire<CSReqMissFrame>();
            csReqMissFrame.StartTick = startTick;
            tcpChannel.Send(csReqMissFrame);
        }

        public ServerFrame GetFrame(int tick)
        {
            var sFrame = GetServerFrame(tick);
            if (sFrame != null)
            {
                return sFrame;
            }

            return GetLocalFrame(tick);
        }

        public ServerFrame GetServerFrame(int tick)
        {
            if (tick > MaxServerTickInBuffer)
            {
                return null;
            }

            return GetFrame(m_ServerBuffer, tick);
        }

        public ServerFrame GetLocalFrame(int tick)
        {
            if (tick >= m_NextClientTick)
            {
                return null;
            }

            return GetFrame(m_ClientBuffer, tick);
        }

        private ServerFrame GetFrame(ServerFrame[] buffer, int tick)
        {
            var idx = tick % m_BufferSize;
            var frame = buffer[idx];
            if (frame == null)
            {
                return null;
            }
            if (frame.Tick != tick)
            {
                return null;
            }
            return frame;
        }
    }
}
