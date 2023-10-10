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
            GameEntry.Event.Subscribe(SCLoadingProgressEventArgs.EventId, OnLoadingProgressResponse);
            GameEntry.Event.Subscribe(SCServerFrameEventArgs.EventId, OnServerFrameResponse);

            GameEntry.UI.OpenUIForm(UIFormId.MainUIForm, this);

            // 场景已经加载完成，发送加载进度。

            SendLoadingProgress();
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            GameEntry.Event.Unsubscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);
            GameEntry.Event.Unsubscribe(SCLoadingProgressEventArgs.EventId, OnLoadingProgressResponse);
            GameEntry.Event.Unsubscribe(SCServerFrameEventArgs.EventId, OnServerFrameResponse);


            if (m_MainUIForm != null)
            {
                m_MainUIForm.Close(isShutdown);
                m_MainUIForm = null;
            }
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            //模拟器更新
            Simulator.Instance.Update(elapseSeconds, realElapseSeconds);
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


        private void SendLoadingProgress()
        {
            INetworkChannel tcpChannel = GameEntry.NetworkExtended.TcpChannel;
            if (tcpChannel == null)
            {
                Log.Error("Cannot SendLoadingProgress, tcpChannel is null.");
                return;
            }
            //发送进度
            CSLoadingProgress csLoadingProgress = ReferencePool.Acquire<CSLoadingProgress>();
            csLoadingProgress.Progress = 100;
            tcpChannel.Send(csLoadingProgress);
        }

        private void OnLoadingProgressResponse(object sender, GameEventArgs e)
        {
            SCLoadingProgressEventArgs scLoadingProgressEventArgs = (SCLoadingProgressEventArgs)e;
            if (scLoadingProgressEventArgs == null)
            {
                return;
            }

            if(scLoadingProgressEventArgs.AllProgress < 100)
            {
                //TODO: Loading 界面 设置加载进度。
                Log.Info($"OnLoadingProgressResponse AllProgress:{scLoadingProgressEventArgs.AllProgress}");
            }
            else
            {
                // 所有客户端都完成了准备工作。
                // 开始游戏，发送帧数据
                Log.Info("Start Game.");

                Simulator.Instance.StartSimulate();
            }
        }

        private void OnServerFrameResponse(object sender, GameEventArgs e)
        {
            SCServerFrameEventArgs scServerFrameEventArgs = (SCServerFrameEventArgs)e;
            if (scServerFrameEventArgs == null)
            {
                return;
            }
            //Log.Error("ProcedureMap:OnServerFrameResponse.Tick {0}", scServerFrameEventArgs.ServerFrames[scServerFrameEventArgs.ServerFrames.Count - 1].Tick);
            Simulator.Instance.OnServerFrame(scServerFrameEventArgs.ServerFrames);
        }
    }
}
