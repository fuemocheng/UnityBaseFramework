using UnityEngine;

namespace XGame
{
    public class PingMono : MonoBehaviour
    {
        private GUIStyle m_GUIStyle;

        private void Awake()
        {
            m_GUIStyle = new GUIStyle();
            m_GUIStyle.fontStyle = FontStyle.Normal;
            m_GUIStyle.normal.textColor = Color.red;
            m_GUIStyle.fontSize = 12;
        }

        private void Start()
        {

        }

        private void OnGUI()
        {
            if (Simulator.Instance == null)
            {
                return;
            }

            GUI.Label(new Rect(10, 0, 200, 50), $"Ping: {Simulator.Instance.PingVal}ms Dealy: {Simulator.Instance.DelayVal}ms", m_GUIStyle);
        }
    }
}
