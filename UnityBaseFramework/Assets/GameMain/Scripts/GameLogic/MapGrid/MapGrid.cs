using Lockstep.Collision2D;
using Lockstep.ECS.ECDefine;
using Lockstep.Math;
using UnityEngine;

namespace XGame
{
    public class MapGrid : MonoBehaviour
    {
        private ColliderProxy m_ColliderProxy = null;

        public EColliderLayer ColliderLayer = EColliderLayer.Static;

        void Start()
        {
            GameEntry.Map?.AddMapGrid(this);

            //地图打开的时候预创建。
            m_ColliderProxy = CreateColliderProxy();
        }

        void Update()
        {

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

        public void RegisterColliderProxy()
        {
            if (m_ColliderProxy != null)
            {
                PhysicSystem.Instance?.GetCollisionSystem()?.AddCollider(m_ColliderProxy);
            }
        }
    }
}
