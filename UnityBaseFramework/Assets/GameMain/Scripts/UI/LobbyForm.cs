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

        private TMP_Text m_WaitingNum = null;
        private Button m_BtnReady = null;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);

            m_WaitingNum = transform.Find("Content/Center/WaitingNum").GetComponent<TMP_Text>();
            m_WaitingNum.text = "Waiting...";
            m_BtnReady = transform.Find("Content/Center/BtnReady").GetComponent<Button>();
            m_BtnReady.onClick.AddListener(OnClickBtnReady);
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

        private void OnClickBtnReady()
        {
            INetworkChannel tcpChannel = GameEntry.NetworkExtended.TcpChannel;
            if (tcpChannel == null)
            {
                Log.Error("Cannot Start, tcpChannel is null.");
                return;
            }
            //准备游戏
            CSReady csReady = ReferencePool.Acquire<CSReady>();
            tcpChannel.Send(csReady);
        }
    }
}
