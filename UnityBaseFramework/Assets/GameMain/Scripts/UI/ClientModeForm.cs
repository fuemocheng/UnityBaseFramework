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

            m_BtnReadRecord = transform.Find("Content/BtnReadRecord").GetComponent<Button>();
            m_MaxTickText = transform.Find("Content/MaxTickBg/MaxTick").GetComponent<Text>();
            m_InputFrameIndex = transform.Find("Content/InputFrameIndex").GetComponent<InputField>();
            m_BtnJumpTo = transform.Find("Content/BtnJumpTo").GetComponent<Button>();

            m_BtnStart = transform.Find("Content/BtnStart").GetComponent<Button>();
            m_BtnPause = transform.Find("Content/BtnPause").GetComponent<Button>();
            m_BtnResume = transform.Find("Content/BtnResume").GetComponent<Button>();
            m_BtnQuit = transform.Find("Content/BtnQuit").GetComponent<Button>();


            m_BtnReadRecord.onClick.AddListener(OnClickReadRecord);
            m_BtnJumpTo.onClick.AddListener(OnClickJumpTo);
            m_BtnStart.onClick.AddListener(OnClickStart);
            m_BtnPause.onClick.AddListener(OnClickPause);
            m_BtnResume.onClick.AddListener(OnClickResume);
            m_BtnQuit.onClick.AddListener(OnClickQuit);
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
