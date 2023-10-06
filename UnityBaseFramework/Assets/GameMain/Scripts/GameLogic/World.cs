using Lockstep.Game;
using Lockstep.Math;
using System.Collections;
using System.Collections.Generic;
using UnityBaseFramework.Runtime;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

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

        public static Player MyPlayer;

        public static object MyPlayerTrans => MyPlayer?.engineTransform;

        private List<BaseSystem> m_Systems = new List<BaseSystem>();

        private bool _hasStart = false;

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
            Tick = 0;

            //初始化Service, 初始化系统
            RegisterSystems();
        }

        public void StartSimulate()
        {
            if (_hasStart)
            {
                return;
            }

            _hasStart = true;
        }

        /// <summary>
        /// 帧步进。
        /// </summary>
        public void Step()
        {
            //if (IsPause)
            //{
            //    return;
            //}

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

            Log.Info($" Rollback diff:{Tick - tick} From{Tick}->{tick}  maxContinueServerTick:{maxContinueServerTick} {isNeedClear}");
            //_timeMachineService.RollbackTo(tick);
            //_commonStateService.SetTick(tick);
            Tick = tick;
        }
    }
}
