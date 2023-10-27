using UnityEngine;

namespace XGame
{
    public class CameraFollow : MonoBehaviour
    {
        [Header("Group1:黑方 Group2:白方")]
        [Range(1, 2)]
        public int GroupIndex = 1;
        public Transform Target = null;

        public Vector3 GroupOneOffset = new Vector3(0, 20, -12);
        public Quaternion GroupOneRotation = Quaternion.Euler(55, 0, 0);

        public Vector3 GroupTwoOffset = new Vector3(0, 20, 12);
        public Quaternion GroupTwoRotation = Quaternion.Euler(55, 180, 0);

        public float CameraLerpTime = 0.5f;

        void Start()
        {

        }

        void Update()
        {
            if(Target == null)
            {
                if(World.Instance != null && World.MyPlayerTrans != null)
                {
                    Target = World.MyPlayerTrans as Transform;
                }
                return;
            }
            if (GroupIndex == 1)
            {
                transform.position = Vector3.Lerp(transform.position, Target.position + GroupOneOffset, CameraLerpTime);
                transform.rotation = GroupOneRotation;
            }
            else if (GroupIndex == 2)
            {
                transform.position = Vector3.Lerp(transform.position, Target.position + GroupTwoOffset, CameraLerpTime);
                transform.rotation = GroupTwoRotation;
            }
        }

        public void SetTarget(Transform target)
        {
            Target = target;
        }
    }
}
