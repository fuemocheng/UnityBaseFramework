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

        private int m_MaxTick = 0;

        private SCGameStartInfo m_GameStartInfo = null;
        private SCServerFrame m_ServerFrame = null;

        private GameObject m_Menu = null;
        private GameObject m_ClientSimulate = null;
        private GameObject m_VideoMode = null;

        private Button m_BtnClientSimulate = null;
        private Button m_BtnVideoMode = null;

        private Button m_BtnReadRecord = null;
        private Text m_MaxTickText = null;
        private InputField m_InputFrameIndex = null;
        private Button m_BtnJumpTo = null;

        private Button m_BtnStart = null;
        private Button m_BtnPause = null;
        private Button m_BtnResume = null;
        private Button m_BtnQuit = null;


        protected override void OnInit(object userData)
        {
            base.OnInit(userData);

            m_Menu = transform.Find("Content/Menu").gameObject;
            m_ClientSimulate = transform.Find("Content/ClientSimulate").gameObject;
            m_VideoMode = transform.Find("Content/VideoMode").gameObject;

            m_BtnClientSimulate = transform.Find("Content/Menu/BtnClientSimulate").GetComponent<Button>();
            m_BtnVideoMode = transform.Find("Content/Menu/BtnVideoMode").GetComponent<Button>();

            m_BtnClientSimulate.onClick.AddListener(OnClickClientSimulate);
            m_BtnVideoMode.onClick.AddListener(OnClickVideoMode);

            m_BtnReadRecord = transform.Find("Content/VideoMode/BtnReadRecord").GetComponent<Button>();
            m_MaxTickText = transform.Find("Content/VideoMode/MaxTickBg/MaxTick").GetComponent<Text>();
            m_InputFrameIndex = transform.Find("Content/VideoMode/InputFrameIndex").GetComponent<InputField>();
            m_BtnJumpTo = transform.Find("Content/VideoMode/BtnJumpTo").GetComponent<Button>();

            m_BtnStart = transform.Find("Content/VideoMode/BtnStart").GetComponent<Button>();
            m_BtnPause = transform.Find("Content/VideoMode/BtnPause").GetComponent<Button>();
            m_BtnResume = transform.Find("Content/VideoMode/BtnResume").GetComponent<Button>();
            m_BtnQuit = transform.Find("Content/VideoMode/BtnQuit").GetComponent<Button>();


            m_BtnReadRecord.onClick.AddListener(OnClickReadRecord);
            m_BtnJumpTo.onClick.AddListener(OnClickJumpTo);
            m_BtnStart.onClick.AddListener(OnClickStart);
            m_BtnPause.onClick.AddListener(OnClickPause);
            m_BtnResume.onClick.AddListener(OnClickResume);
            m_BtnQuit.onClick.AddListener(OnClickQuit);

            m_Menu.gameObject.SetActive(true);
            m_ClientSimulate.gameObject.SetActive(false);
            m_VideoMode.gameObject.SetActive(false);
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

            m_MaxTick = 0;
            m_GameStartInfo = null;
            m_ServerFrame = null;

            m_Menu.gameObject.SetActive(true);
            m_ClientSimulate.gameObject.SetActive(false);
            m_VideoMode.gameObject.SetActive(false);
        }

        private void OnClickClientSimulate()
        {
            m_Menu.gameObject.SetActive(false);
            m_ClientSimulate.gameObject.SetActive(true);
            m_VideoMode.gameObject .SetActive(false);
        }

        private void OnClickVideoMode()
        {
            m_Menu.gameObject.SetActive(false);
            m_ClientSimulate.gameObject.SetActive(false);
            m_VideoMode.gameObject.SetActive(true);
        }

        private void OnClickReadRecord()
        {
            string recordPath = Utility.Text.Format("{0}{1}", Directory.GetCurrentDirectory(), "/Assets/GameMain/Record/TestRecord.record");
            recordPath = recordPath.Replace("\\", "/");
            if (!File.Exists(recordPath))
            {
                Log.Error("ClickReadRecord Error: {0} is not exist.", recordPath);
                return;
            }

            m_GameStartInfo = new SCGameStartInfo();
            m_ServerFrame = new SCServerFrame();
            RecordUtility.ReadRecord(recordPath, ref m_GameStartInfo, ref m_ServerFrame);

            m_MaxTick = m_ServerFrame.ServerFrames.Count;
            m_MaxTickText.text = $"MaxTick:{m_MaxTick}";

        }

        private void OnClickJumpTo()
        {
        }

        private void OnClickStart()
        {
            if(m_GameStartInfo == null || m_ServerFrame == null)
            {
                return;
            }

            //初始化时间戳。
            GameTime.InitStartTimeStamp();

            Simulator simulator = new Simulator();
            simulator.Start();
            simulator.OnGameCreate(60,
                m_GameStartInfo.MapId,
                0,  //模拟时，选择 LocalId 为 0； 
                m_GameStartInfo.UserCount,
                m_GameStartInfo.UserGameInfos);

            //将所有的帧数据添加进模拟器。
            simulator.OnServerFrame(m_ServerFrame.ServerFrames);

            simulator.StartSimulate();
        }

        private void OnClickPause()
        {
        }

        private void OnClickResume()
        {
        }

        private void OnClickQuit()
        {
        }

    }
}
