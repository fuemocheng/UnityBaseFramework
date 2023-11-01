using BaseFramework;
using BaseFramework.Event;
using BaseFramework.Network;
using GameProto;
using System.IO;
using UnityBaseFramework.Runtime;
using ProcedureOwner = BaseFramework.Fsm.IFsm<BaseFramework.Procedure.IProcedureManager>;

namespace XGame
{
    public class ProcedureClientMode : ProcedureBase
    {
        private int? m_ClientModeFormSerialId = null;
        private ClientModeForm m_ClientModeForm = null;

        private bool m_BackToLogin = false;

        private enum EMode
        {
            Default,
            Simulate,
            PlayVideo,
        }

        private EMode m_Mode = EMode.Default;

        #region PlayVideo
        private SCGameStartInfo m_GameStartInfo = null;
        private SCServerFrame m_ServerFrame = null;
        #endregion

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

            m_Mode = EMode.Default;
            m_BackToLogin = false;

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

            if(m_BackToLogin)
            {
                // 返回Login。
                procedureOwner.SetData<VarInt32>("NextSceneId", GameEntry.Config.GetInt("Scene.Login"));
                ChangeState<ProcedureChangeScene>(procedureOwner);
                return;
            }

            switch (m_Mode)
            {
                case EMode.Simulate:
                    Simulator.Instance?.Update(elapseSeconds, realElapseSeconds);
                    break;
                case EMode.PlayVideo:
                    Simulator.Instance?.UpdateVideo();
                    break;
                default:
                    break;
            }
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

        #region Menu

        public void OnClickSimulateMode()
        {
            m_Mode = EMode.Simulate;
        }

        public void OnClickPlayVideoMode()
        {
            //m_Mode = EMode.PlayVideo;
        }

        public void OnQuit()
        {
            m_BackToLogin = true;
        }

        #endregion

        #region ClientSimulate

        public void StartClientSimulate()
        {
            //初始化时间戳。
            GameTime.InitStartTimeStamp();

            //游戏初始化数据
            UserGameInfo userGameInfo = new UserGameInfo();
            userGameInfo.LocalId = 0;
            userGameInfo.UserState = (int)EUserState.Playing;
            userGameInfo.Camp = (int)ECamp.White;
            userGameInfo.User = new GameProto.User();
            userGameInfo.User.UserId = 10001;
            userGameInfo.User.UserName = "10001";

            m_GameStartInfo = new();
            m_GameStartInfo.RoomId = 10001;
            m_GameStartInfo.MapId = 0;
            m_GameStartInfo.LocalId = 0;
            m_GameStartInfo.UserGameInfos.Add(userGameInfo);

            Simulator simulator = new Simulator();
            simulator.IsClientMode = true;
            simulator.IsVideoMode = false;
            simulator.Start();
            simulator.OnGameCreate(60,
                m_GameStartInfo.MapId,
                0,  //模拟时，选择 LocalId 为 0； 
                m_GameStartInfo.UserCount,
                m_GameStartInfo.UserGameInfos);

            GameEntry.Map?.AddAllMapColliderProxy();

            simulator.StartSimulate();

            //模拟发送第一帧
            Input input = GameEntry.Service.GetService<GameInputService>().CurrInput;
            InputFrame inputFrame = new();
            inputFrame.Tick = 0;
            inputFrame.LocalId = 0;
            inputFrame.Input = input;

            ServerFrame serverFrame = new();
            serverFrame.Tick = 0;
            serverFrame.InputFrames.Add(inputFrame);

            simulator.OnServerFrame(new System.Collections.Generic.List<ServerFrame> { serverFrame });

            GameEntry.Service.GetService<ConstStateService>().IsClientMode = true;
        }

        public void PauseClientSimulate()
        {
            GameEntry.Service.GetService<CommonStateService>().IsPause = true;
        }

        public void ResumeClientSimulate()
        {
            GameEntry.Service.GetService<CommonStateService>().IsPause = false;
        }

        #endregion

        #region PlayVideo

        public void OnReadRecord()
        {
            string recordPath = Utility.Text.Format("{0}{1}", Directory.GetCurrentDirectory(), "/Assets/GameMain/Record/TestRecord.bytes");
            recordPath = recordPath.Replace("\\", "/");
            if (!File.Exists(recordPath))
            {
                Log.Error("ClickReadRecord Error: {0} is not exist.", recordPath);
                return;
            }

            m_GameStartInfo = new SCGameStartInfo();
            m_ServerFrame = new SCServerFrame();
            RecordUtility.ReadRecord(recordPath, ref m_GameStartInfo, ref m_ServerFrame);

            m_ClientModeForm.SetMaxTick(m_ServerFrame.ServerFrames.Capacity);
            m_ClientModeForm.SetCurrTick(0);

            InitPlayVideo();
        }

        private void InitPlayVideo()
        {
            if (m_GameStartInfo == null || m_ServerFrame == null)
            {
                return;
            }

            //初始化时间戳。
            GameTime.InitStartTimeStamp();

            Simulator simulator = new Simulator();
            simulator.IsClientMode = false;
            simulator.IsVideoMode = true;
            simulator.Start();
            simulator.OnGameCreate(60,
                m_GameStartInfo.MapId,
                0,  //模拟时，选择 LocalId 为 0； 
                m_GameStartInfo.UserCount,
                m_GameStartInfo.UserGameInfos);

            GameEntry.Map?.AddAllMapColliderProxy();

            simulator.SetVideoFrame(m_ServerFrame);

            simulator.StartSimulate();

            GameEntry.Service.GetService<ConstStateService>().IsVideoMode = true;
        }

        public void OnStartPlayVideo()
        {
            m_Mode = EMode.PlayVideo;
        }

        public void OnPauseVideo()
        {
            GameEntry.Service.GetService<CommonStateService>().IsPause = true;
        }

        public void OnResumeVideo()
        {
            GameEntry.Service.GetService<CommonStateService>().IsPause = false;
        }

        public void OnJumpTo(int targetTick)
        {
            Simulator.Instance?.JumpTo(targetTick);
        }

        public void OnPreFrame()
        {
            if (World.Instance == null)
            {
                return;
            }
            Simulator.Instance?.JumpTo(World.Instance.Tick - 5);
        }

        public void OnNextFrame()
        {
            if (World.Instance == null)
            {
                return;
            }
            Simulator.Instance?.JumpTo(World.Instance.Tick + 5);
        }

        #endregion


    }
}
