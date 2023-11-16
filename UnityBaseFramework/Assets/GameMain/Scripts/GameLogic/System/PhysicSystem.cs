using System;
using System.Collections;
using System.Collections.Generic;
using Lockstep.Collision2D;
using Lockstep.Math;
using Lockstep.UnsafeCollision2D;
using UnityEngine;
using UnityEngine.Profiling;
using Ray2D = Lockstep.Collision2D.Ray2D;
using Debug = Lockstep.Logging.Debug;
using UnityBaseFramework.Runtime;

namespace XGame
{
    public class PhysicSystem : BaseSystem
    {
        private static PhysicSystem _instance;
        public static PhysicSystem Instance => _instance;

        ICollisionSystem collisionSystem;

        static Dictionary<int, ColliderPrefab> _fabId2ColPrefab = new Dictionary<int, ColliderPrefab>();
        static Dictionary<int, int> _fabId2Layer = new Dictionary<int, int>();

        static Dictionary<ILPTriggerEventHandler, ColliderProxy> _mono2ColProxy =
            new Dictionary<ILPTriggerEventHandler, ColliderProxy>();

        static Dictionary<ColliderProxy, ILPTriggerEventHandler> _colProxy2Mono =
            new Dictionary<ColliderProxy, ILPTriggerEventHandler>();

        public bool[] collisionMatrix = new bool[(int)EColliderLayer.EnumCount * (int)EColliderLayer.EnumCount];
        public LVector3 pos = new LVector3(0, 0, 0);
        public LFloat worldSize = new LFloat(60);
        public LFloat minNodeSize = new LFloat(1);
        public LFloat loosenessval = new LFloat(true, 1250);

        private LFloat halfworldSize => worldSize / 2 - 5;

        //private int[] allTypes = new int[] { 0, 1, 2 };

        public int showTreeId = 0;

        public override void Awake()
        {
            _instance = this;
        }

        public override void Start()
        {
            if (_instance != this)
            {
                Debug.LogError("Duplicate CollisionSystemAdapt!");
                return;
            }

            var collisionSystem = new CollisionSystem()
            {
                worldSize = worldSize,
                pos = pos,
                minNodeSize = minNodeSize,
                loosenessval = loosenessval
            };
            Log.Info($"worldSize:{worldSize} pos:{pos} minNodeSize:{minNodeSize} loosenessval:{loosenessval}");
            this.collisionSystem = collisionSystem;

            List<int> colliderAllLayers = new List<int>();
            for (int i = 0; i < (int)EColliderLayer.EnumCount; i++)
            {
                colliderAllLayers.Add(i);
            }

            //TODO：Read From File.
            //碰撞层级矩阵
            //collisionMatrix[layer(要检测的碰撞物的层级) * (int)EColliderLayer.EnumCount + layer(会被碰撞的层级)] = true;
            collisionMatrix[(int)EColliderLayer.Hero * (int)EColliderLayer.EnumCount + (int)EColliderLayer.Static] = true;              // Hero * Static
            collisionMatrix[(int)EColliderLayer.Hero * (int)EColliderLayer.EnumCount + (int)EColliderLayer.MapBlack] = true;         // Hero * MapGroupOne
            collisionMatrix[(int)EColliderLayer.Hero * (int)EColliderLayer.EnumCount + (int)EColliderLayer.MapWhite] = true;         // Hero * MapGroupTwo
            collisionMatrix[(int)EColliderLayer.Hero * (int)EColliderLayer.EnumCount + (int)EColliderLayer.Enemy] = false;              // Hero 不会和 Enemy 碰撞
            collisionMatrix[(int)EColliderLayer.Hero * (int)EColliderLayer.EnumCount + (int)EColliderLayer.Hero] = false;               // Hero 不会和 Hero 碰撞

            collisionSystem.DoStart(collisionMatrix, colliderAllLayers.ToArray());
            collisionSystem.funcGlobalOnTriggerEvent += GlobalOnTriggerEvent;
        }

        public override void Update(LFloat deltaTime)
        {
            collisionSystem.ShowTreeId = showTreeId;
            collisionSystem.DoUpdate(deltaTime);
        }

        public static void GlobalOnTriggerEvent(ColliderProxy a, ColliderProxy b, ECollisionEvent type)
        {
            if (_colProxy2Mono.TryGetValue(a, out var handlera))
            {
                CollisionSystem.TriggerEvent(handlera, b, type);
            }

            if (_colProxy2Mono.TryGetValue(b, out var handlerb))
            {
                CollisionSystem.TriggerEvent(handlerb, a, type);
            }
        }


        public static ColliderProxy GetCollider(int id)
        {
            return _instance.collisionSystem.GetCollider(id);
        }

        public static bool Raycast(int layerMask, Ray2D ray, out LRaycastHit2D ret)
        {
            return Raycast(layerMask, ray, out ret, LFloat.MaxValue);
        }

        public static bool Raycast(int layerMask, Ray2D ray, out LRaycastHit2D ret, LFloat maxDistance)
        {
            ret = new LRaycastHit2D();
            LFloat t = LFloat.one;
            int id;
            if (_instance.DoRaycast(layerMask, ray, out t, out id, maxDistance))
            {
                ret.point = ray.origin + ray.direction * t;
                ret.distance = t * ray.direction.magnitude;
                ret.colliderId = id;
                return true;
            }

            return false;
        }

        public static void QueryRegion(int layerType, LVector2 pos, LVector2 size, LVector2 forward,
            FuncCollision callback)
        {
            _instance._QueryRegion(layerType, pos, size, forward, callback);
        }

        public static void QueryRegion(int layerType, LVector2 pos, LFloat radius, FuncCollision callback)
        {
            _instance._QueryRegion(layerType, pos, radius, callback);
        }

        private void _QueryRegion(int layerType, LVector2 pos, LVector2 size, LVector2 forward, FuncCollision callback)
        {
            collisionSystem.QueryRegion(layerType, pos, size, forward, callback);
        }

        private void _QueryRegion(int layerType, LVector2 pos, LFloat radius, FuncCollision callback)
        {
            collisionSystem.QueryRegion(layerType, pos, radius, callback);
        }

        public bool DoRaycast(int layerMask, Ray2D ray, out LFloat t, out int id, LFloat maxDistance)
        {
            Profiler.BeginSample("DoRaycast ");
            var ret = collisionSystem.Raycast(layerMask, ray, out t, out id, maxDistance);
            Profiler.EndSample();
            return ret;
        }


        public void RigisterPrefab(int prefabId, int val)
        {
            _fabId2Layer[prefabId] = val;
        }

        public void RegisterEntity(int prefabId, CEntity entity)
        {
            ColliderPrefab colliderPrefab = null;

            RigisterPrefab(prefabId, prefabId < 100000 ? (int)EColliderLayer.Hero : (int)EColliderLayer.Enemy);

            //GameObject fab = ((Transform)entity.EngineTransform)?.gameObject;
            //if (fab == null)
            //{
            //    Log.Error("PhysicSystem RegisterEntity Prefab is null.");
            //    return;
            //}
            //if (!_fabId2ColPrefab.TryGetValue(prefabId, out colliderPrefab))
            //{
            //    colliderPrefab = CollisionSystem.CreateColliderPrefab(fab, entity.colliderData);
            //}

            if (!_fabId2ColPrefab.TryGetValue(prefabId, out colliderPrefab))
            {
                colliderPrefab = CollisionSystem.CreateColliderPrefab($"Collier_{entity.EntityId}", entity.ColliderData);
            }

            AttachToColSystem(_fabId2Layer[prefabId], colliderPrefab, entity);
        }

        public void AttachToColSystem(int layer, ColliderPrefab prefab, BaseEntity entity)
        {
            var proxy = new ColliderProxy();
            proxy.EntityObject = entity;
            proxy.Init(prefab, entity.CTransform);
            proxy.IsStatic = false;
            proxy.LayerType = layer;
            var eventHandler = entity;
            if (eventHandler != null)
            {
                _mono2ColProxy[eventHandler] = proxy;
                _colProxy2Mono[proxy] = eventHandler;
            }

            collisionSystem.AddCollider(proxy);
        }

        public void RemoveCollider(ILPTriggerEventHandler handler)
        {
            if (_mono2ColProxy.TryGetValue(handler, out var proxy))
            {
                RemoveCollider(proxy);
                _mono2ColProxy.Remove(handler);
                _colProxy2Mono.Remove(proxy);
            }
        }

        public void RemoveCollider(ColliderProxy collider)
        {
            collisionSystem.RemoveCollider(collider);
        }

        public void OnDrawGizmos()
        {
            collisionSystem?.DrawGizmos();
        }

        public ICollisionSystem GetCollisionSystem()
        {
            return collisionSystem;
        }
    }
}