using BaseFramework;
using BaseFramework.Event;
using BaseFramework.Network;
using GameProto;
using Lockstep.Util;
using UnityBaseFramework.Runtime;
using ProcedureOwner = BaseFramework.Fsm.IFsm<BaseFramework.Procedure.IProcedureManager>;

namespace XGame
{
    public class ProcedureMap : ProcedureBase
    {
        private MainUIForm m_MainUIForm = null;

        private EUserState m_UserState = EUserState.Default;

        private float m_DelaySendLoadingProgressTime = CommonDefinitions.DelaySendLoadingProgressTime;
        private float m_Time = 0;

        private bool m_IsReconnected = false;

        public override bool UseNativeDialog
        {
            get
            {
                return false;
            }
        }

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            GameEntry.Event.Subscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);
            GameEntry.Event.Subscribe(SCPingEventArgs.EventId, OnPingResponse);
            GameEntry.Event.Subscribe(SCLoadingProgressEventArgs.EventId, OnLoadingProgressResponse);
            GameEntry.Event.Subscribe(SCServerFrameEventArgs.EventId, OnServerFrameResponse);
            GameEntry.Event.Subscribe(SCReqMissFrameEventArgs.EventId, OnReqMissFrameResponse);
            GameEntry.Event.Subscribe(SCGameStartInfoEventArgs.EventId, OnGameStartInfoResponse);

            GameEntry.UI.OpenUIForm(UIFormId.MainUIForm, this);

            // 重连逻辑。
            if (procedureOwner.HasData("UserState"))
            {
                m_UserState = (EUserState)(int)(procedureOwner.GetData<VarInt32>("UserState"));
                switch (m_UserState)
                {
                    case EUserState.Loading:
                    case EUserState.Playing:
                        m_IsReconnected = true;
                        break;
                    default:
                        m_IsReconnected = false;
                        break;
                }
            }

            m_Time = 0;

            // 场景已经加载完成，延迟发送加载进度。
            //SendLoadingProgress();
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            GameEntry.Event.Unsubscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);
            GameEntry.Event.Unsubscribe(SCPingEventArgs.EventId, OnPingResponse);
            GameEntry.Event.Unsubscribe(SCLoadingProgressEventArgs.EventId, OnLoadingProgressResponse);
            GameEntry.Event.Unsubscribe(SCServerFrameEventArgs.EventId, OnServerFrameResponse);
            GameEntry.Event.Unsubscribe(SCReqMissFrameEventArgs.EventId, OnReqMissFrameResponse);
            GameEntry.Event.Unsubscribe(SCGameStartInfoEventArgs.EventId, OnGameStartInfoResponse);

            if (m_MainUIForm != null)
            {
                m_MainUIForm.Close(isShutdown);
                m_MainUIForm = null;
            }
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (!m_IsReconnected)
            {
                if (m_Time < m_DelaySendLoadingProgressTime)
                {
                    m_Time += elapseSeconds;
                    if (m_Time >= m_DelaySendLoadingProgressTime)
                    {
                        // 延迟发送加载进度。
                        SendLoadingProgress();
                    }
                }
            }

            //模拟器更新
            Simulator.Instance?.Update(elapseSeconds, realElapseSeconds);
        }

        private void OnOpenUIFormSuccess(object sender, GameEventArgs e)
        {
            OpenUIFormSuccessEventArgs ne = (OpenUIFormSuccessEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            m_MainUIForm = (MainUIForm)ne.UIForm.Logic;

            //检查是否重连。
            CheckReconnected();
        }


        private void SendLoadingProgress()
        {
            INetworkChannel tcpChannel = GameEntry.NetworkExtended.TcpChannel;
            if (tcpChannel == null)
            {
                Log.Error("Cannot SendLoadingProgress, tcpChannel is null.");
                return;
            }
            //发送进度
            CSLoadingProgress csLoadingProgress = ReferencePool.Acquire<CSLoadingProgress>();
            csLoadingProgress.Progress = 100;
            tcpChannel.Send(csLoadingProgress);
        }

        private void OnLoadingProgressResponse(object sender, GameEventArgs e)
        {
            SCLoadingProgressEventArgs scLoadingProgressEventArgs = (SCLoadingProgressEventArgs)e;
            if (scLoadingProgressEventArgs == null)
            {
                return;
            }

            if(scLoadingProgressEventArgs.AllProgress < 100)
            {
                //TODO: Loading 界面 设置加载进度。
                Log.Info($"OnLoadingProgressResponse AllProgress:{scLoadingProgressEventArgs.AllProgress}");
            }
            else
            {
                // 所有客户端都完成了准备工作。
                // 开始游戏，发送帧数据
                Log.Info("Start Game.");

                Simulator.Instance?.StartSimulate();
            }
        }

        private void OnPingResponse(object sender, GameEventArgs e)
        {
            SCPingEventArgs scPingEventArgs = (SCPingEventArgs)e;
            if (scPingEventArgs == null)
            {
                return;
            }
            Simulator.Instance?.OnPing(scPingEventArgs);
        }

        private void OnServerFrameResponse(object sender, GameEventArgs e)
        {
            SCServerFrameEventArgs scServerFrameEventArgs = (SCServerFrameEventArgs)e;
            if (scServerFrameEventArgs == null)
            {
                return;
            }
            //Log.Error("ProcedureMap:OnServerFrameResponse.Tick {0}", scServerFrameEventArgs.ServerFrames[scServerFrameEventArgs.ServerFrames.Count - 1].Tick);
            Simulator.Instance?.OnServerFrame(scServerFrameEventArgs.ServerFrames);
        }

        private void OnReqMissFrameResponse(object sender, GameEventArgs e)
        {
            SCReqMissFrameEventArgs scReqMissFrameEventArgs = (SCReqMissFrameEventArgs)e;
            if (scReqMissFrameEventArgs == null)
            {
                return;
            }
            //Log.Error("ProcedureMap:OnReqMissFrameResponse.Tick {0}", scReqMissFrameEventArgs.ServerFrames[scReqMissFrameEventArgs.ServerFrames.Count - 1].Tick);
            Simulator.Instance?.OnReqMissFrame(scReqMissFrameEventArgs.ServerFrames);
        }

        #region Reconnect
        private void CheckReconnected()
        {
            switch (m_UserState)
            {
                case EUserState.Loading:
                    Log.Info("Reconnected - Loading.");

                    // 重连是加载状态，则立即发送加载完成信息，不在延迟。
                    m_DelaySendLoadingProgressTime = 0;
                    break;
                case EUserState.Playing:
                    Log.Info("Reconnected - Playing.");

                    // 重连是游戏中，则向服务器获取游戏基础信息。
                    SendCSGameStartInfo();
                    break;
                default:
                    break;
            }
        }

        private void SendCSGameStartInfo()
        {
            INetworkChannel tcpChannel = GameEntry.NetworkExtended.TcpChannel;
            if (tcpChannel == null)
            {
                Log.Error("Cannot Ready, tcpChannel is null.");
                return;
            }
            CSGameStartInfo csGameStartInfo = ReferencePool.Acquire<CSGameStartInfo>();
            tcpChannel.Send(csGameStartInfo);
        }

        private void OnGameStartInfoResponse(object sender, GameEventArgs e)
        {
            SCGameStartInfoEventArgs scGameStartInfoEventArgs = (SCGameStartInfoEventArgs)e;
            if (scGameStartInfoEventArgs == null)
            {
                return;
            }
            Log.Info($"OnGameStartInfoResponse  RoomId:{scGameStartInfoEventArgs.RoomId}  MapId:{scGameStartInfoEventArgs.MapId}  UserCount:{scGameStartInfoEventArgs.Users.Count}");

            // 开始计时。
            //LTime.DoStart();
            // 初始化时间戳。
            GameTime.InitStartTimeStamp();

            Simulator simulator = new Simulator();
            simulator.Start();
            simulator.OnGameCreate(60,
                scGameStartInfoEventArgs.MapId,
                scGameStartInfoEventArgs.LocalId,
                scGameStartInfoEventArgs.UserCount,
                scGameStartInfoEventArgs.Users);

            // 因为是重连，则直接开始游戏。
            Log.Info("Reconnected - Start Game.");

            simulator.StartSimulate();

            simulator.SendReqMissFrame(0);
        }

        #endregion
    }
}
