using BaseFramework;
using BaseFramework.Event;
using BaseFramework.Network;
using GameProto;
using UnityBaseFramework.Runtime;
using ProcedureOwner = BaseFramework.Fsm.IFsm<BaseFramework.Procedure.IProcedureManager>;

namespace XGame
{
    public class ProcedureLobby : ProcedureBase
    {
        private int? m_LobbyFormSerialId = null;
        private LobbyForm m_LobbyForm = null;

        private EUserState m_UserState = EUserState.Default;

        private bool m_IsAllReady = false;

        private bool m_IsNetworkError = false;

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
            GameEntry.Event.Subscribe(SCJoinRoomEventArgs.EventId, OnJoinRoomResponse);
            GameEntry.Event.Subscribe(SCReadyEventArgs.EventId, OnReadyResponse);
            GameEntry.Event.Subscribe(SCGameStartInfoEventArgs.EventId, OnGameStartInfoResponse);
            GameEntry.Event.Subscribe(UnityBaseFramework.Runtime.NetworkClosedEventArgs.EventId, OnNetworkClosed);

            //重置数据。
            m_IsAllReady = false;
            m_UserState = EUserState.Default;
            m_IsNetworkError = false;

            if (procedureOwner.HasData("UserState"))
            {
                int tUserState = procedureOwner.GetData<VarInt32>("UserState");
                m_UserState = (EUserState)tUserState;
            }

            if (m_LobbyFormSerialId == null || !GameEntry.UI.HasUIForm((int)m_LobbyFormSerialId))
            {
                m_LobbyFormSerialId = GameEntry.UI.OpenUIForm(UIFormId.LobbyForm, this);
            }
            else
            {
                UIForm form = GameEntry.UI.GetUIForm((int)m_LobbyFormSerialId);
                if (form != null)
                {
                    m_LobbyForm = (LobbyForm)form.Logic;
                    GameEntry.UI.RefocusUIForm(form);

                    OnAfterOpenUI();
                }
            }
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            GameEntry.Event.Unsubscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);
            GameEntry.Event.Unsubscribe(SCJoinRoomEventArgs.EventId, OnJoinRoomResponse);
            GameEntry.Event.Unsubscribe(SCReadyEventArgs.EventId, OnReadyResponse);
            GameEntry.Event.Unsubscribe(SCGameStartInfoEventArgs.EventId, OnGameStartInfoResponse);
            GameEntry.Event.Unsubscribe(UnityBaseFramework.Runtime.NetworkClosedEventArgs.EventId, OnNetworkClosed);

            if (m_LobbyForm != null)
            {
                m_LobbyForm.Close(isShutdown);
                m_LobbyForm = null;
            }
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (m_IsNetworkError)
            {
                // 切换场景，然后发送开始游戏。
                procedureOwner.SetData<VarInt32>("NextSceneId", GameEntry.Config.GetInt("Scene.Login"));
                ChangeState<ProcedureChangeScene>(procedureOwner);
            }

            // 所有人都准备好了，加载游戏场景。
            if (m_IsAllReady)
            {
                // 切换场景，然后发送开始游戏。
                procedureOwner.SetData<VarInt32>("NextSceneId", GameEntry.Config.GetInt("Scene.Map01"));
                ChangeState<ProcedureChangeScene>(procedureOwner);
            }
        }

        private void OnOpenUIFormSuccess(object sender, GameEventArgs e)
        {
            OpenUIFormSuccessEventArgs ne = (OpenUIFormSuccessEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            m_LobbyForm = (LobbyForm)ne.UIForm.Logic;

            OnAfterOpenUI();
        }

        private void OnAfterOpenUI()
        {
            CheckReconnected();
        }

        private void CheckReconnected()
        {
            switch (m_UserState)
            {
                case EUserState.NotReady:
                    //重连，在房间未准备状态。
                    Ready((int)EUserState.LoggedIn);
                    break;
                case EUserState.Ready:
                    //重连，在房间已准备状态。
                    Ready((int)EUserState.LoggedIn);
                    break;
                default:
                    break;
            }
        }

        public void JoinRoom()
        {
            INetworkChannel tcpChannel = GameEntry.NetworkExtended.TcpChannel;
            if (tcpChannel == null)
            {
                Log.Error("Cannot JoinRoom, tcpChannel is null.");
                return;
            }
            //JoinRoom；
            CSJoinRoom csJoinRoom = ReferencePool.Acquire<CSJoinRoom>();
            //-1:离开房间 0:随机加入 其他:加入指定房间。
            csJoinRoom.RoomId = 0;
            tcpChannel.Send(csJoinRoom);
        }

        public void LeaveRoom()
        {
            INetworkChannel tcpChannel = GameEntry.NetworkExtended.TcpChannel;
            if (tcpChannel == null)
            {
                Log.Error("Cannot JoinRoom, tcpChannel is null.");
                return;
            }
            //LeaveRoom；
            CSJoinRoom csJoinRoom = ReferencePool.Acquire<CSJoinRoom>();
            //-1:离开房间 0:随机加入 其他:加入指定房间
            csJoinRoom.RoomId = -1;
            tcpChannel.Send(csJoinRoom);
        }

        private void OnJoinRoomResponse(object sender, GameEventArgs e)
        {
            SCJoinRoomEventArgs scJoinRoomEventArgs = (SCJoinRoomEventArgs)e;
            if (scJoinRoomEventArgs == null)
            {
                return;
            }

            if (scJoinRoomEventArgs.RoomId >= 0)
            {
                int readyCount = 0;
                for (int i = 0; i < scJoinRoomEventArgs.UserGameInfos.Count; i++)
                {
                    UserGameInfo userGameInfo = scJoinRoomEventArgs.UserGameInfos[i];
                    if (userGameInfo != null && userGameInfo.UserState == (int)EUserState.Ready)
                    {
                        readyCount++;
                    }

                    //记录自己的状态
                    if (userGameInfo.LocalId == scJoinRoomEventArgs.LocalId)
                    {
                        m_UserState = (EUserState)userGameInfo.UserState;
                    }
                }
                m_LobbyForm?.OnJoinedRoom(scJoinRoomEventArgs.RoomId, scJoinRoomEventArgs.LocalId, readyCount, m_UserState);

                Log.Info($"OnJoinRoomResponse RoomId:{scJoinRoomEventArgs.RoomId} LocalId:{scJoinRoomEventArgs.LocalId}");
            }
            else
            {
                //LeaveRoom
                m_LobbyForm?.OnLeaveRoom();
            }
        }

        public void Ready(int readyState)
        {
            //readyState
            INetworkChannel tcpChannel = GameEntry.NetworkExtended.TcpChannel;
            if (tcpChannel == null)
            {
                Log.Error("Cannot Ready, tcpChannel is null.");
                return;
            }
            //准备游戏
            CSReady csReady = ReferencePool.Acquire<CSReady>();
            csReady.UserState = readyState;
            tcpChannel.Send(csReady);
        }

        private void OnReadyResponse(object sender, GameEventArgs e)
        {
            SCReadyEventArgs scReadyEventArgs = (SCReadyEventArgs)e;
            if (scReadyEventArgs == null)
            {
                return;
            }

            int readyCount = 0;
            for (int i = 0; i < scReadyEventArgs.UserGameInfos.Count; i++)
            {
                UserGameInfo userGameInfo = scReadyEventArgs.UserGameInfos[i];
                if (userGameInfo != null && userGameInfo.UserState == (int)EUserState.Ready)
                {
                    readyCount++;
                }

                //记录自己的状态
                if (userGameInfo.LocalId == scReadyEventArgs.LocalId)
                {
                    m_UserState = (EUserState)userGameInfo.UserState;
                }
            }

            m_LobbyForm?.OnReadyState(scReadyEventArgs.RoomId, scReadyEventArgs.LocalId, readyCount, m_UserState);
        }

        private void OnGameStartInfoResponse(object sender, GameEventArgs e)
        {
            SCGameStartInfoEventArgs scGameStartInfoEventArgs = (SCGameStartInfoEventArgs)e;
            if (scGameStartInfoEventArgs == null)
            {
                return;
            }
            Log.Info($"OnGameStartInfoResponse  RoomId:{scGameStartInfoEventArgs.RoomId}  MapId:{scGameStartInfoEventArgs.MapId}  UserCount:{scGameStartInfoEventArgs.UserGameInfos.Count}");

            //开始计时。
            //初始化时间戳。
            GameTime.InitStartTimeStamp();

            Simulator simulator = new Simulator();
            simulator.Start();
            simulator.OnGameCreate(
                60,
                scGameStartInfoEventArgs.MapId,
                scGameStartInfoEventArgs.LocalId,
                scGameStartInfoEventArgs.UserCount,
                scGameStartInfoEventArgs.UserGameInfos);

            //所有人都准备完成。
            m_IsAllReady = true;
        }


        #region Network

        private void OnNetworkClosed(object sender, GameEventArgs e)
        {
            UnityBaseFramework.Runtime.NetworkClosedEventArgs ne = (UnityBaseFramework.Runtime.NetworkClosedEventArgs)e;
            if (ne.NetworkChannel != GameEntry.NetworkExtended.TcpChannel)
            {
                return;
            }

            m_IsNetworkError = true;
        }

        #endregion
    }
}
