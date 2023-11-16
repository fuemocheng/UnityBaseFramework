using Lockstep.Math;
using System;
using UnityBaseFramework.Runtime;
using UnityEngine;

namespace XGame
{
    public class InputMono : MonoBehaviour
    {
        [HideInInspector]
        private int FloorMask;
        private float CamRayLength = 100;

        //Input
        private LVector2 m_InputUV;
        private LVector2 m_MousePos;
        private bool m_IsFire;
        private bool m_IsSpeedUp;
        private int m_SkillId;

        void Start()
        {
            FloorMask = LayerMask.GetMask("Floor");
            m_InputUV = new LVector2(0, 0);
            m_MousePos = new LVector2(0, 0);
            m_IsFire = false;
            m_IsSpeedUp = false;
            m_SkillId = 0;
        }

        public void Update()
        {
            if (World.Instance != null && Camera.main != null)
            {
                float h = Input.GetAxisRaw("Horizontal");
                float v = Input.GetAxisRaw("Vertical");
                m_InputUV = new LVector2(h.ToLFloat(), v.ToLFloat());

                Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit floorHit;
                if (Physics.Raycast(camRay, out floorHit, CamRayLength, FloorMask))
                {
                    m_MousePos = floorHit.point.ToLVector2XZ();
                }

                m_IsFire = Input.GetMouseButtonUp(0);
                m_IsSpeedUp = !(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl));

                GameProto.Input currInput = GameEntry.Service.GetService<GameInputService>().CurrInput;
                currInput.InputH = m_InputUV.x._val;
                currInput.InputV = m_InputUV.y._val;
                currInput.MousePosX = m_MousePos.x._val;
                currInput.MousePosY = m_MousePos.y._val;
                currInput.IsFire = m_IsFire;
                currInput.IsSpeedUp = m_IsSpeedUp;
            }
        }
    }
}
