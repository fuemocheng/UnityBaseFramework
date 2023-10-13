using BaseFramework;
using BaseFramework.Event;
using BaseFramework.Network;
using GameProto;
using Lockstep.Util;
using UnityBaseFramework.Runtime;
using ProcedureOwner = BaseFramework.Fsm.IFsm<BaseFramework.Procedure.IProcedureManager>;

namespace XGame
{
    public class ProcedureLobby : ProcedureBase
    {
        private LobbyForm m_LobbyForm = null;

        private EUserState m_UserState = EUserState.Default;

        private bool m_IsAllReady = false;

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

            GameEntry.UI.OpenUIForm(UIFormId.LobbyForm, this);

            if (procedureOwner.HasData("UserState"))
            {
                int tUserState = procedureOwner.GetData<VarInt32>("UserState");
                m_UserState = (EUserState)tUserState;
            }
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            GameEntry.Event.Unsubscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);
            GameEntry.Event.Unsubscribe(SCJoinRoomEventArgs.EventId, OnJoinRoomResponse);
            GameEntry.Event.Unsubscribe(SCReadyEventArgs.EventId, OnReadyResponse);
            GameEntry.Event.Unsubscribe(SCGameStartInfoEventArgs.EventId, OnGameStartInfoResponse);

            if (m_LobbyForm != null)
            {
                m_LobbyForm.Close(isShutdown);
                m_LobbyForm = null;
            }

            m_IsAllReady = false;
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

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
            // JoinRoom；
            CSJoinRoom csJoinRoom = ReferencePool.Acquire<CSJoinRoom>();
            // RoomId为0，随机加入房间；重连的话也用默认Id。
            csJoinRoom.RoomId = 0;
            tcpChannel.Send(csJoinRoom);
        }

        private void OnJoinRoomResponse(object sender, GameEventArgs e)
        {
            SCJoinRoomEventArgs scJoinRoomEventArgs = (SCJoinRoomEventArgs)e;
            if (scJoinRoomEventArgs == null)
            {
                return;
            }

            int readyCount = 0;
            for (int i = 0; i < scJoinRoomEventArgs.UserReadyInfos.Count; i++)
            {
                UserReadyInfo userReadyInfo = scJoinRoomEventArgs.UserReadyInfos[i];
                if (userReadyInfo != null && userReadyInfo.UserState == (int)EUserState.Ready)
                {
                    readyCount++;
                }
            }
            m_LobbyForm.OnJoinedRoom(scJoinRoomEventArgs.RoomId, scJoinRoomEventArgs.LocalId, readyCount);

            Log.Info($"OnJoinRoomResponse RoomId:{scJoinRoomEventArgs.RoomId} LocalId:{scJoinRoomEventArgs.LocalId}");
        }

        public void Ready(int readyState)
        {
            // readyState
            INetworkChannel tcpChannel = GameEntry.NetworkExtended.TcpChannel;
            if (tcpChannel == null)
            {
                Log.Error("Cannot Ready, tcpChannel is null.");
                return;
            }
            // 准备游戏
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

            // 开始计时。
            LTime.DoStart();

            int readyCount = 0;
            for (int i = 0; i < scReadyEventArgs.UserReadyInfos.Count; i++)
            {
                UserReadyInfo userReadyInfo = scReadyEventArgs.UserReadyInfos[i];
                if (userReadyInfo != null && userReadyInfo.UserState == (int)EUserState.Ready)
                {
                    readyCount++;
                }
            }

            m_LobbyForm.OnReadyState(scReadyEventArgs.RoomId, scReadyEventArgs.LocalId, readyCount);
        }

        private void OnGameStartInfoResponse(object sender, GameEventArgs e)
        {
            SCGameStartInfoEventArgs scGameStartInfoEventArgs = (SCGameStartInfoEventArgs)e;
            if (scGameStartInfoEventArgs == null)
            {
                return;
            }

            Simulator simulator = new Simulator();
            simulator.Start();
            simulator.OnGameCreate(60,
                scGameStartInfoEventArgs.MapId,
                scGameStartInfoEventArgs.LocalId,
                scGameStartInfoEventArgs.UserCount,
                scGameStartInfoEventArgs.Users);

            // 所有人都准备完成。
            m_IsAllReady = true;

            Log.Info($"OnGameStartInfoResponse  RoomId:{scGameStartInfoEventArgs.RoomId}  MapId:{scGameStartInfoEventArgs.MapId}  UserCount:{scGameStartInfoEventArgs.Users.Count}");
        }
    }
}
