using UnityEngine;
using UnityBaseFramework.Runtime;
using TMPro;
using UnityEngine.UI;
using BaseFramework.Network;
using GameProto;
using BaseFramework;

namespace XGame
{
    public class MainUIForm : UGuiForm
    {
        private ProcedureLobby m_ProcedureLobby = null;


        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            //m_ProcedureLobby = (ProcedureLobby)userData;
            //if (m_ProcedureLobby == null)
            //{
            //    Log.Warning("ProcedureMenu is invalid when open MenuForm.");
            //    return;
            //}
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            //m_ProcedureLobby = null;

            base.OnClose(isShutdown, userData);
        }
    }
}
