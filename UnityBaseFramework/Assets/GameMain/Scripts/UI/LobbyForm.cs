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
            INetworkChannel tcpChannel = GameEntry.NetworkExtended.TcpChannel;
            if (tcpChannel == null)
            {
                Log.Error("Cannot Start, tcpChannel is null.");
                return;
            }
            // JoinRoom；
            CSJoinRoom csJoinRoom = ReferencePool.Acquire<CSJoinRoom>();
            // RoomId为0，随机加入房间；
            csJoinRoom.RoomId = 0;  
            tcpChannel.Send(csJoinRoom);
        }

        private void OnClickBtnReady()
        {
            INetworkChannel tcpChannel = GameEntry.NetworkExtended.TcpChannel;
            if (tcpChannel == null)
            {
                Log.Error("Cannot Start, tcpChannel is null.");
                return;
            }
            // 准备游戏
            CSReady csReady = ReferencePool.Acquire<CSReady>();
            // 0:取消准备，1:准备
            csReady.Status = 1;
            tcpChannel.Send(csReady);
        }

        public void OnJoinedRoom(int roomId, int readyCount)
        {
            m_BtnJoin.gameObject.SetActive(false);
            m_BtnReady.gameObject.SetActive(true);
            m_RoomText.text = $"RoomId: {roomId}";
            m_ReadyCount.text = $"ReadyCount: {readyCount}/{CommonDefinitions.MaxRoomMemberCount}";
        }

        public void RefreshReadyCount(int count)
        {
            if (count <= 0)
            {
                m_ReadyCount.text = "ReadyCount ...";
            }
            else
            {
                m_ReadyCount.text = $"ReadyCount... {count}/{CommonDefinitions.MaxRoomMemberCount}";
            }
        }
    }
}
