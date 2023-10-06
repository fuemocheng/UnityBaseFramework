using Lockstep.Math;

namespace XGame
{
    public class EnemySystem : BaseSystem
    {
        private Spawner[] Spawners;
        private Enemy[] AllEnemy;

        public override void Start()
        {
            //for (int i = 0; i < 3; i++)
            //{
            //    var configId = 100 + i;
            //    var config = _gameConfigService.GetEntityConfig(configId) as SpawnerConfig;
            //    _gameStateService.CreateEntity<Spawner>(configId, config.entity.Info.spawnPoint);
            //}

            //foreach (var spawner in Spawners)
            //{
            //    spawner.ServiceContainer = _serviceContainer;
            //    spawner.GameStateService = _gameStateService;
            //    spawner.DebugService = _debugService;
            //    spawner.Start();
            //}
        }

        public override void Update(LFloat deltaTime)
        {
            //foreach (var spawner in Spawners)
            //{
            //    spawner.Update(deltaTime);
            //}

            //foreach (var enemy in AllEnemy)
            //{
            //    enemy.Update(deltaTime);
            //}
        }
    }
}
