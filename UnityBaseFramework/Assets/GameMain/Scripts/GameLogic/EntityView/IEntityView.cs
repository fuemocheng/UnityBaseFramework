using Lockstep.Math;

namespace XGame
{
    public interface IEntityView
    {
        void OnTakeDamage(int amount, LVector3 hitPoint);

        void OnDead();

        void OnRollbackDestroy();
    }
}
