using BaseFramework.Event;
using UnityBaseFramework.Runtime;
using ProcedureOwner = BaseFramework.Fsm.IFsm<BaseFramework.Procedure.IProcedureManager>;

namespace XGame
{
    public class ProcedureLobby : ProcedureBase
    {
        private LobbyForm m_LobbyForm = null;

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

            GameEntry.UI.OpenUIForm(UIFormId.LobbyForm, this);
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            GameEntry.Event.Unsubscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);

            if (m_LobbyForm != null)
            {
                m_LobbyForm.Close(isShutdown);
                m_LobbyForm = null;
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

            m_LobbyForm = (LobbyForm)ne.UIForm.Logic;
        }
    }
}
