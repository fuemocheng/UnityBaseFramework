using BaseFramework;
using BaseFramework.Runtime;
using GameProto;
using Lockstep.Util;

namespace Server
{
    public class Game
    {
        public Room Room;
        public int MapId;
        public EGameState GameState;

        public int Tick = 0;

        /// <summary>
        /// 所有的历史帧。
        /// </summary>
        private List<ServerFrame> m_AllHistoryFrames;

        private long m_GameStartTimestampMs = -1;
        private int m_ServerTickDealy = 10;
        private int m_TickSinceGameStart => (int)((LTime.realtimeSinceStartupMS - m_GameStartTimestampMs) / NetworkDefine.UPDATE_DELTATIME);

        private float m_TimeSinceLoaded;
        private float m_FirstFrameTimeStamp = 0;

        public Game(Room room)
        {
            Room = room;
            Tick = 0;
            m_AllHistoryFrames = new();
            GameState = EGameState.Idle;
            m_GameStartTimestampMs = -1;
        }


        public void Clear()
        {
            Tick = 0;
            m_AllHistoryFrames.Clear();
            GameState = EGameState.Idle;
            m_GameStartTimestampMs = -1;
        }

        public void SetLoading()
        {
            GameState = EGameState.Loading;
        }

        public void SetLoadingFinished()
        {
            GameState = EGameState.PartLoaded;
        }

        public void Update(double elapseSeconds, double realElapseSeconds)
        {
            m_TimeSinceLoaded += (float)elapseSeconds;

            if (GameState != EGameState.Playing)
            {
                return;
            }
            if (m_GameStartTimestampMs <= 0)
            {
                return;
            }
            while (Tick < m_TickSinceGameStart)
            {
                CheckBroadcastServerFrame(true);
            }
        }

        public void ReceiveInput(User user, CSInputFrame input)
        {
            Log.Info("ReceiveInput CSInputFrame.Tick: {0}", input.InputFrame.Tick);

            if (GameState != EGameState.PartLoaded &&
                GameState != EGameState.Playing)
            {
                return;
            }

            // 收到第一个输入，即开始。
            if (GameState == EGameState.PartLoaded)
            {
                GameState = EGameState.Playing;
            }

            // 这条帧数据的帧落后于服务器的帧。
            if (input.InputFrame.Tick < Tick)
            {
                return;
            }

            ServerFrame frame = GetOrCreateFrame(Tick);

            int localId = input.InputFrame.LocalId;
            frame.InputFrames[localId] = input.InputFrame;

            // 不强制广播，等所有人数据都到了再广播。
            CheckBroadcastServerFrame(false);
        }


        private ServerFrame GetOrCreateFrame(int tick)
        {
            // 扩充帧列表。
            int frameCount = m_AllHistoryFrames.Count;
            if (frameCount <= tick)
            {
                int count = tick - m_AllHistoryFrames.Count + 1;
                for (int i = 0; i < count; i++)
                {
                    m_AllHistoryFrames.Add(null);
                }
            }

            if (m_AllHistoryFrames[tick] == null)
            {
                ServerFrame serverFrame = new ServerFrame();
                serverFrame.Tick = tick;
                m_AllHistoryFrames[tick] = serverFrame;
            }

            ServerFrame frame = m_AllHistoryFrames[tick];
            if (frame.InputFrames.Count == 0)
            {
                for (int i = 0; i < CommonDefinitions.MaxRoomMemberCount; i++)
                {
                    // 填充空数据。
                    frame.InputFrames.Add(null);
                }
            }

            return frame;
        }

        /// <summary>
        /// 检查是否广播服务器帧数据。
        /// </summary>
        /// <param name="isForce">是否强制广播。</param>
        /// <returns></returns>
        private bool CheckBroadcastServerFrame(bool isForce = false)
        {
            if (GameState != EGameState.Playing)
            {
                return false;
            }

            ServerFrame frame = GetOrCreateFrame(Tick);
            List<InputFrame> inputs = frame.InputFrames;

            if (!isForce)
            {
                // 非强制广播，则等待其他玩家数据。
                for (int i = 0; i < inputs.Count; i++)
                {
                    if (inputs[i] == null)
                    {
                        return false;
                    }
                }
            }

            // 准备广播。
            // 将所有未到的包，设置 Miss。
            for (int i = 0; i < inputs.Count; i++)
            {
                if (inputs[i] == null)
                {
                    InputFrame tInput = new InputFrame();
                    tInput.Tick = Tick;
                    tInput.LocalId = i;
                    tInput.IsMiss = true;
                    inputs[i] = tInput;
                }
            }

            // 要发送的过往帧的数量。最多3帧。
            int count = Tick < 2 ? Tick + 1 : 3;
            ServerFrame[] frames = new ServerFrame[count];
            for (int i = 0; i < count; i++)
            {
                frames[count - i - 1] = m_AllHistoryFrames[Tick - i];
            }
            Log.Info("BroadcastServerFrame ServerFrame.Tick: {0}", Tick);
            BroadcastServerFrame(frames);


            if (m_FirstFrameTimeStamp <= 0)
            {
                m_FirstFrameTimeStamp = m_TimeSinceLoaded;
            }

            // 所有人的第一帧都收到了，进行广播时，才开始赋值。
            if (m_GameStartTimestampMs < 0)
            {
                m_GameStartTimestampMs = LTime.realtimeSinceStartupMS + NetworkDefine.UPDATE_DELTATIME * m_ServerTickDealy;
            }

            Tick++;
            return true;
        }


        private void BroadcastServerFrame(ServerFrame[] serverFrames)
        {
            // 广播进度。
            foreach (KeyValuePair<long, User> kvp in Room.GetUsersDictionary())
            {
                User sUser = kvp.Value;
                SCServerFrame scServerFrame = ReferencePool.Acquire<SCServerFrame>();
                scServerFrame.StartTick = serverFrames[0].Tick;
                scServerFrame.ServerFrames.AddRange(serverFrames);
                sUser.TcpSession.Send(scServerFrame);
            }
        }
    }
}
