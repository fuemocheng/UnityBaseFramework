using BaseFramework;
using System.Collections;
using System.Collections.Generic;
using UnityBaseFramework.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace XGame
{
    public class DialogForm : UGuiForm
    {
        private Text m_TitleText = null;
        private Text m_MessageText = null;
        private List<GameObject> m_ModeObjects = null;
        private List<Button> m_ConfirmButtons = null;
        private List<Button> m_CancelButtons = null;
        private List<Button> m_OtherButtons = null;
        private List<Text> m_ConfirmTexts = null;
        private List<Text> m_CancelTexts = null;
        private List<Text> m_OtherTexts = null;

        private int m_DialogMode = 1;
        private bool m_PauseGame = false;
        private object m_UserData = null;
        private BaseFrameworkAction<object> m_OnClickConfirm = null;
        private BaseFrameworkAction<object> m_OnClickCancel = null;
        private BaseFrameworkAction<object> m_OnClickOther = null;

        public int DialogMode
        {
            get
            {
                return m_DialogMode;
            }
        }

        public bool PauseGame
        {
            get
            {
                return m_PauseGame;
            }
        }

        public object UserData
        {
            get
            {
                return m_UserData;
            }
        }

        public void OnConfirmButtonClick()
        {
            Close();

            if (m_OnClickConfirm != null)
            {
                m_OnClickConfirm(m_UserData);
            }
        }

        public void OnCancelButtonClick()
        {
            Close();

            if (m_OnClickCancel != null)
            {
                m_OnClickCancel(m_UserData);
            }
        }

        public void OnOtherButtonClick()
        {
            Close();

            if (m_OnClickOther != null)
            {
                m_OnClickOther(m_UserData);
            }
        }

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);

            // 初始化UI。
            m_TitleText = transform.Find("Mask/Background/TitleBackground/Title").GetComponent<Text>();
            m_MessageText = transform.Find("Mask/Background/Message").GetComponent<Text>();

            GameObject buttonGroup1 = transform.Find("Mask/Background/ButtonGroup1").gameObject;
            GameObject buttonGroup2 = transform.Find("Mask/Background/ButtonGroup2").gameObject;
            GameObject buttonGroup3 = transform.Find("Mask/Background/ButtonGroup3").gameObject;
            Button buttonConfirm1 = buttonGroup1.transform.Find("Confirm").GetComponent<Button>();
            Button buttonConfirm2 = buttonGroup2.transform.Find("Confirm").GetComponent<Button>();
            Button buttonConfirm3 = buttonGroup3.transform.Find("Confirm").GetComponent<Button>();
            Button buttonCancel2 = buttonGroup2.transform.Find("Cancel").GetComponent<Button>();
            Button buttonCancel3 = buttonGroup3.transform.Find("Cancel").GetComponent<Button>();
            Button buttonOther3 = buttonGroup3.transform.Find("Other").GetComponent<Button>();
            Text textConfirm1 = buttonConfirm1.transform.Find("Text").GetComponent<Text>();
            Text textConfirm2 = buttonConfirm2.transform.Find("Text").GetComponent<Text>();
            Text textConfirm3 = buttonConfirm3.transform.Find("Text").GetComponent<Text>();
            Text textCancel2 = buttonCancel2.transform.Find("Text").GetComponent<Text>();
            Text textCancel3 = buttonCancel3.transform.Find("Text").GetComponent<Text>();
            Text textOther3 = buttonOther3.transform.Find("Text").GetComponent<Text>();

            m_ModeObjects = new()
            {
                buttonGroup1,
                buttonGroup2,
                buttonGroup3
            };

            m_ConfirmButtons = new()
            {
                buttonConfirm1,
                buttonConfirm2,
                buttonConfirm3,
            };

            m_CancelButtons = new()
            {
                buttonCancel2,
                buttonCancel3,
            };

            m_OtherButtons = new()
            {
                buttonOther3,
            };

            m_ConfirmTexts = new()
            {
                textConfirm1,
                textConfirm2,
                textConfirm3,
            };

            m_CancelTexts = new()
            {
                textCancel2,
                textCancel3,
            };

            m_OtherTexts = new()
            {
                textOther3,
            };

            foreach (var button in m_ConfirmButtons)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(OnConfirmButtonClick);
            }

            foreach (var button in m_CancelButtons)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(OnCancelButtonClick);
            }

            foreach (var button in m_OtherButtons)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(OnOtherButtonClick);
            }
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            DialogParams dialogParams = (DialogParams)userData;
            if (dialogParams == null)
            {
                Log.Warning("DialogParams is invalid.");
                return;
            }

            m_DialogMode = dialogParams.Mode;
            RefreshDialogMode();

            m_TitleText.text = dialogParams.Title;
            m_MessageText.text = dialogParams.Message;

            m_PauseGame = dialogParams.PauseGame;
            RefreshPauseGame();

            m_UserData = dialogParams.UserData;

            RefreshConfirmText(dialogParams.ConfirmText);
            m_OnClickConfirm = dialogParams.OnClickConfirm;

            RefreshCancelText(dialogParams.CancelText);
            m_OnClickCancel = dialogParams.OnClickCancel;

            RefreshOtherText(dialogParams.OtherText);
            m_OnClickOther = dialogParams.OnClickOther;
        }


        protected override void OnClose(bool isShutdown, object userData)
        {
            if (m_PauseGame)
            {
                GameEntry.Base.ResumeGame();
            }

            m_DialogMode = 1;
            m_TitleText.text = string.Empty;
            m_MessageText.text = string.Empty;
            m_PauseGame = false;
            m_UserData = null;

            RefreshConfirmText(string.Empty);
            m_OnClickConfirm = null;

            RefreshCancelText(string.Empty);
            m_OnClickCancel = null;

            RefreshOtherText(string.Empty);
            m_OnClickOther = null;

            base.OnClose(isShutdown, userData);
        }

        private void RefreshDialogMode()
        {
            for (int i = 1; i <= m_ModeObjects.Count; i++)
            {
                m_ModeObjects[i - 1].SetActive(i == m_DialogMode);
            }
        }

        private void RefreshPauseGame()
        {
            if (m_PauseGame)
            {
                GameEntry.Base.PauseGame();
            }
        }

        private void RefreshConfirmText(string confirmText)
        {
            if (string.IsNullOrEmpty(confirmText))
            {
                confirmText = GameEntry.Localization.GetString("Dialog.ConfirmButton");
            }

            for (int i = 0; i < m_ConfirmTexts.Count; i++)
            {
                m_ConfirmTexts[i].text = confirmText;
            }
        }

        private void RefreshCancelText(string cancelText)
        {
            if (string.IsNullOrEmpty(cancelText))
            {
                cancelText = GameEntry.Localization.GetString("Dialog.CancelButton");
            }

            for (int i = 0; i < m_CancelTexts.Count; i++)
            {
                m_CancelTexts[i].text = cancelText;
            }
        }

        private void RefreshOtherText(string otherText)
        {
            if (string.IsNullOrEmpty(otherText))
            {
                otherText = GameEntry.Localization.GetString("Dialog.OtherButton");
            }

            for (int i = 0; i < m_OtherTexts.Count; i++)
            {
                m_OtherTexts[i].text = otherText;
            }
        }
    }
}
