using GameProto;
using Lockstep.Math;
using System.Collections.Generic;
using System.Linq;
using UnityBaseFramework.Runtime;

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

        public Input[] PlayerInputs => GameEntry.Service.GetService<GameStateService>().GetPlayers().Select(a => a.input).ToArray();

        public static Player MyPlayer;

        public static object MyPlayerTrans => MyPlayer?.engineTransform;

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
    }
}
