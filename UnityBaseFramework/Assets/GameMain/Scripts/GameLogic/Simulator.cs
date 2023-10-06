using GameProto;
using Lockstep.Util;
using System.Collections;
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

        public int PingVal => m_FrameBuffer?.PingVal ?? 0;
        public int DelayVal => m_FrameBuffer?.DelayVal ?? 0;

        public bool IsRunning { get; set; }

        public long GameStartTimestampMs = -1; //游戏开始时间戳。
        private int m_TickSinceGameStart = 0; //从游戏开始运行的逻辑帧数。

        private int m_InputTick = 0;

        // frame count that need predict(TODO should change according current network's delay).
        public int FramePredictCount = 0;
        // 预发送输入帧数量。
        public int PreSendInputCount = 1;

        public int TargetTick => m_TickSinceGameStart + FramePredictCount;
        public int InputTargetTick => m_TickSinceGameStart + PreSendInputCount;


        private int m_SnapshotFrameInterval = 1; //快照间隔。
        private bool m_HasRecvInputMsg = false; //是否收到输入帧消息。


        public void Start()
        {
            Instance = this;

            World = new World();
            m_FrameBuffer = new FrameBuffer(this, 2000, m_SnapshotFrameInterval, MaxPredictFrameCount);

            IsRunning = false;
            m_HasRecvInputMsg = false;
            m_SnapshotFrameInterval = 1;
        }

        public void Destroy()
        {
            IsRunning = false;
            m_HasRecvInputMsg = false;
            m_SnapshotFrameInterval = 1;
        }


        public void OnGameCreate(int targetFps, int localActorId, int actorCount)
        {
            Log.Info("OnGameCreate");

            // Service 创建。

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

            World.StartSimulate();
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

            if (m_HasRecvInputMsg)
            {
                if (GameStartTimestampMs == -1)
                {
                    GameStartTimestampMs = LTime.realtimeSinceStartupMS;
                }
            }

            if (GameStartTimestampMs <= 0)
            {
                return;
            }

            m_TickSinceGameStart = (int)((LTime.realtimeSinceStartupMS - GameStartTimestampMs) / CommonDefinitions.UpdateDeltatime);

            while (m_InputTick <= InputTargetTick)
            {
                //SendInputs(m_InputTick++);
            }

            DoNormalUpdate();
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
            var deadline = LTime.realtimeSinceStartupMS + MaxSimulationMsPerFrame;
            while (World.Tick < m_FrameBuffer.CurTickInServer)
            {
                var tick = World.Tick;
                var sFrame = m_FrameBuffer.GetServerFrame(tick);
                if (sFrame == null)
                {
                    //OnPursuingFrame();
                    return;
                }

                m_FrameBuffer.PushLocalFrame(sFrame);
                Simulate(sFrame, tick == minTickToBackup);
                if (LTime.realtimeSinceStartupMS > deadline)
                {
                    //OnPursuingFrame();
                    return;
                }
            }

            //if (_constStateService.IsPursueFrame)
            //{
            //    _constStateService.IsPursueFrame = false;
            //    EventHelper.Trigger(EEvent.PursueFrameDone);
            //}


            // Roll back
            if (m_FrameBuffer.IsNeedRollback)
            {
                //RollbackTo(m_FrameBuffer.NextTickToCheck, maxContinueServerTick);
                //CleanUselessSnapshot(System.Math.Min(m_FrameBuffer.NextTickToCheck - 1, World.Tick));

                minTickToBackup = System.Math.Max(minTickToBackup, World.Tick + 1);
                while (World.Tick <= maxContinueServerTick)
                {
                    var sFrame = m_FrameBuffer.GetServerFrame(World.Tick);
                    //Logging.Debug.Assert(sFrame != null && sFrame.tick == World.Tick,
                    //    $" logic error: server Frame  must exist tick {World.Tick}");
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
                    //FillInputWithLastFrame(cFrame);
                    frame = cFrame;
                }

                m_FrameBuffer.PushLocalFrame(frame);
                //Predict(frame, true);
            }

            //_hashHelper.CheckAndSendHashCodes();

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
        private void Predoct(ServerFrame serverFrame, bool isNeedGenSnap = true)
        {
            Step(serverFrame, isNeedGenSnap);
        }

        /// <summary>
        /// 帧步进。
        /// </summary>
        /// <param name="serverFrame"></param>
        /// <param name="isNeedGenSnap"></param>
        private void Step(ServerFrame serverFrame, bool isNeedGenSnap = true)
        {
            // 记录当前 Tick。
            int tick = World.Tick;

            // 备份当前帧。
            //_timeMachineService.Backup(World.Tick);


            World.Step();

        }
    }
}
