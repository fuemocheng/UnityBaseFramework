using UnityEngine;

namespace XGame
{
    public class PingMono : MonoBehaviour
    {
        private void OnGUI()
        {
            if (Simulator.Instance == null)
            {
                return;
            }
            GUI.Label(new Rect(0, 0, 100, 100), $"Ping: {Simulator.Instance.PingVal}ms Dealy: {Simulator.Instance.DelayVal}ms ");
        }
    }
}
