using GameProto;
using Lockstep.Collision2D;
using Lockstep.Math;
using System.Collections.Generic;
using System.Linq;
using UnityBaseFramework.Runtime;
using UnityEngine;

namespace XGame
{
    public class World
    {
        public static World Instance
        {
            get;
            private set;
        }

        public World()
        {
            Instance = this;
        }

        public int Tick { get; private set; }

        public GameProto.Input[] PlayerInputs => GameEntry.Service.GetService<GameStateService>().GetPlayers().Select(a => a.input).ToArray();

        public static Player MyPlayer;

        public static object MyPlayerTrans => MyPlayer?.EngineTransform;

        private List<BaseSystem> m_Systems = new List<BaseSystem>();

        private bool m_HasStart = false;

        private void RegisterSystem(BaseSystem baseSystem)
        {
            m_Systems.Add(baseSystem);
        }

        private void RegisterSystems()
        {
            RegisterSystem(new HeroSystem());
            RegisterSystem(new EnemySystem());
            RegisterSystem(new PhysicSystem());
            RegisterSystem(new HashSystem());

            
        }

        public void OnGameCreate()
        {
            Instance = this;

            Tick = 0;

            // 注册系统。
            RegisterSystems();

            // Awake。
            foreach (BaseSystem system in m_Systems)
            {
                system.Awake();
            }

            // Start。
            foreach (BaseSystem system in m_Systems)
            {
                system.Start();
            }

            InitColliderTest();
        }

        public void OnGameDestroy()
        {
            Instance = null;

            foreach (BaseSystem system in m_Systems)
            {
                system?.Destroy();
            }
            m_Systems.Clear();
        }

        public void StartSimulate(List<UserGameInfo> userGameInfos, int localActorId)
        {
            if (m_HasStart)
            {
                return;
            }

            m_HasStart = true;

            for (int i = 0; i < userGameInfos.Count; i++)
            {
                int prefabId = 0;
                LVector2 initPos = LVector2.zero;
                Player player = GameEntry.Service.GetService<GameStateService>().CreateEntity<Player>(prefabId, initPos);

                player.localId = userGameInfos[i].LocalId;
            }

            var allPlayers = GameEntry.Service.GetService<GameStateService>().GetPlayers();

            MyPlayer = allPlayers[localActorId];
        }

        /// <summary>
        /// 帧步进。
        /// </summary>
        public void Step()
        {
            if (GameEntry.Service.GetService<CommonStateService>().IsPause)
            {
                return;
            }

            //更新系统
            var deltaTime = new LFloat(true, 30);
            foreach (var system in m_Systems)
            {
                if (system.Enable)
                {
                    system.Update(deltaTime);
                }
            }

            Tick++;
        }

        public void RollbackTo(int tick, int maxContinueServerTick, bool isNeedClear = true)
        {
            if (tick < 0)
            {
                Log.Error("Target Tick invalid!" + tick);
                return;
            }

            Log.Info($"Rollback diff:{Tick - tick} From{Tick}->{tick}  maxContinueServerTick:{maxContinueServerTick} {isNeedClear}");
            GameEntry.Service.GetService<TimeMachineService>().RollbackTo(tick);
            GameEntry.Service.GetService<CommonStateService>().SetTick(tick);
            Tick = tick;
        }

        #region Test

        private List<ColliderPrefab> prefabs = new List<ColliderPrefab>();

        Dictionary<EShape2D, PrimitiveType> type2PType = new Dictionary<EShape2D, PrimitiveType>() {
            {EShape2D.Circle, PrimitiveType.Cylinder},
            {EShape2D.AABB, PrimitiveType.Cube},
            {EShape2D.OBB, PrimitiveType.Cube},
        };

        const int size = 4;

        public int count = 1;

        public float percent = 0.1f;

        private void InitColliderTest()
        {
            void CreatePrefab(CBaseShape collider)
            {
                var prefab = new ColliderPrefab();
                prefab.parts.Add(new ColliderPart()
                {
                    transform = new CTransform2D(LVector2.zero),
                    collider = collider
                });
                prefabs.Add(prefab);
            }

            for (int i = 1; i < 2; i++)
            {
                for (int j = 1; j < 2; j++)
                {
                    CreatePrefab(new CAABB(new LVector2(i, j)));
                    CreatePrefab(new COBB(new LVector2(i, j), LFloat.zero));
                    CreatePrefab(new CCircle(((i + j) * 0.5f).ToLFloat()));
                }
            }

            for (int i = 0; i < count; i++)
            {
                int layerType = 0;
                var rawColor = Color.white;
                bool isStatic = true;
                if (i < percent * count * 2)
                {
                    layerType = 1;
                    isStatic = false;
                    rawColor = Color.yellow;
                    if (i < percent * count)
                    {
                        rawColor = Color.green;
                        layerType = 2;
                    }
                }

                var proxy = CreateType(0, true, rawColor);
                PhysicSystem.Instance.GetCollisionSystem().AddCollider(proxy);
                //collisionSystem.AddCollider(proxy);
            }
        }

        private ColliderProxy CreateType(int layerType, bool isStatic, Color rawColor)
        {
            var prefab = prefabs[UnityEngine.Random.Range(0, prefabs.Count)];
            var type = (EShape2D)prefab.collider.TypeId;
            var obj = GameObject.CreatePrimitive(type2PType[type]).GetComponent<Collider>();
            //obj.transform.SetParent(transform, false);
            obj.transform.position = new Vector3(UnityEngine.Random.Range(-20, 20), 0,
                UnityEngine.Random.Range(-20, 20));
            switch (type)
            {
                case EShape2D.Circle:
                    {
                        var colInfo = (CCircle)prefab.collider;
                        obj.transform.localScale =
                            new Vector3(colInfo.radius.ToFloat() * 2, 1, colInfo.radius.ToFloat() * 2);
                        break;
                    }
                case EShape2D.AABB:
                    {
                        var colInfo = (CAABB)prefab.collider;
                        obj.transform.localScale =
                            new Vector3(colInfo.size.x.ToFloat() * 2, 1, colInfo.size.y.ToFloat() * 2);
                        break;
                    }
                case EShape2D.OBB:
                    {
                        var colInfo = (COBB)prefab.collider;
                        obj.transform.localScale =
                            new Vector3(colInfo.size.x.ToFloat() * 2, 1, colInfo.size.y.ToFloat() * 2);
                        break;
                    }
            }

            var proxy = new ColliderProxy();
            proxy.Init(prefab, obj.transform.position.ToLVector2XZ());
            if (!isStatic)
            {
                var mover = obj.gameObject.AddComponent<RandomMove>();
                mover.halfworldSize = 20;
                mover.isNeedRotate = type == EShape2D.OBB;
            }

            proxy.IsStatic = isStatic;
            proxy.LayerType = layerType;
            return proxy;
        }
        #endregion
    }
}
