using BaseFramework.Event;
using System;
using UnityBaseFramework.Runtime;
using ProcedureOwner = BaseFramework.Fsm.IFsm<BaseFramework.Procedure.IProcedureManager>;

namespace XGame
{
    public class ProcedureLogin : ProcedureBase
    {
        private int? m_LoginFormSerialId = null;
        private LoginForm m_LoginForm = null;

        private bool m_Connected = false;
        private bool m_LoggedIn = false;
        private EUserState m_UserState = EUserState.Default;

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
            GameEntry.Event.Subscribe(SCLoginEventArgs.EventId, OnLoginResponse);

            //重置数据。
            m_UserState = EUserState.Default;
            m_Connected = false;
            m_LoggedIn = false;

            if (m_LoginFormSerialId == null || !GameEntry.UI.HasUIForm((int)m_LoginFormSerialId))
            {
                m_LoginFormSerialId = GameEntry.UI.OpenUIForm(UIFormId.LoginForm, this);
            }
            else
            {
                UIForm form = GameEntry.UI.GetUIForm((int)m_LoginFormSerialId);
                if (form != null)
                {
                    m_LoginForm = (LoginForm)form.Logic;
                    GameEntry.UI.RefocusUIForm(form);
                }
            }
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            GameEntry.Event.Unsubscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);
            GameEntry.Event.Unsubscribe(NetworkConnectedEventArgs.EventId, OnNetworkConnected);
            GameEntry.Event.Unsubscribe(SCLoginEventArgs.EventId, OnLoginResponse);

            if (m_LoginForm != null)
            {
                m_LoginForm.Close(isShutdown);
                m_LoginForm = null;
            }
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (m_Connected && m_LoggedIn)
            {
                // 登录或者重连的不同的处理。
                switch (m_UserState)
                {
                    case EUserState.LoggedIn:
                    case EUserState.NotReady:
                    case EUserState.Ready:
                        procedureOwner.SetData<VarInt32>("NextSceneId", GameEntry.Config.GetInt("Scene.Lobby"));
                        procedureOwner.SetData<VarInt32>("UserState", (int)m_UserState);
                        ChangeState<ProcedureChangeScene>(procedureOwner);
                        break;
                    case EUserState.Loading:
                        procedureOwner.SetData<VarInt32>("NextSceneId", GameEntry.Config.GetInt("Scene.Lobby"));
                        procedureOwner.SetData<VarInt32>("UserState", (int)m_UserState);
                        ChangeState<ProcedureChangeScene>(procedureOwner);
                        break;
                    case EUserState.Playing:
                        procedureOwner.SetData<VarInt32>("NextSceneId", GameEntry.Config.GetInt("Scene.Map01"));
                        procedureOwner.SetData<VarInt32>("UserState", (int)m_UserState);
                        ChangeState<ProcedureChangeScene>(procedureOwner);
                        break;
                    default:
                        break;
                }
            }
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
            m_LoginForm?.Login();
        }

        private void OnLoginResponse(object sender, GameEventArgs e)
        {
            SCLoginEventArgs ne = (SCLoginEventArgs)e;

            if (ne.RetCode == (int)EErrorCode.IncorrectPassword)
            {
                m_LoggedIn = false;
                // TODO: Show Tips
                Log.Error("Password incorrect.");
                return;
            }

            if (ne.RetCode == (int)EErrorCode.Success)
            {
                m_LoggedIn = true;
                m_UserState = (EUserState)ne.UserState;
            }
        }
    }
}
