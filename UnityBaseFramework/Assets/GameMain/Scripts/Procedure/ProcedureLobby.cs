using BaseFramework.Event;
using UnityBaseFramework.Runtime;
using ProcedureOwner = BaseFramework.Fsm.IFsm<BaseFramework.Procedure.IProcedureManager>;

namespace XGame
{
    public class ProcedureLobby : ProcedureBase
    {
        private LobbyForm m_LobbyForm = null;

        private bool m_IsAllReady = false;

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
            GameEntry.Event.Subscribe(SCReadyEventArgs.EventId, OnReadyResponse);

            GameEntry.UI.OpenUIForm(UIFormId.LobbyForm, this);
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            GameEntry.Event.Unsubscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);
            GameEntry.Event.Unsubscribe(SCReadyEventArgs.EventId, OnReadyResponse);

            if (m_LobbyForm != null)
            {
                m_LobbyForm.Close(isShutdown);
                m_LobbyForm = null;
            }

            m_IsAllReady = false;
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            // 所有人都准备好了，加载游戏场景。
            if (m_IsAllReady)
            {
                // 切换场景，然后发送开始游戏。
                procedureOwner.SetData<VarInt32>("NextSceneId", GameEntry.Config.GetInt("Scene.Map01"));
                ChangeState<ProcedureChangeScene>(procedureOwner);
            }
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

        private void OnReadyResponse(object sender, GameEventArgs e)
        {
            SCReadyEventArgs ne = (SCReadyEventArgs)e;

            m_LobbyForm.SetWaitingCount(ne.CurrCount);

            if (ne.CurrCount >= CommonDefinitions.MaxRoomMemberCount)
            {
                m_IsAllReady = true;
            }
        }
    }
}
