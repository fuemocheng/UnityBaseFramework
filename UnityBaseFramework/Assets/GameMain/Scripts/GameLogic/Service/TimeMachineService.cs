using Lockstep.Game;
using System.Collections.Generic;
using System.Linq;

namespace XGame
{
    public class TimeMachineService : IService
    {
        private HashSet<ITimeMachine> _timeMachineHash = new HashSet<ITimeMachine>();
        private ITimeMachine[] _allTimeMachines;

        public int CurTick { get; private set; }

        public void RegisterTimeMachine(ITimeMachine roll)
        {
            if (roll != null && roll != this && _timeMachineHash.Add(roll))
            {
                _allTimeMachines = null;
            }
        }

        private ITimeMachine[] GetAllTimeMachines()
        {
            if (_allTimeMachines == null)
            {
                _allTimeMachines = _timeMachineHash.ToArray();
            }

            return _allTimeMachines;
        }

        public void RollbackTo(int tick)
        {
            CurTick = tick;
            foreach (var timeMachine in GetAllTimeMachines())
            {
                timeMachine.RollbackTo(tick);
            }
        }

        public void Backup(int tick)
        {
            CurTick = tick;
            foreach (var timeMachine in GetAllTimeMachines())
            {
                timeMachine.Backup(tick);
            }
        }

        public void Clean(int maxVerifiedTick)
        {
            foreach (var timeMachine in GetAllTimeMachines())
            {
                timeMachine.Clean(maxVerifiedTick);
            }
        }
    }
}
