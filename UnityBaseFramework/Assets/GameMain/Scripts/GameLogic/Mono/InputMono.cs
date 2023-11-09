using Lockstep.Math;
using UnityBaseFramework.Runtime;
using UnityEngine;

namespace XGame
{
    public class InputMono : MonoBehaviour
    {
        [HideInInspector]
        public int FloorMask;
        public float CamRayLength = 100;

        //Input
        public LVector2 InputUV;
        public LVector2 MousePos;
        public int SkillId;

        void Start()
        {
            FloorMask = LayerMask.GetMask("Floor");
            InputUV = new LVector2(0, 0);
            MousePos = new LVector2(0, 0);
        }

        public void Update()
        {
            if (World.Instance != null)
            {
                float h = Input.GetAxisRaw("Horizontal");
                float v = Input.GetAxisRaw("Vertical");
                InputUV = new LVector2(h.ToLFloat(), v.ToLFloat());

                Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit floorHit;
                if (Physics.Raycast(camRay, out floorHit, CamRayLength, FloorMask))
                {
                    MousePos = floorHit.point.ToLVector2XZ();
                }

                GameProto.Input currInput = GameEntry.Service.GetService<GameInputService>().CurrInput;
                currInput.InputH = InputUV.x._val;
                currInput.InputV = InputUV.y._val;
                currInput.MousePosX = MousePos.x._val;
                currInput.MousePosY = MousePos.y._val;
            }
        }
    }
}
