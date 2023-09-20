using BaseFramework.Event;
using GameProto;
using UnityBaseFramework.Runtime;
using ProcedureOwner = BaseFramework.Fsm.IFsm<BaseFramework.Procedure.IProcedureManager>;

namespace XGame
{
    public class ProcedureLobby : ProcedureBase
    {
        private LobbyForm m_LobbyForm = null;

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
        }

        private void OnJoinRoomResponse(object sender, GameEventArgs e)
        {
            SCJoinRoomEventArgs ne = (SCJoinRoomEventArgs)e;
            if (ne.UserData == null)
            {
                return;
            }
            SCJoinRoom scReady = ne.UserData as SCJoinRoom;
            if (scReady == null)
            {
                return;
            }

            int readyCount = 0;
            for (int i = 0; i < scReady.UserReadyInfos.Count; i++)
            {
                UserReadyInfo userReadyInfo = scReady.UserReadyInfos[i];
                if (userReadyInfo != null && userReadyInfo.Status == 1)
                {
                    readyCount++;
                }
            }
            m_LobbyForm.OnJoinedRoom(scReady.RoomId, readyCount);

            Log.Info($"OnJoinRoomResponse RoomId:{scReady.RoomId} LocalId:{scReady.LocalId}");
        }

        private void OnReadyResponse(object sender, GameEventArgs e)
        {
            SCReadyEventArgs ne = (SCReadyEventArgs)e;
            if (ne.UserData == null)
            {
                return;
            }
            SCReady scReady = ne.UserData as SCReady;
            if (scReady == null)
            {
                return;
            }

            int readyCount = 0;
            for (int i = 0; i < scReady.UserReadyInfos.Count; i++)
            {
                UserReadyInfo userReadyInfo = scReady.UserReadyInfos[i];
                if (userReadyInfo != null && userReadyInfo.Status == 1)
                {
                    readyCount++;
                }
            }
            
            m_LobbyForm.RefreshReadyCount(readyCount);
        }

        private void OnGameStartInfoResponse(object sender, GameEventArgs e)
        {
            SCGameStartInfoEventArgs ne = (SCGameStartInfoEventArgs)e;
            if (ne.UserData == null)
            {
                return;
            }
            SCGameStartInfo scGameStartInfo = ne.UserData as SCGameStartInfo;
            if (scGameStartInfo == null)
            {
                return;
            }

            //for (int i = 0; i < scGameStartInfo.Users.Count; i++)
            //{
            //    User user = scGameStartInfo.Users[i];
            //}

            //TODO:
            //这里用服务器发送的MapId，加载地图；
            //用服务器发送的玩家数据，加载角色；

            // 所有人都准备完成。
            m_IsAllReady = true;
        }
    }
}
