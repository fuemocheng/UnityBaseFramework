using BaseFramework;
using GameProto;
using Lockstep.Game;
using System.Collections.Generic;
using UnityBaseFramework.Runtime;
using UnityEngine;

namespace XGame
{
    public class Simulator
    {
        public static Simulator Instance
        {
            get;
            private set;
        }

        public Simulator()
        {
            Instance = this;
        }

        public const long MinMissFrameReqTickDiff = 10;
        public const long MaxSimulationMsPerFrame = 20;
        public const int MaxPredictFrameCount = 30;

        public World World { get; private set; }

        private FrameBuffer m_FrameBuffer;
        private HashHelper m_HashHelper;
        private DumpHelper m_DumpHelper;

        private int m_MapId = 0;
        private int m_LocalId = -1;
        private List<UserGameInfo> m_UserGameInfos = new List<UserGameInfo>();

        public GameProto.Input[] PlayerInputs => World.PlayerInputs;

        public int PingVal => m_FrameBuffer?.PingVal ?? 0;
        public int DelayVal => m_FrameBuffer?.DelayVal ?? 0;

        public bool IsRunning { get; set; }

        public long GameStartTimestampMs = -1;      //游戏开始时间戳。
        private int m_TickSinceGameStart = 0;       //从游戏开始运行的逻辑帧数。

        private int m_InputTick = 0;

        // frame count that need predict(TODO should change according current network's delay).
        public int FramePredictCount = 0;
        // 预发送输入帧数量。
        public int PreSendInputCount = 1;

        public int TargetTick => m_TickSinceGameStart + FramePredictCount;
        public int InputTargetTick => m_TickSinceGameStart + PreSendInputCount;


        private int m_SnapshotFrameInterval = 1; //快照间隔。
        private bool m_HasRecvInputMsg = false; //是否收到输入帧消息。

        #region VideoMode
        public bool IsVideoMode = false;
        private SCServerFrame m_VideoFrames;
        private bool m_IsInitVideo = false;
        private int m_TickOnLastJumpTo = 0;
        private long m_TimestampOnLastJumpToMs = 0;
        #endregion

        #region ClientSimulateMode
        public bool IsClientMode = false;
        private bool m_IsDebugRollBack = false;
        #endregion

        public void Start()
        {
            Instance = this;

            World = new World();
            m_FrameBuffer = new FrameBuffer(this, 2000, m_SnapshotFrameInterval, MaxPredictFrameCount);
            m_HashHelper = new HashHelper(World, m_FrameBuffer);
            m_DumpHelper = new DumpHelper(World, m_HashHelper);
            m_DumpHelper.Enable = true;

            IsRunning = false;
            m_HasRecvInputMsg = false;
            m_SnapshotFrameInterval = 1;
        }

        public void Destroy()
        {
            Instance = null;

            IsRunning = false;
            m_HasRecvInputMsg = false;
            m_SnapshotFrameInterval = 1;

            m_DumpHelper.DumpAll();
            m_DumpHelper.Clear();
            m_DumpHelper = null;
            m_HashHelper.Clear();
            m_HashHelper = null;
            m_FrameBuffer.Clear();
            m_FrameBuffer = null;
            World.OnGameDestroy();
            World = null;

            m_VideoFrames?.ServerFrames?.Clear();
            m_VideoFrames = null;
            m_IsInitVideo = false;

            GameEntry.Service.RemoveAllServices();
        }

        public void OnGameCreate(int targetFps, int mapId, int localId, int actorCount, List<UserGameInfo> userGameInfos)
        {
            Log.Info($"OnGameCreate:{localId}");
            m_MapId = mapId;
            m_LocalId = localId;
            m_UserGameInfos.AddRange(userGameInfos);
            m_FrameBuffer.LocalId = localId;

            // Service 创建。
            GameEntry.Service.RegisterService(new CommonStateService());
            GameEntry.Service.RegisterService(new ConstStateService());
            GameEntry.Service.RegisterService(new GameStateService());
            GameEntry.Service.RegisterService(new IdService());
            GameEntry.Service.RegisterService(new TimeMachineService());
            GameEntry.Service.RegisterService(new GameViewService());
            GameEntry.Service.RegisterService(new GameInputService());

            var svcs = GameEntry.Service.GetAllServices();
            foreach (var service in svcs)
            {
                GameEntry.Service.GetService<TimeMachineService>().RegisterTimeMachine(service as ITimeMachine);
            }

            GameEntry.GameLogic.gameObject.GetOrAddComponent<PingMono>();
            GameEntry.GameLogic.gameObject.GetOrAddComponent<InputMono>();

#if UNITY_EDITOR
            GameEntry.GameLogic.gameObject.GetOrAddComponent<TestMono>();
#endif

            World.OnGameCreate();
        }


        public void StartSimulate()
        {
            if (IsRunning)
            {
                Log.Error("Already started.");
                return;
            }
            IsRunning = true;

            World.StartSimulate(m_UserGameInfos, m_LocalId);

            // 发送Input
            while (m_InputTick < PreSendInputCount)
            {
                SendInputs(m_InputTick++);
            }
        }

        /// <summary>
        /// 更新。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            // 第一帧未开始开始。
            if (!IsRunning)
            {
                return;
            }

            // 收到第一帧的消息，记录时间。
            if (m_HasRecvInputMsg)
            {
                if (GameStartTimestampMs == -1)
                {
                    GameStartTimestampMs = GameTime.CurrTimeStamp;
                }
            }

            // 未收到第一帧的消息，则不继续。
            if (GameStartTimestampMs <= 0)
            {
                return;
            }

            if (GameEntry.Service.GetService<ConstStateService>().IsVideoMode)
            {
                return;
            }

            if (GameEntry.Service.GetService<CommonStateService>().IsPause)
            {
                return;
            }

            m_TickSinceGameStart = (int)((GameTime.CurrTimeStamp - GameStartTimestampMs) / CommonDefinitions.UpdateDeltatime);

            m_FrameBuffer.Update(elapseSeconds);


            if (IsClientMode)
            {
                //单机模式更新。
                DoClientUpdate();
            }
            else
            {
                //多人模式更新。
                while (m_InputTick <= InputTargetTick)
                {
                    SendInputs(m_InputTick++);
                }

                DoNormalUpdate();
            }
        }

        private void DoClientUpdate()
        {
            int maxRollbackCount = 5;
            if (m_IsDebugRollBack && World.Tick > maxRollbackCount && World.Tick % maxRollbackCount == 0)
            {
                int rawTick = World.Tick;
                int revertCount = Utility.Random.GetRandom(1, maxRollbackCount);
                for (int i = 0; i < revertCount; i++)
                {
                    InputFrame inputFrame = new();
                    inputFrame.Tick = World.Tick;
                    inputFrame.LocalId = 0;
                    inputFrame.Input = new();
                    inputFrame.Input.InputH = Utility.Random.GetRandom(-1, 1);
                    inputFrame.Input.InputV = Utility.Random.GetRandom(-1, 1);

                    ServerFrame serverFrame = new();
                    serverFrame.Tick = rawTick - i;
                    serverFrame.InputFrames.Add(inputFrame);

                    m_FrameBuffer.ForcePushDebugFrame(serverFrame);
                }

                Log.Info("RollbackTo:{0}", World.Tick - revertCount);

                if (!RollbackTo(World.Tick - revertCount, World.Tick))
                {
                    GameEntry.Service.GetService<CommonStateService>().IsPause = true;
                    return;
                }

                while (World.Tick < rawTick)
                {
                    ServerFrame sFrame = m_FrameBuffer.GetServerFrame(World.Tick);
                    if (sFrame == null || sFrame.Tick != World.Tick)
                    {
                        Log.Error("DoClientUpdate ServerFrame is Error.");
                        return;
                    }
                    m_FrameBuffer.PushLocalFrame(sFrame);
                    Simulate(sFrame);
                    if (GameEntry.Service.GetService<CommonStateService>().IsPause)
                    {
                        return;
                    }
                }
            }

            while (World.Tick < TargetTick)
            {
                FramePredictCount = 0;

                GameProto.Input input = GameEntry.Service.GetService<GameInputService>().CurrInput;

                InputFrame inputFrame = new();
                inputFrame.Tick = World.Tick;
                inputFrame.LocalId = 0;
                inputFrame.Input = input;

                ServerFrame serverFrame = new();
                serverFrame.Tick = World.Tick;
                serverFrame.InputFrames.Add(inputFrame);

                m_FrameBuffer.PushLocalFrame(serverFrame);
                m_FrameBuffer.PushServerFrames(new ServerFrame[] { serverFrame });
                Simulate(m_FrameBuffer.GetFrame(World.Tick));
                if (GameEntry.Service.GetService<CommonStateService>().IsPause)
                {
                    return;
                }
            }
        }

        private void DoNormalUpdate()
        {
            //make sure client is not move ahead too much than server.
            var maxContinueServerTick = m_FrameBuffer.MaxContinueServerTick;
            if ((World.Tick - maxContinueServerTick) > MaxPredictFrameCount)
            {
                return;
            }

            var minTickToBackup = (maxContinueServerTick - (maxContinueServerTick % m_SnapshotFrameInterval));

            // Pursue Server frames
            //var deadline = LTime.realtimeSinceStartupMS + MaxSimulationMsPerFrame;
            var deadline = GameTime.CurrTimeStamp + MaxSimulationMsPerFrame;
            while (World.Tick < m_FrameBuffer.CurTickInServer)
            {
                var tick = World.Tick;
                var sFrame = m_FrameBuffer.GetServerFrame(tick);
                if (sFrame == null)
                {
                    OnPursuingFrame();
                    return;
                }

                m_FrameBuffer.PushLocalFrame(sFrame);
                Simulate(sFrame, tick == minTickToBackup);
                //if (LTime.realtimeSinceStartupMS > deadline)
                if (GameTime.CurrTimeStamp > deadline)
                {
                    OnPursuingFrame();
                    return;
                }
            }

            if (GameEntry.Service.GetService<ConstStateService>().IsPursueFrame)
            {
                // 追帧结束。
                GameEntry.Service.GetService<ConstStateService>().IsPursueFrame = false;
            }


            // Roll back
            if (m_FrameBuffer.IsNeedRollback)
            {
                RollbackTo(m_FrameBuffer.NextTickToCheck, maxContinueServerTick);
                CleanUselessSnapshot(System.Math.Min(m_FrameBuffer.NextTickToCheck - 1, World.Tick));

                minTickToBackup = System.Math.Max(minTickToBackup, World.Tick + 1);
                while (World.Tick <= maxContinueServerTick)
                {
                    var sFrame = m_FrameBuffer.GetServerFrame(World.Tick);
                    if (sFrame == null && sFrame.Tick != World.Tick)
                    {
                        Log.Error($" logic error: server Frame  must exist tick {World.Tick}");
                    }
                    m_FrameBuffer.PushLocalFrame(sFrame);
                    Simulate(sFrame, World.Tick == minTickToBackup);
                }
            }


            //Run frames
            while (World.Tick <= TargetTick)
            {
                var curTick = World.Tick;
                ServerFrame frame = null;
                var sFrame = m_FrameBuffer.GetServerFrame(curTick);
                if (sFrame != null)
                {
                    frame = sFrame;
                }
                else
                {
                    var cFrame = m_FrameBuffer.GetLocalFrame(curTick);
                    FillInputWithLastFrame(cFrame);
                    frame = cFrame;
                }

                m_FrameBuffer.PushLocalFrame(frame);
                Predict(frame, true);
            }

            m_HashHelper.CheckAndSendHashCodes();

        }

        private void SendInputs(int curTick)
        {
            GameProto.Input input = GameEntry.Service.GetService<GameInputService>().CurrInput;
            InputFrame inputFrame = new();
            inputFrame.Tick = curTick;
            inputFrame.LocalId = m_LocalId;
            inputFrame.Input = input;
            CSInputFrame csInputFrame = new();
            csInputFrame.InputFrame = inputFrame;

            ServerFrame cFrame = new ServerFrame();
            var inputFrames = new InputFrame[m_UserGameInfos.Count];
            inputFrames[m_LocalId] = inputFrame;
            cFrame.InputFrames.AddRange(inputFrames);
            cFrame.Tick = curTick;
            FillInputWithLastFrame(cFrame);
            m_FrameBuffer.PushLocalFrame(cFrame);
            //if (input.Commands != null) {
            //    var playerInput = new Deserializer(input.Commands[0].content).Parse<Lockstep.Game.PlayerInput>();
            //    Debug.Log($"SendInput curTick{curTick} maxSvrTick{_cmdBuffer.MaxServerTickInBuffer} _tickSinceGameStart {_tickSinceGameStart} uv {playerInput.inputUV}");
            //}
            if (curTick > m_FrameBuffer.MaxServerTickInBuffer)
            {
                //TODO combine all history inputs into one Msg 
                //Debug.Log("SendInput " + curTick +" _tickSinceGameStart " + _tickSinceGameStart);
                m_FrameBuffer.SendInput(csInputFrame);
            }
        }

        /// <summary>
        /// 模拟帧。
        /// </summary>
        /// <param name="serverFrame"></param>
        /// <param name="isNeedGenSnap"></param>
        private void Simulate(ServerFrame serverFrame, bool isNeedGenSnap = true)
        {
            Step(serverFrame, isNeedGenSnap);
        }

        /// <summary>
        /// 预测帧。
        /// </summary>
        /// <param name="serverFrame"></param>
        /// <param name="isNeedGenSnap"></param>
        private void Predict(ServerFrame serverFrame, bool isNeedGenSnap = true)
        {
            Step(serverFrame, isNeedGenSnap);
        }

        private bool RollbackTo(int tick, int maxContinueServerTick, bool isNeedClear = true)
        {
            World.RollbackTo(tick, maxContinueServerTick, isNeedClear);

            var hash = GameEntry.Service.GetService<CommonStateService>().Hash;
            var curHash = m_HashHelper.CalculateHash();
            if (hash != curHash)
            {
                Log.Error($"Tick [{tick}] Rollback Error: Hash is diff. OldHash:{hash}  CurHash:{curHash}");
#if UNITY_EDITOR
                m_DumpHelper.DumpToFile(true);
                return false;
#endif
            }
            return true;
        }


        /// <summary>
        /// 帧步进。
        /// </summary>
        /// <param name="serverFrame"></param>
        /// <param name="isNeedGenSnap"></param>
        private void Step(ServerFrame serverFrame, bool isNeedGenSnap = true)
        {
            GameEntry.Service.GetService<CommonStateService>().SetTick(World.Tick);

            var hash = m_HashHelper.CalculateHash();
            GameEntry.Service.GetService<CommonStateService>().Hash = hash;

            // 备份当前帧。
            GameEntry.Service.GetService<TimeMachineService>().Backup(World.Tick);

            DumpFrameBegin();

            hash = m_HashHelper.CalculateHash(true);
            m_HashHelper.SetHash(World.Tick, hash);
            ProcessInputQueue(serverFrame);

            World.Step();

            DumpFrameEnd();

            // 记录当前 Tick。
            int tick = World.Tick;
            m_FrameBuffer.SetClientTick(tick);
        }

        private void DumpFrameBegin()
        {
            m_DumpHelper.DumpFrame(true);
        }

        private void DumpFrameEnd()
        {
            m_DumpHelper.OnFrameEnd();
        }

        private void CleanUselessSnapshot(int tick)
        {
            //TODO
        }

        private void FillInputWithLastFrame(ServerFrame frame)
        {
            int tick = frame.Tick;
            var inputs = frame.InputFrames;
            var lastServerInputs = tick == 0 ? null : m_FrameBuffer.GetFrame(tick - 1)?.InputFrames;
            var myInput = inputs[m_LocalId];
            //fill inputs with last frame's input (Input predict)
            for (int i = 0; i < m_UserGameInfos.Count; i++)
            {
                inputs[i] = new InputFrame();
                inputs[i].Tick = tick;
                if (lastServerInputs != null && lastServerInputs[i] != null)
                {
                    inputs[i].LocalId = lastServerInputs[i].LocalId;
                    inputs[i].Input = lastServerInputs[i].Input;
                }
                else
                {
                    inputs[i].LocalId = i;
                    inputs[i].Input = new GameProto.Input();
                }
            }

            inputs[m_LocalId] = myInput;
        }

        private void ProcessInputQueue(ServerFrame sframe)
        {
            var inputFrames = sframe.InputFrames;
            foreach (var playerInput in PlayerInputs)
            {
                playerInput.InputV = 0;
                playerInput.InputH = 0;
                playerInput.SkillId = 0;
            }

            foreach (var inputFrame in inputFrames)
            {
                if (inputFrame.Input == null) continue;
                if (inputFrame.LocalId >= PlayerInputs.Length) continue;
                var inputEntity = PlayerInputs[inputFrame.LocalId];
                //foreach (var command in inputFrame.Commands)
                //{
                //    Log.Info(inputFrame.ActorId + " >> " + inputFrame.Tick + ": " + inputFrame);
                //    _inputService.Execute(command, inputEntity);
                //}
                inputEntity.InputH = inputFrame.Input.InputH;
                inputEntity.InputV = inputFrame.Input.InputV;
            }
        }

        private void OnPursuingFrame()
        {
            GameEntry.Service.GetService<ConstStateService>().IsPursueFrame = true;

            Log.Info($"PurchaseServering curTick:" + World.Tick);
            var progress = World.Tick * 1.0f / m_FrameBuffer.CurTickInServer;
            //EventHelper.Trigger(EEvent.PursueFrameProcess, progress);
        }

        public void OnPing(SCPingEventArgs scPingEventArgs)
        {
            m_FrameBuffer?.OnPing(scPingEventArgs);
        }

        public void OnServerFrame(List<ServerFrame> serverFrames)
        {
            m_HasRecvInputMsg = true;
            m_FrameBuffer?.PushServerFrames(serverFrames.ToArray());
        }

        public void OnReqMissFrame(List<ServerFrame> serverFrames)
        {
            m_FrameBuffer?.PushMissServerFrames(serverFrames.ToArray());
        }

        public void SendReqMissFrame(int startTick)
        {
            m_FrameBuffer?.SendReqMissFrame(startTick);
        }

        #region VideoMode

        public void SetVideoFrame(SCServerFrame frames)
        {
            m_VideoFrames = frames;
        }

        public void JumpTo(int tick)
        {
            if (m_VideoFrames == null)
            {
                return;
            }

            if (tick < 0)
            {
                return;
            }

            if (tick + 1 == World.Tick || tick == World.Tick)
            {
                return;
            }

            tick = System.Math.Min(tick, m_VideoFrames.ServerFrames.Count);

            float time = GameTime.CurrTimeStamp + 0.05f;
            //把所有帧运行一遍
            if (!m_IsInitVideo)
            {
                GameEntry.Service.GetService<ConstStateService>().IsVideoLoading = true;
                while (World.Tick < m_VideoFrames.ServerFrames.Count)
                {
                    ServerFrame sFrame = m_VideoFrames.ServerFrames[World.Tick];
                    Simulate(sFrame, true);
                    if (GameTime.CurrTimeStamp > time)
                    {
                        //每帧最多执行的时间。
                        return;
                    }
                }
                GameEntry.Service.GetService<ConstStateService>().IsVideoLoading = false;
                m_IsInitVideo = true;
            }

            if (World.Tick > tick)
            {
                RollbackTo(tick, m_VideoFrames.ServerFrames.Count, false);
            }

            while (World.Tick <= tick)
            {
                ServerFrame tframe = m_VideoFrames.ServerFrames[World.Tick];
                Simulate(tframe, false);
            }

            m_TimestampOnLastJumpToMs = GameTime.CurrTimeStamp;
            m_TickOnLastJumpTo = tick;
        }

        public void UpdateVideo()
        {
            if (m_VideoFrames == null)
            {
                return;
            }

            if (m_TickOnLastJumpTo == World.Tick)
            {
                m_TimestampOnLastJumpToMs = GameTime.CurrTimeStamp;
                m_TickOnLastJumpTo = World.Tick;
            }

            if (GameEntry.Service.GetService<CommonStateService>().IsPause)
            {
                return;
            }

            long frameDeltaTime = (GameTime.CurrTimeStamp - m_TimestampOnLastJumpToMs);
            int targetTick = (int)System.Math.Ceiling((double)frameDeltaTime / CommonDefinitions.UpdateDeltatime) + m_TickOnLastJumpTo;
            while (World.Tick <= targetTick)
            {
                if (World.Tick < m_VideoFrames.ServerFrames.Count)
                {
                    ServerFrame frame = m_VideoFrames.ServerFrames[World.Tick];
                    Simulate(frame, false);
                }
                else
                {
                    break;
                }
            }
        }

        #endregion
    }
}
