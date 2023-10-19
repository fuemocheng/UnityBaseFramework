using UnityEngine;
using UnityBaseFramework.Runtime;
using UnityEngine.UI;
using BaseFramework.Network;
using GameProto;
using BaseFramework;

namespace XGame
{
    public class LobbyForm : UGuiForm
    {
        private ProcedureLobby m_ProcedureLobby = null;

        private Text m_RoomText = null;
        private Text m_ReadyCount = null;
        private Button m_BtnJoinRoom = null;
        private Button m_BtnLeaveRoom = null;
        private Button m_BtnGetReady = null;
        private Button m_BtnCancelReady = null;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);

            m_RoomText = transform.Find("Content/Center/RoomId").GetComponent<Text>();
            m_ReadyCount = transform.Find("Content/Center/ReadyCount").GetComponent<Text>();
            m_BtnJoinRoom = transform.Find("Content/Center/BtnJoinRoom").GetComponent<Button>();
            m_BtnJoinRoom.onClick.AddListener(OnClickBtnJoinRoom);
            m_BtnLeaveRoom = transform.Find("Content/Center/BtnLeaveRoom").GetComponent<Button>();
            m_BtnLeaveRoom.onClick.AddListener(OnClickBtnLeaveRoom);
            m_BtnGetReady = transform.Find("Content/Center/BtnGetReady").GetComponent<Button>();
            m_BtnGetReady.onClick.AddListener(OnClickBtnGetReady);
            m_BtnCancelReady = transform.Find("Content/Center/BtnCancelReady").GetComponent<Button>();
            m_BtnCancelReady.onClick.AddListener(OnClickBtnCancelReady);

            m_RoomText.text = $"Start Matching";
            m_ReadyCount.text = "";
            m_BtnJoinRoom.gameObject.SetActive(true);
            m_BtnLeaveRoom.gameObject.SetActive(false);
            m_BtnGetReady.gameObject.SetActive(false);
            m_BtnCancelReady.gameObject.SetActive(false);
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            m_ProcedureLobby = (ProcedureLobby)userData;
            if (m_ProcedureLobby == null)
            {
                Log.Warning("ProcedureMenu is invalid when open MenuForm.");
                return;
            }
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            m_ProcedureLobby = null;

            base.OnClose(isShutdown, userData);
        }

        protected override void OnResume()
        {
            base.OnResume();

            m_RoomText.text = $"Start Matching";
            m_ReadyCount.text = "";
            m_BtnJoinRoom.gameObject.SetActive(true);
            m_BtnLeaveRoom.gameObject.SetActive(false);
            m_BtnGetReady.gameObject.SetActive(false);
            m_BtnCancelReady.gameObject.SetActive(false);
        }

        private void OnClickBtnJoinRoom()
        {
            m_ProcedureLobby.JoinRoom();
        }

        private void OnClickBtnLeaveRoom()
        {
            m_ProcedureLobby.LeaveRoom();
        }

        private void OnClickBtnGetReady()
        {
            m_ProcedureLobby.Ready((int)EUserState.Ready);
        }

        private void OnClickBtnCancelReady()
        {
            m_ProcedureLobby.Ready((int)EUserState.NotReady);
        }

        public void OnJoinedRoom(int roomId, int localId, int readyCount, EUserState userState)
        {
            m_BtnJoinRoom.gameObject.SetActive(false);
            m_BtnLeaveRoom.gameObject.SetActive(true);
            if (userState == EUserState.NotReady)
            {
                m_BtnGetReady.gameObject.SetActive(true);
                m_BtnCancelReady.gameObject.SetActive(false);
            }
            else
            {
                m_BtnGetReady.gameObject.SetActive(false);
                m_BtnCancelReady.gameObject.SetActive(true);
            }

            m_RoomText.text = $"RoomId: {roomId}  LocalId:{localId}";
            m_ReadyCount.text = $"ReadyCount: {readyCount}/{CommonDefinitions.MaxRoomMemberCount}";
        }

        public void OnLeaveRoom()
        {
            m_BtnJoinRoom.gameObject.SetActive(true);
            m_BtnLeaveRoom.gameObject.SetActive(false);
            m_BtnGetReady.gameObject.SetActive(false);
            m_BtnCancelReady.gameObject.SetActive(false);

            m_RoomText.text = $"Start Matching";
            m_ReadyCount.text = "";
        }

        public void OnReadyState(int roomId, int localId, int readyCount, EUserState userState)
        {
            m_BtnJoinRoom.gameObject.SetActive(false);
            m_BtnLeaveRoom.gameObject.SetActive(true);
            if (userState == EUserState.NotReady)
            {
                m_BtnGetReady.gameObject.SetActive(true);
                m_BtnCancelReady.gameObject.SetActive(false);
            }
            else
            {
                m_BtnGetReady.gameObject.SetActive(false);
                m_BtnCancelReady.gameObject.SetActive(true);
            }

            m_RoomText.text = $"RoomId: {roomId}  LocalId:{localId}";
            m_ReadyCount.text = $"ReadyCount: {readyCount}/{CommonDefinitions.MaxRoomMemberCount}";
        }
    }
}
