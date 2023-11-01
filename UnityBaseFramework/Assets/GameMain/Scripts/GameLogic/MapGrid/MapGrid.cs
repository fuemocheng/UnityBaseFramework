using Lockstep.Collision2D;
using Lockstep.ECS.ECDefine;
using Lockstep.Math;
using UnityBaseFramework.Runtime;
using UnityEngine;

namespace XGame
{
    public class MapGrid : MonoBehaviour
    {
        private Renderer m_Renderer;
        private MaterialPropertyBlock m_MaterialPropertyBlock;

        public EColliderLayer ColliderLayer = EColliderLayer.Static;

        private ColliderProxy m_ColliderProxy = null;

        private Color m_ColorBlack = new Color(70 / 255f, 70 / 255f, 70 / 255f);
        private Color m_ColorWhite = new Color(255 / 255f, 255 / 255f, 255 / 255f);

        private int m_LastChangeValue = 0;
        private const float MaxChangeTick = 150f;

        void Start()
        {
            m_Renderer = GetComponent<Renderer>();
            m_MaterialPropertyBlock = new MaterialPropertyBlock();

            //地图打开的时候预创建。
            m_ColliderProxy = CreateColliderProxy();

            GameEntry.Map?.AddMapGrid(this);
        }

        void Update()
        {
            if (World.Instance == null || World.Instance.Tick <= 0 || m_ColliderProxy == null)
            {
                return;
            }

            //每隔150帧变换颜色；大概5秒；
            int changeValue = Mathf.FloorToInt(World.Instance.Tick / MaxChangeTick);
            if (changeValue != m_LastChangeValue)
            {
                m_LastChangeValue = changeValue;

                if (ColliderLayer == EColliderLayer.MapBlack)
                {
                    ColliderLayer = EColliderLayer.MapWhite;
                }
                else if (ColliderLayer == EColliderLayer.MapWhite)
                {
                    ColliderLayer = EColliderLayer.MapBlack;
                }

                if (m_ColliderProxy.LayerType != (int)ColliderLayer)
                {
                    PhysicSystem.Instance?.GetCollisionSystem()?.RemoveCollider(m_ColliderProxy);

                    //重新赋值碰撞层 ColliderLayer；
                    m_ColliderProxy.LayerType = (int)ColliderLayer;

                    PhysicSystem.Instance?.GetCollisionSystem()?.AddCollider(m_ColliderProxy);
                }

                //ChangeColor
                if (ColliderLayer == EColliderLayer.MapBlack)
                {
                    m_MaterialPropertyBlock.SetColor("_Color", m_ColorBlack);
                    m_Renderer.SetPropertyBlock(m_MaterialPropertyBlock);
                }
                else if (ColliderLayer == EColliderLayer.MapWhite)
                {
                    m_MaterialPropertyBlock.SetColor("_Color", m_ColorWhite);
                    m_Renderer.SetPropertyBlock(m_MaterialPropertyBlock);
                }
            }
        }

        private ColliderPrefab CreateColliderPrefab()
        {
            //Create Shape
            LVector2 size = new LVector2((transform.localScale.x * 10f / 2f).ToLFloat(), (transform.localScale.z * 10f / 2f).ToLFloat());
            CBaseShape cBaseShape = new CAABB(size);

            //CreateCTransform2D 这里为localPosition。
            LVector2 pos = LVector2.zero;
            CTransform2D cTransform2D = new CTransform2D(pos);

            ColliderPart colliderPart = new ColliderPart()
            {
                collider = cBaseShape,
                transform = cTransform2D,
            };

            ColliderPrefab colliderPrefab = new ColliderPrefab();
            colliderPrefab.parts.Add(colliderPart);

            return colliderPrefab;
        }

        private ColliderProxy CreateColliderProxy()
        {
            ColliderPrefab colliderPrefab = CreateColliderPrefab();

            ColliderProxy proxy = new ColliderProxy();
            proxy.Init(colliderPrefab, transform.position.ToLVector2XZ());
            proxy.IsStatic = true;
            proxy.LayerType = (int)ColliderLayer;
            return proxy;
        }

        public void AddColliderProxy()
        {
            if (m_ColliderProxy != null)
            {
                PhysicSystem.Instance?.GetCollisionSystem()?.AddCollider(m_ColliderProxy);
            }
        }

        public void RemoveColliderProxy()
        {
            if (m_ColliderProxy != null)
            {
                PhysicSystem.Instance?.GetCollisionSystem()?.RemoveCollider(m_ColliderProxy);
            }
        }
    }
}
