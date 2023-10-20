using BaseFramework;
using BaseFramework.Event;
using BaseFramework.Network;
using GameProto;
using UnityBaseFramework.Runtime;
using ProcedureOwner = BaseFramework.Fsm.IFsm<BaseFramework.Procedure.IProcedureManager>;

namespace XGame
{
    public class ProcedureClientMode : ProcedureBase
    {
        private int? m_ClientModeFormSerialId = null;
        private ClientModeForm m_ClientModeForm = null;

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

            if (m_ClientModeFormSerialId == null || !GameEntry.UI.HasUIForm((int)m_ClientModeFormSerialId))
            {
                m_ClientModeFormSerialId = GameEntry.UI.OpenUIForm(UIFormId.ClientModeForm, this);
            }
            else
            {
                UIForm form = GameEntry.UI.GetUIForm((int)m_ClientModeFormSerialId);
                if (form != null)
                {
                    m_ClientModeForm = (ClientModeForm)form.Logic;
                    GameEntry.UI.RefocusUIForm(form);

                    OnAfterOpenUI();
                }
            }
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            GameEntry.Event.Unsubscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);

            if (m_ClientModeForm != null)
            {
                m_ClientModeForm.Close(isShutdown);
                m_ClientModeForm = null;
            }
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            //模拟器更新
            Simulator.Instance?.Update(elapseSeconds, realElapseSeconds);
        }

        private void OnOpenUIFormSuccess(object sender, GameEventArgs e)
        {
            OpenUIFormSuccessEventArgs ne = (OpenUIFormSuccessEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            m_ClientModeForm = (ClientModeForm)ne.UIForm.Logic;

            OnAfterOpenUI();
        }

        private void OnAfterOpenUI()
        {

        }

    }
}
