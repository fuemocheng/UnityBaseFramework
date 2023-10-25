using UnityEngine;
using UnityBaseFramework.Runtime;
using UnityEngine.UI;
using BaseFramework.Network;
using GameProto;
using BaseFramework;
using System.IO;

namespace XGame
{
    public class ClientModeForm : UGuiForm
    {
        private ProcedureClientMode m_ProcedureClientMode = null;

        private GameObject m_Menu = null;
        private GameObject m_SimulatePanel = null;
        private GameObject m_PlayVideoPanel = null;

        //Menu
        private Button m_BtnSimulateMode = null;
        private Button m_BtnPlayVideoMode = null;
        private Button m_BtnQuit = null;

        //ClientSimulate
        private Button m_BtnStartSimulate = null;
        private Button m_BtnPauseSimulate = null;
        private Button m_BtnResumeSimulate = null;

        //PlayVideo
        private Button m_BtnReadRecord = null;
        private Button m_BtnStartVideo = null;
        private Button m_BtnPauseVideo = null;
        private Button m_BtnResumeVideo = null;
        private Text m_MaxTickText = null;
        private InputField m_InputFrameIndex = null;
        private Button m_BtnJumpTo = null;
        private Button m_BtnPreFrame = null;
        private Button m_BtnNextFrame = null;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);

            m_Menu = transform.Find("Content/Menu").gameObject;
            m_SimulatePanel = transform.Find("Content/SimulatePanel").gameObject;
            m_PlayVideoPanel = transform.Find("Content/PlayVideoPanel").gameObject;

            //Menu
            m_BtnSimulateMode = transform.Find("Content/Menu/BtnSimulateMode").GetComponent<Button>();
            m_BtnPlayVideoMode = transform.Find("Content/Menu/BtnPlayVideoMode").GetComponent<Button>();
            m_BtnQuit = transform.Find("Content/BtnQuit").GetComponent<Button>();
            m_BtnSimulateMode.onClick.AddListener(OnClickBtnSimulateMode);
            m_BtnPlayVideoMode.onClick.AddListener(OnClickBtnPlayVideoMode);
            m_BtnQuit.onClick.AddListener(OnClickQuit);

            //ClientSimulate
            m_BtnStartSimulate = transform.Find("Content/SimulatePanel/BtnStartSimulate").GetComponent<Button>();
            m_BtnPauseSimulate = transform.Find("Content/SimulatePanel/BtnPauseSimulate").GetComponent<Button>();
            m_BtnResumeSimulate = transform.Find("Content/SimulatePanel/BtnResumeSimulate").GetComponent<Button>();
            m_BtnStartSimulate.onClick.AddListener(OnClickStartSimulate);
            m_BtnPauseSimulate.onClick.AddListener(OnClickPauseSimulate);
            m_BtnResumeSimulate.onClick.AddListener(OnClickResumeSimulate);


            //PlayVideo
            m_BtnReadRecord = transform.Find("Content/PlayVideoPanel/BtnReadRecord").GetComponent<Button>();
            m_BtnStartVideo = transform.Find("Content/PlayVideoPanel/BtnStartVideo").GetComponent<Button>();
            m_BtnPauseVideo = transform.Find("Content/PlayVideoPanel/BtnPauseVideo").GetComponent<Button>();
            m_BtnResumeVideo = transform.Find("Content/PlayVideoPanel/BtnResumeVideo").GetComponent<Button>();
            m_MaxTickText = transform.Find("Content/PlayVideoPanel/MaxTickBg/MaxTick").GetComponent<Text>();
            m_InputFrameIndex = transform.Find("Content/PlayVideoPanel/InputFrameIndex").GetComponent<InputField>();
            m_BtnJumpTo = transform.Find("Content/PlayVideoPanel/BtnJumpTo").GetComponent<Button>();
            m_BtnPreFrame = transform.Find("Content/PlayVideoPanel/BtnPreFrame").GetComponent<Button>();
            m_BtnNextFrame = transform.Find("Content/PlayVideoPanel/BtnNextFrame").GetComponent<Button>();
            m_BtnReadRecord.onClick.AddListener(OnClickReadRecord);
            m_BtnStartVideo.onClick.AddListener(OnClickStartVideo);
            m_BtnPauseVideo.onClick.AddListener(OnClickPauseVideo);
            m_BtnResumeVideo.onClick.AddListener(OnClickResumeVideo);
            m_BtnJumpTo.onClick.AddListener(OnClickJumpTo);
            m_BtnPreFrame.onClick.AddListener(OnClickPreFrame);
            m_BtnNextFrame.onClick.AddListener(OnClickNextFrame);

            //Default
            m_Menu.gameObject.SetActive(true);
            m_SimulatePanel.gameObject.SetActive(false);
            m_PlayVideoPanel.gameObject.SetActive(false);
            m_BtnStartSimulate.gameObject.SetActive(true);

        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            m_ProcedureClientMode = (ProcedureClientMode)userData;
            if (m_ProcedureClientMode == null)
            {
                Log.Warning("ProcedureMenu is invalid when open MenuForm.");
                return;
            }
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            m_ProcedureClientMode = null;

            base.OnClose(isShutdown, userData);
        }

        protected override void OnResume()
        {
            base.OnResume();

            m_Menu.gameObject.SetActive(true);
            m_SimulatePanel.gameObject.SetActive(false);
            m_PlayVideoPanel.gameObject.SetActive(false);
            m_BtnStartSimulate.gameObject.SetActive(true);
        }

        #region Menu

        private void OnClickBtnSimulateMode()
        {
            m_Menu.gameObject.SetActive(false);
            m_SimulatePanel.gameObject.SetActive(true);
            m_PlayVideoPanel.gameObject.SetActive(false);

            m_ProcedureClientMode.OnClickSimulateMode();
        }

        private void OnClickBtnPlayVideoMode()
        {
            m_Menu.gameObject.SetActive(false);
            m_SimulatePanel.gameObject.SetActive(false);
            m_PlayVideoPanel.gameObject.SetActive(true);

            m_ProcedureClientMode.OnClickPlayVideoMode();
        }

        private void OnClickQuit()
        {
            m_ProcedureClientMode.OnQuit();
        }

        #endregion

        #region ClientSimulate

        private void OnClickStartSimulate()
        {
            m_BtnStartSimulate.gameObject.SetActive(false);
            m_ProcedureClientMode.StartClientSimulate();
        }

        private void OnClickPauseSimulate()
        {
            m_ProcedureClientMode.PauseClientSimulate();
        }

        private void OnClickResumeSimulate()
        {
            m_ProcedureClientMode.ResumeClientSimulate();
        }

        #endregion

        #region PlayVideo

        private void OnClickReadRecord()
        {
            m_ProcedureClientMode.OnReadRecord();
        }

        private void OnClickStartVideo()
        {
            m_ProcedureClientMode.OnStartPlayVideo();
        }

        private void OnClickPauseVideo()
        {
            m_ProcedureClientMode.OnPauseVideo();
        }

        private void OnClickResumeVideo()
        {
            m_ProcedureClientMode.OnResumeVideo();
        }

        private void OnClickJumpTo()
        {
            int targetTick;
            if (!int.TryParse(m_InputFrameIndex.text, out targetTick))
            {
                return;
            }
            m_ProcedureClientMode.OnJumpTo(targetTick);
        }

        private void OnClickPreFrame()
        {
            m_ProcedureClientMode.OnPreFrame();
        }

        private void OnClickNextFrame()
        {
            m_ProcedureClientMode.OnNextFrame();
        }

        public void SetMaxTick(int tick)
        {
            m_MaxTickText.text = $"MaxTick:{tick}";
        }

        public void SetCurrTick(int tick)
        {
            m_InputFrameIndex.text = tick.ToString();
        }
        #endregion
    }
}
