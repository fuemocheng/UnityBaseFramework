using UnityEngine;
using UnityBaseFramework.Runtime;
using UnityEngine.UI;
using BaseFramework.Network;
using System.Net;

namespace XGame
{
    public class LoginForm : UGuiForm
    {
        private ProcedureLogin m_ProcedureLogin = null;

        private InputField m_InputAccount = null;
        private InputField m_InputPassword = null;
        private Button m_BtnLogin = null;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);

            m_InputAccount = transform.Find("Content/Center/Account").GetComponent<InputField>();
            m_InputPassword = transform.Find("Content/Center/Password").GetComponent<InputField>();
            m_BtnLogin = transform.Find("Content/Center/BtnLogin").GetComponent<Button>();

            m_BtnLogin.onClick.AddListener(OnClickLogin);
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            m_ProcedureLogin = (ProcedureLogin)userData;
            if (m_ProcedureLogin == null)
            {
                Log.Warning("ProcedureMenu is invalid when open MenuForm.");
                return;
            }
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            m_ProcedureLogin = null;

            base.OnClose(isShutdown, userData);
        }


        private void OnClickLogin()
        {
            INetworkChannel tcpChannel = GameEntry.NetworkExtended.TcpChannel;
            if (tcpChannel == null)
            {
                Log.Error("Cannot connected, tcpChannel is null.");
                return;
            }
            tcpChannel.Connect(IPAddress.Parse("127.0.0.1"), 9001);
        }
    }
}
