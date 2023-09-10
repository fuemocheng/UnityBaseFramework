using UnityEngine;
using UnityBaseFramework.Runtime;

namespace XGame
{
    public class LobbyForm : UGuiForm
    {
        private ProcedureLobby m_ProcedureLobby = null;

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            m_ProcedureLobby = (ProcedureLobby)userData;
            if (m_ProcedureLobby == null)
            {
                Log.Warning("ProcedureMenu is invalid when open MenuForm.");
                return;
            }
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            m_ProcedureLobby = null;

            base.OnClose(isShutdown, userData);
        }
    }
}
