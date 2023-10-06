using System;
using Lockstep.Math;

namespace XGame
{
    [Serializable]
    public partial class Spawner : BaseEntity
    {
        //public SpawnerInfo Info = new SpawnerInfo();
        public LFloat Timer;

        public override void Start()
        {
            //Timer = Info.spawnTime;
        }

        public override void Update(LFloat deltaTime)
        {
            //Timer += deltaTime;
            //if (Timer > Info.spawnTime)
            //{
            //    Timer = LFloat.zero;
            //    Spawn();
            //}
        }

        public void Spawn()
        {
            //if (GameStateService.CurEnemyCount >= GameStateService.MaxEnemyCount)
            //{
            //    return;
            //}

            //GameStateService.CurEnemyCount++;
            //GameStateService.CreateEntity<Enemy>(Info.prefabId, Info.spawnPoint);
        }
    }
}