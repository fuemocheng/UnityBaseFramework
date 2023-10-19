using UnityBaseFramework.Runtime;
using UnityEngine.UI;

namespace XGame
{
    public class MainUIForm : UGuiForm
    {
        private ProcedureMap m_ProcedureMap = null;

        private Text m_GameState = null;
        private Button m_BtnSetting = null;
        private Button m_BtnSettingMask = null;
        private Button m_BtnPause = null;
        private Button m_BtnResume = null;
        private Button m_BtnQuit = null;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);

            m_GameState = transform.Find("Content/Center/BtnSettingMask/GameState").GetComponent<Text>();
            m_GameState.text = "";
            m_BtnSetting = transform.Find("Content/Center/BtnSetting").GetComponent<Button>();
            m_BtnSetting.onClick.AddListener(OnClickBtnSetting);
            m_BtnSettingMask = transform.Find("Content/Center/BtnSettingMask").GetComponent<Button>();
            m_BtnSettingMask.onClick.AddListener(OnClickBtnSettingMask);
            m_BtnPause = transform.Find("Content/Center/BtnSettingMask/BtnPause").GetComponent<Button>();
            m_BtnPause.onClick.AddListener(OnClickBtnPause);
            m_BtnResume = transform.Find("Content/Center/BtnSettingMask/BtnResume").GetComponent<Button>();
            m_BtnResume.onClick.AddListener(OnClickBtnResume);
            m_BtnQuit = transform.Find("Content/Center/BtnSettingMask/BtnQuit").GetComponent<Button>();
            m_BtnQuit.onClick.AddListener(OnClickBtnQuit);

            m_BtnSettingMask.gameObject.SetActive(false);
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            m_ProcedureMap = (ProcedureMap)userData;
            if (m_ProcedureMap == null)
            {
                Log.Warning("ProcedureMap is invalid when open MenuForm.");
                return;
            }
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            m_ProcedureMap = null;

            base.OnClose(isShutdown, userData);
        }

        protected override void OnResume()
        {
            base.OnResume();

            m_BtnSettingMask.gameObject.SetActive(false);
        }

        private void OnClickBtnSetting()
        {
            m_BtnSettingMask.gameObject.SetActive(true);
        }

        private void OnClickBtnSettingMask()
        {
            m_BtnSettingMask.gameObject.SetActive(false);
        }

        private void OnClickBtnPause()
        {
            m_ProcedureMap.Pause();
        }

        private void OnClickBtnResume()
        {
            m_ProcedureMap.Resume();
        }

        private void OnClickBtnQuit()
        {
            m_ProcedureMap.Quit();
        }
    }
}
