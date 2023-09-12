using BaseFramework.Event;
using UnityBaseFramework.Runtime;
using ProcedureOwner = BaseFramework.Fsm.IFsm<BaseFramework.Procedure.IProcedureManager>;

namespace XGame
{
    public class ProcedureLogin : ProcedureBase
    {
        private bool m_Connected = false;
        private bool m_LoggedIn = false;
        private LoginForm m_LoginForm = null;

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
            GameEntry.Event.Subscribe(NetworkConnectedEventArgs.EventId, OnNetworkConnected);

            GameEntry.UI.OpenUIForm(UIFormId.LoginForm, this);
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            GameEntry.Event.Unsubscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);
            GameEntry.Event.Unsubscribe(NetworkConnectedEventArgs.EventId, OnNetworkConnected);

            if (m_LoginForm != null)
            {
                m_LoginForm.Close(isShutdown);
                m_LoginForm = null;
            }
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
        }

        private void OnOpenUIFormSuccess(object sender, GameEventArgs e)
        {
            OpenUIFormSuccessEventArgs ne = (OpenUIFormSuccessEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            m_LoginForm = (LoginForm)ne.UIForm.Logic;
        }

        private void OnNetworkConnected(object sender, GameEventArgs e)
        {
            NetworkConnectedEventArgs ne = (NetworkConnectedEventArgs)e;
            if (ne.NetworkChannel != GameEntry.NetworkExtended.TcpChannel)
            {
                return;
            }
            m_Connected = true;
            m_LoginForm.Login();
        }
    }
}
