using System.Net;
using BaseFramework;
using BaseFramework.Network;
using UnityBaseFramework.Runtime;
using UnityEngine.UI;
using GameProto;

namespace XGame
{
    public class LoginForm : UGuiForm
    {
        private ProcedureLogin m_ProcedureLogin = null;

        private InputField m_InputAccount = null;
        private InputField m_InputPassword = null;
        private Button m_BtnLogin = null;
        private Button m_BtnClientMode = null;

        private bool m_IsClientMode = false;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);

            m_InputAccount = transform.Find("Content/Center/Account").GetComponent<InputField>();
            m_InputPassword = transform.Find("Content/Center/Password").GetComponent<InputField>();
            m_BtnLogin = transform.Find("Content/Center/BtnLogin").GetComponent<Button>();
            m_BtnClientMode = transform.Find("Content/Center/BtnClientMode").GetComponent<Button>();

            m_BtnLogin.onClick.AddListener(OnClickLogin);
            m_BtnClientMode.onClick.AddListener(OnClickClientMode);
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
            string account = m_InputAccount.text;
            if (account.Length < 6 || account.Length > 12)
            {
                GameEntry.UI.OpenDialog(new DialogParams
                {
                    Mode = 1,
                    Title = "Account Error",
                    Message = "Please input correct account.",
                    ConfirmText = "Confirm",
                    OnClickConfirm = delegate (object userData)
                    {
                        //Do something.
                    },
                });
                return;
            }
            //string passWord = m_InputPassword.text;
            //if (passWord.Length < 6 || passWord.Length > 12)
            //{
            //    Log.Error("Please input correct password.");
            //    return;
            //}

            INetworkChannel tcpChannel = GameEntry.NetworkExtended.TcpChannel;
            if (tcpChannel == null)
            {
                Log.Error("Cannot connected, tcpChannel is null.");
                return;
            }
            tcpChannel.Connect(IPAddress.Parse("127.0.0.1"), 9001);
        }

        public void Login()
        {
            INetworkChannel tcpChannel = GameEntry.NetworkExtended.TcpChannel;
            if (tcpChannel == null)
            {
                Log.Error("Cannot Login, tcpChannel is null.");
                return;
            }
            CSLogin loginVerify = ReferencePool.Acquire<CSLogin>();
            loginVerify.Account = m_InputAccount.text;
            loginVerify.Password = m_InputPassword.text;
            tcpChannel.Send(loginVerify);
        }

        private void OnClickClientMode()
        {
            m_ProcedureLogin?.OnClientMode();
        }
    }
}
