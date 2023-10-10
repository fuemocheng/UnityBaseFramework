using System.Collections.Generic;
using Lockstep.Game;

namespace XGame
{
    public partial class IdService : IService, ITimeMachine
    {
        public int CurTick { get; set; }

        private int Id;

        public int GenId()
        {
            return Id++;
        }

        Dictionary<int, int> _tick2Id = new Dictionary<int, int>();

        public void RollbackTo(int tick)
        {
            Id = _tick2Id[tick];
        }

        public void Backup(int tick)
        {
            _tick2Id[tick] = Id;
        }

        public void Clean(int maxVerifiedTick) { }
    }
}
