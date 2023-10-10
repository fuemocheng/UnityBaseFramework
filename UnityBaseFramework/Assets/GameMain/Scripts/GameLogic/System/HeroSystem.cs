using Lockstep.Math;

namespace XGame
{
    public class HeroSystem : BaseSystem
    {
        public override void Update(LFloat deltaTime)
        {
            foreach (var player in GameEntry.Service.GetService<GameStateService>().GetPlayers())
            {
                player.Update(deltaTime);
            }
        }
    }
}
