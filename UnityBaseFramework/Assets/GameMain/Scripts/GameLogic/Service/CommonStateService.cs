using Lockstep.Game;
using Lockstep.Math;
using System.Collections.Generic;

namespace XGame
{
    public partial class CommonStateService : IService, ITimeMachine
    {
        public int Tick { get; set; }
        public int CurTick { get; set; }
        public LFloat DeltaTime { get; set; }
        public LFloat TimeSinceGameStart { get; set; }
        public int Hash { get; set; }
        public bool IsPause { get; set; }

        Dictionary<int, int> _tick2State = new Dictionary<int, int>();

        public void SetTick(int val)
        {
            Tick = val;
        }

        public void SetDeltaTime(LFloat val)
        {
            DeltaTime = val;
        }

        public void SetTimeSinceGameStart(LFloat val)
        {
            TimeSinceGameStart = val;
        }

        public void RollbackTo(int tick)
        {
            Hash = _tick2State[tick];
        }

        public void Backup(int tick)
        {
            _tick2State[tick] = Hash;
        }

        public void Clean(int maxVerifiedTick) { }
    }
}
