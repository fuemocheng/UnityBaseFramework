using UnityEngine;
using UnityBaseFramework.Runtime;
using TMPro;
using UnityEngine.UI;
using BaseFramework.Network;
using GameProto;
using BaseFramework;

namespace XGame
{
    public class LobbyForm : UGuiForm
    {
        private ProcedureLobby m_ProcedureLobby = null;

        private TMP_Text m_RoomText = null;
        private TMP_Text m_ReadyCount = null;
        private Button m_BtnJoin = null;
        private Button m_BtnReady = null;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);

            m_RoomText = transform.Find("Content/Center/RoomText").GetComponent<TMP_Text>();
            m_RoomText.text = "";
            m_ReadyCount = transform.Find("Content/Center/ReadyCount").GetComponent<TMP_Text>();
            m_ReadyCount.text = "";
            m_BtnJoin = transform.Find("Content/Center/BtnJoin").GetComponent<Button>();
            m_BtnJoin.onClick.AddListener(OnClickBtnJoin);
            m_BtnReady = transform.Find("Content/Center/BtnReady").GetComponent<Button>();
            m_BtnReady.onClick.AddListener(OnClickBtnReady);
            m_BtnReady.gameObject.SetActive(false);
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

        private void OnClickBtnJoin()
        {
            m_ProcedureLobby.JoinRoom();
        }

        private void OnClickBtnReady()
        {
            m_ProcedureLobby.Ready((int)EUserState.Ready);
        }

        private void OnClickBtnCancelReady()
        {
            m_ProcedureLobby.Ready((int)EUserState.NotReady);
        }

        public void OnJoinedRoom(int roomId, int localId, int readyCount)
        {
            m_BtnJoin.gameObject.SetActive(false);
            m_BtnReady.gameObject.SetActive(true);
            m_RoomText.text = $"RoomId: {roomId}";
            m_ReadyCount.text = $"ReadyCount: {readyCount}/{CommonDefinitions.MaxRoomMemberCount}";
        }

        public void OnReadyState(int roomId, int localId, int readyCount)
        {
            m_BtnJoin.gameObject.SetActive(false);
            m_BtnReady.gameObject.SetActive(true);
            m_RoomText.text = $"RoomId: {roomId}";
            if (readyCount <= 0)
            {
                m_ReadyCount.text = "ReadyCount ...";
            }
            else
            {
                m_ReadyCount.text = $"ReadyCount... {readyCount}/{CommonDefinitions.MaxRoomMemberCount}";
            }
        }
    }
}
