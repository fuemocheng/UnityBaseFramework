using BaseFramework;
using BaseFramework.Runtime;
using GameProto;
using Network;

namespace Server
{
    public class Game
    {
        public Room Room;
        public int MapId;
       
        public int Tick = 0;
        public bool Paused = false;

        public EGameState GameState = EGameState.Default;

        /// <summary>
        /// 所有的历史帧。
        /// </summary>
        private List<ServerFrame> m_AllHistoryFrames;

        private long m_GameStartTimestampMs = -1;

        private float m_TimeSinceLoaded;
        private float m_FirstFrameTimeStamp = 0;

        private int m_TickSinceGameStart => (int)((GameTime.CurrTimeStamp - m_GameStartTimestampMs) / CommonDefinitions.UpdateDeltatime);

        private Dictionary<int, HashCodeChecker> m_HashCodeCheckers = new Dictionary<int, HashCodeChecker>();

        public Game(Room room)
        {
            Room = room;
            Tick = 0;
            Paused = false;
            m_AllHistoryFrames = new();
            GameState = EGameState.Default;
            m_GameStartTimestampMs = -1;
            m_TimeSinceLoaded = 0;
            m_FirstFrameTimeStamp = 0;
            m_HashCodeCheckers.Clear();
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

        public void Clear()
        {
            Tick = 0;
            Paused = false;
            m_AllHistoryFrames.Clear();
            GameState = EGameState.Default;
            m_GameStartTimestampMs = -1;
            m_TimeSinceLoaded = 0;
            m_FirstFrameTimeStamp = 0;
            m_HashCodeCheckers.Clear();
        }

        public long GameStartTimestampMs
        {
            get
            {
                return m_GameStartTimestampMs;
            }
        }

        public void SetLoading()
        {
            GameState = EGameState.Loading;
        }

        public void SetLoadingFinished()
        {
            GameState = EGameState.Loaded;
        }

        public void StopGame()
        {
            //不是默认状态，且不是停止状态，则停止游戏。
            if (GameState != EGameState.Default && GameState != EGameState.Finished)
            {
                OnFinished();
                GameState = EGameState.Finished;

                Log.Info($"StopGame RoomId:{Room.RoomId}");
            }
        }

        public void ReceiveInput(User user, CSInputFrame input)
        {
            //Log.Info("ReceiveInput CSInputFrame.Tick: {0}", input.InputFrame.Tick);

            if (GameState != EGameState.Loaded && GameState != EGameState.Playing)
            {
                return;
            }

            // 收到第一个输入，即开始。
            if (GameState == EGameState.Loaded)
            {
                GameState = EGameState.Playing;
                Log.Info($"StartGame RoomId:{Room.RoomId}");
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
            //Log.Info("BroadcastServerFrame ServerFrame.Tick: {0}", Tick);
            BroadcastServerFrame(frames);


            if (m_FirstFrameTimeStamp <= 0)
            {
                m_FirstFrameTimeStamp = m_TimeSinceLoaded;
            }

            // 所有人的第一帧都收到了，进行广播时，才开始赋值。
            if (m_GameStartTimestampMs < 0)
            {
                m_GameStartTimestampMs = GameTime.CurrTimeStamp + CommonDefinitions.UpdateDeltatime * CommonDefinitions.ServerDelayTick;
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
                if (sUser == null || sUser.TcpSession == null)
                {
                    continue;
                }
                SCServerFrame scServerFrame = ReferencePool.Acquire<SCServerFrame>();
                scServerFrame.StartTick = serverFrames[0].Tick;
                scServerFrame.ServerFrames.AddRange(serverFrames);
                sUser.TcpSession?.Send(scServerFrame);
            }
        }

        public void OnReqMissFrame(Session session, int startTick)
        {
            if (session == null)
            {
                return;
            }

            //Log.Info($"OnReqMissFrame : {startTick}");

            int count = Math.Min((Math.Min((Tick - 1), m_AllHistoryFrames.Count) - startTick), CommonDefinitions.MaxRepMissFrameCountPerPack);
            if (count <= 0)
            {
                return;
            }

            SCReqMissFrame scReqMissFrame = ReferencePool.Acquire<SCReqMissFrame>();
            scReqMissFrame.StartTick = startTick;

            var frames = new ServerFrame[count];
            for (int i = 0; i < count; i++)
            {
                frames[i] = m_AllHistoryFrames[startTick + i];
                if (frames[i] == null)
                {
                    throw new Exception($"HistoryFrames[i] is null.");
                }
            }
            scReqMissFrame.ServerFrames.AddRange(frames);
            session?.Send(scReqMissFrame);
        }

        public void OnCheckHashCode(User user, CSHashCode csHashCode)
        {
            if (user == null)
            {
                return;
            }
            var localId = user.LocalId;
            for (int i = 0; i < csHashCode.HashCodes.Count; i++)
            {
                int tick = csHashCode.StartTick + i;
                int code = csHashCode.HashCodes[i];

                //Log.Info($"OnHashCode LocalId:{localId} Tick: {tick} HashCode {code}");

                if (m_HashCodeCheckers.TryGetValue(tick, out HashCodeChecker matcher))
                {
                    // 匹配器已经为空，说明匹配之前成功了，或者已经收到过此玩家此帧的数据。
                    if (matcher == null || matcher.ReceivedResult[localId])
                    {
                        continue;
                    }

                    // 出现了HashCode不匹配的情况。
                    if (matcher.HashCode != code)
                    {
                        OnCheckHashCodeResult(user, tick, code, false);
                    }

                    matcher.CheckedCount = matcher.CheckedCount + 1;
                    matcher.ReceivedResult[localId] = true;
                    if (matcher.IsMatched)
                    {
                        OnCheckHashCodeResult(user, tick, code, true);
                    }
                }
                else
                {
                    var newMatcher = ReferencePool.Acquire<HashCodeChecker>();
                    newMatcher.HashCode = code;
                    newMatcher.CheckedCount = 1;
                    newMatcher.ReceivedResult[localId] = true;
                    m_HashCodeCheckers.Add(tick, newMatcher);
                    if (newMatcher.IsMatched)
                    {
                        OnCheckHashCodeResult(user, tick, code, true);
                    }
                }
            }
        }

        private void OnCheckHashCodeResult(User user, int tick, long hashCode, bool isMatched)
        {
            // 此帧全部校验成功，则将数据置空，缓解内存压力。
            if (isMatched)
            {
                ReferencePool.Release(m_HashCodeCheckers[tick]);
                m_HashCodeCheckers[tick] = null;
            }

            // 数据不匹配，打印报错。
            if (!isMatched)
            {
                Log.Error($"Error: HashCode is not matched! User:{user.UserName} Tick: {tick} HashCode: {hashCode}");

                SCHashCode scHashCode = ReferencePool.Acquire<SCHashCode>();
                scHashCode.RetCode = (int)EErrorCode.HashCodeMismatch;
                scHashCode.MismatchedTick = tick;
                user?.TcpSession?.Send(scHashCode);
            }
        }

        private void OnFinished()
        {
            //记录对局信息。
            DumpGameFrames();

            Clear();
        }

        private void DumpGameFrames()
        {
            int count = Math.Min((Tick - 1), m_AllHistoryFrames.Count);
            if (count <= 0)
            {
                return;
            }

            SCGameStartInfo gameStartInfo = new();
            gameStartInfo.RoomId = Room.RoomId;
            gameStartInfo.MapId = 1;
            gameStartInfo.LocalId = -1; //存储的所有人的信息
            gameStartInfo.UserCount = Room.GetCurrCount();
            gameStartInfo.Seed = 0;
            // 遍历添加所有人信息，添加到 SCGameStartInfo
            foreach (KeyValuePair<long, User> kvp in Room.GetUsersDictionary())
            {
                UserGameInfo userGameInfo = new UserGameInfo();
                userGameInfo.LocalId = kvp.Value.LocalId;
                userGameInfo.UserState = (int)kvp.Value.UserState;
                userGameInfo.Camp = (int)kvp.Value.Camp;
                userGameInfo.User = new GameProto.User();
                userGameInfo.User.UserId = kvp.Value.UserId;
                userGameInfo.User.UserName = kvp.Value.UserName;

                gameStartInfo.UserGameInfos.Add(userGameInfo);
            }

            SCServerFrame scServerFrame = new();
            for (int i = 0; i < count; i++)
            {
                scServerFrame.ServerFrames.Add(m_AllHistoryFrames[i]);
                if (m_AllHistoryFrames[i] == null)
                {
                    Log.Error($"DumpGameFrames Error: Frame[{i}] is null.");
                }
            }
            scServerFrame.StartTick = m_AllHistoryFrames[0].Tick;

            string recordPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                $"../Record/{DateTime.Now.ToString("yyyyMMddHHmmss")}_{Room.RoomId}_{MapId}.bytes");

            RecordUtility.WriteRecord(recordPath, gameStartInfo, scServerFrame);
        }
    }
}
