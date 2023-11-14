using Lockstep.Game;

namespace XGame
{
    public partial class Enemy : IAfterBackup { public void OnAfterDeserialize() { } }
    public partial class Player : IAfterBackup { public void OnAfterDeserialize() { } }
    public partial class Spawner : IAfterBackup { public void OnAfterDeserialize() { } }
    public partial class Bullet : IAfterBackup { public void OnAfterDeserialize() { } }
}
