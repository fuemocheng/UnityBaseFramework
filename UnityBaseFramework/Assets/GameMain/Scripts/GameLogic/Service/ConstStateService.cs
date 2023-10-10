using Lockstep;
using Lockstep.Game;

namespace XGame
{
    public partial class ConstStateService : IService
    {
        public static ConstStateService Instance { get; private set; }

        public ConstStateService()
        {
            Instance = this;
        }

        public bool IsVideoLoading { get; set; }
        public bool IsVideoMode { get; set; }
        public bool IsRunVideo { get; set; }
        public bool IsClientMode { get; set; }
        public bool IsReconnecting { get; set; }

        public bool IsPursueFrame { get; set; }

        public string GameName { get; set; }
        public int CurLevel { get; set; }
        public IContexts Contexts { get; set; }
        public int SnapshotFrameInterval { get; set; }
        public EPureModeType RunMode { get; set; }
        public byte LocalActorId { get; set; }

        private string _clientConfigPath;

        public string ClientConfigPath =>
            _clientConfigPath ?? (_clientConfigPath = _relPath + $"Data/Client/{GameName}/");

        private string _relPath = "";

        public string RelPath
        {
            get => _relPath;
            set => _relPath = value;
        }
    }
}
