using UnityEngine;

namespace XGame
{
    public class TestMono : MonoBehaviour
    {
        void Start()
        {

        }

        void Update()
        {

        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (World.Instance != null)
            {
                PhysicSystem.Instance?.OnDrawGizmos();
            }

        }
#endif
    }
}
