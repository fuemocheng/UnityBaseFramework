using Lockstep.Math;
using UnityBaseFramework.Runtime;
using UnityEngine;

namespace XGame
{
    public class InputMono : MonoBehaviour
    {
        [HideInInspector] public int floorMask;
        public float camRayLength = 100;

        public bool hasHitFloor;
        public LVector2 mousePos;
        public LVector2 inputUV;
        public bool isInputFire;
        public int skillId;
        public bool isSpeedUp;

        void Start()
        {
            floorMask = LayerMask.GetMask("Floor");
        }

        public void Update()
        {
            if (World.Instance != null)
            {
                float h = Input.GetAxisRaw("Horizontal");
                float v = Input.GetAxisRaw("Vertical");
                inputUV = new LVector2(h.ToLFloat(), v.ToLFloat());

                //hasHitFloor = Input.GetMouseButtonDown(1);
                //if (hasHitFloor)
                //{
                //    Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                //    RaycastHit floorHit;
                //    if (Physics.Raycast(camRay, out floorHit, camRayLength, floorMask))
                //    {
                //        mousePos = floorHit.point.ToLVector2XZ();
                //    }
                //}

                if (h != 0 || v != 0)
                {
                    Log.Error("Raw:" + h + " " + v);
                    Log.Error("Trans:" + inputUV.x._val + " " + inputUV.y._val);
                }

                GameProto.Input currInput = GameEntry.Service.GetService<GameInputService>().CurrInput;
                currInput.InputH = inputUV.x._val;
                currInput.InputV = inputUV.y._val;

                //GameInputService.CurGameInput = new PlayerInput()
                //{
                //    mousePos = mousePos,
                //    inputUV = inputUV,
                //    isInputFire = isInputFire,
                //    skillId = skillId,
                //    isSpeedUp = isSpeedUp,
                //};
            }
        }
    }
}
