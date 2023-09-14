using BaseFramework;
using BaseFramework.Event;
using BaseFramework.Network;
using GameProto;
using UnityBaseFramework.Runtime;
using ProcedureOwner = BaseFramework.Fsm.IFsm<BaseFramework.Procedure.IProcedureManager>;

namespace XGame
{
    public class ProcedureMap : ProcedureBase
    {
        private MainUIForm m_MainUIForm = null;

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
            GameEntry.Event.Subscribe(SCStartEventArgs.EventId, OnStartResponse);

            GameEntry.UI.OpenUIForm(UIFormId.MainUIForm, this);

            // 场景已经加载完成，发送开始游戏。
            // TODO: 加载角色，加载完发送开始游戏的消息。
            SendStartGame();
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            GameEntry.Event.Unsubscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);
            GameEntry.Event.Unsubscribe(SCStartEventArgs.EventId, OnStartResponse);

            if (m_MainUIForm != null)
            {
                m_MainUIForm.Close(isShutdown);
                m_MainUIForm = null;
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

            m_MainUIForm = (MainUIForm)ne.UIForm.Logic;
        }


        private void SendStartGame()
        {
            INetworkChannel tcpChannel = GameEntry.NetworkExtended.TcpChannel;
            if (tcpChannel == null)
            {
                Log.Error("Cannot Start, tcpChannel is null.");
                return;
            }
            //开始游戏
            CSStart csStart = ReferencePool.Acquire<CSStart>();
            tcpChannel.Send(csStart);
        }

        private void OnStartResponse(object sender, GameEventArgs e)
        {
            SCStartEventArgs ne = (SCStartEventArgs)e;

            // 所有客户端都完成了准备工作。
            // 开始游戏。
            Log.Error("Start Game.");
        }
    }
}
