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
        private int _maxClientPredictFrameCount;
        private int _bufferSize;
        private int _spaceRollbackNeed;
        private int _maxServerOverFrameCount;

        private ServerFrame[] _serverBuffer;
        private ServerFrame[] _clientBuffer;

        //ping 
        public int PingVal { get; private set; }
        private List<long> _pings = new List<long>();
        private long _guessServerStartTimestamp = Int64.MaxValue;
        private long _historyMinPing = Int64.MaxValue;
        private long _minPing = Int64.MaxValue;
        private long _maxPing = Int64.MinValue;
        public int DelayVal { get; private set; }
        private float _pingTimer;
        private List<long> _delays = new List<long>();
        Dictionary<int, long> _tick2SendTimestamp = new Dictionary<int, long>();

        // the tick client need run in next update
        private int _nextClientTick;

        public int CurTickInServer { get; private set; }
        public int NextTickToCheck { get; private set; }
        public int MaxServerTickInBuffer { get; private set; } = -1;
        public bool IsNeedRollback { get; private set; }
        public int MaxContinueServerTick { get; private set; }

        public byte LocalId;

        public FrameBuffer(Simulator simulator, int bufferSize, int snapshotFrameInterval, int maxClientPredictFrameCount)
        {
            m_Simulator = simulator;
            m_PredictCountHelper = new PredictCountHelper(m_Simulator, this);

            _bufferSize = bufferSize;
            _maxClientPredictFrameCount = maxClientPredictFrameCount;
            _spaceRollbackNeed = snapshotFrameInterval * 2;
            _maxServerOverFrameCount = bufferSize - _spaceRollbackNeed;
            _serverBuffer = new ServerFrame[bufferSize];
            _clientBuffer = new ServerFrame[bufferSize];
        }

        public void SetClientTick(int tick)
        {
            _nextClientTick = tick + 1;
        }

        public void PushLocalFrame(ServerFrame frame)
        {
            var sIdx = frame.Tick % _bufferSize;
            //if (_clientBuffer[sIdx] == null || _clientBuffer[sIdx].Tick <= frame.Tick)
            //{
            //    Log.Error("Push local frame error!");
            //}
            _clientBuffer[sIdx] = frame;
        }

        public void OnPlayerPing(SCPing scPing)
        {
            //PushServerFrames(frames, isNeedDebugCheck);
            var ping = LTime.realtimeSinceStartupMS - scPing.SendTimestamp;
            _pings.Add(ping);
            if (ping > _maxPing) _maxPing = ping;
            if (ping < _minPing)
            {
                _minPing = ping;
                _guessServerStartTimestamp = (LTime.realtimeSinceStartupMS - scPing.TimeSinceServerStart) - _minPing / 2;
            }
        }

        public void PushServerFrames(ServerFrame[] frames, bool isNeedDebugCheck = true)
        {
            int lastTick = frames[frames.Length - 1].Tick;
            if (lastTick == 1 || lastTick == 2 || lastTick == 3)
            {
                int t = 1;
            }

            var count = frames.Length;
            for (int i = 0; i < count; i++)
            {
                var data = frames[i];

                if (_tick2SendTimestamp.TryGetValue(data.Tick, out var sendTick))
                {
                    var delay = LTime.realtimeSinceStartupMS - sendTick;
                    _delays.Add(delay);
                    _tick2SendTimestamp.Remove(data.Tick);
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

                if (data.Tick >= NextTickToCheck + _maxServerOverFrameCount - 1)
                {
                    //to avoid ringBuffer override the frame that have not been checked
                    continue;
                }

                //Debug.Log("PushServerFramesSucc" + data.tick);
                if (data.Tick > MaxServerTickInBuffer)
                {
                    MaxServerTickInBuffer = data.Tick;
                }

                var targetIdx = data.Tick % _bufferSize;
                if (_serverBuffer[targetIdx] == null || _serverBuffer[targetIdx].Tick != data.Tick)
                {
                    _serverBuffer[targetIdx] = data;
                    Log.Error("PushServerFrames:" + targetIdx);
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
            //_networkService.SendPing(_simulatorService.LocalActorId, LTime.realtimeSinceStartupMS);
            m_PredictCountHelper.Update(deltaTime);
            int worldTick = m_Simulator.World.Tick;
            UpdatePingVal(deltaTime);

            //Debug.Assert(nextTickToCheck <= nextClientTick, "localServerTick <= localClientTick ");
            //Confirm frames
            IsNeedRollback = false;
            while (NextTickToCheck <= MaxServerTickInBuffer && NextTickToCheck < worldTick)
            {
                var sIdx = NextTickToCheck % _bufferSize;
                var cFrame = _clientBuffer[sIdx];
                var sFrame = _serverBuffer[sIdx];
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
                var idx = tick % _bufferSize;
                if (_serverBuffer[idx] == null || _serverBuffer[idx].Tick != tick)
                {
                    break;
                }
            }

            MaxContinueServerTick = tick - 1;
            if (MaxContinueServerTick <= 0) return;
            if (MaxContinueServerTick < CurTickInServer // has some middle frame pack was lost
                || _nextClientTick >
                MaxContinueServerTick + (_maxClientPredictFrameCount - 3) //client has predict too much
            )
            {
                Log.Info("SendMissFrameReq " + MaxContinueServerTick);
                //_networkService.SendMissFrameReq(MaxContinueServerTick);
            }
        }

        private void UpdatePingVal(float deltaTime)
        {
            _pingTimer += deltaTime;
            if (_pingTimer > 0.5f)
            {
                _pingTimer = 0;
                DelayVal = (int)(_delays.Sum() / LMath.Max(_delays.Count, 1));
                _delays.Clear();
                PingVal = (int)(_pings.Sum() / LMath.Max(_pings.Count, 1));
                _pings.Clear();

                if (_minPing < _historyMinPing && m_Simulator.GameStartTimestampMs != -1)
                {
                    _historyMinPing = _minPing;
#if UNITY_EDITOR
                    Log.Warning(
                        $"Recalc _gameStartTimestampMs {m_Simulator.GameStartTimestampMs} _guessServerStartTimestamp:{_guessServerStartTimestamp}");
#endif
                    m_Simulator.GameStartTimestampMs = LMath.Min(_guessServerStartTimestamp,
                        m_Simulator.GameStartTimestampMs);
                }

                _minPing = Int64.MaxValue;
                _maxPing = Int64.MinValue;
            }
        }

        public void SendInput(CSInputFrame csInputFrame)
        {
            _tick2SendTimestamp[csInputFrame.InputFrame.Tick] = LTime.realtimeSinceStartupMS;
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

            return _GetFrame(_serverBuffer, tick);
        }

        public ServerFrame GetLocalFrame(int tick)
        {
            if (tick >= _nextClientTick)
            {
                return null;
            }

            return _GetFrame(_clientBuffer, tick);
        }

        private ServerFrame _GetFrame(ServerFrame[] buffer, int tick)
        {
            var idx = tick % _bufferSize;
            var frame = buffer[idx];
            if (frame == null) return null;
            if (frame.Tick != tick) return null;
            return frame;
        }
    }
}
