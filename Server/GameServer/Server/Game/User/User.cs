using BaseFramework;
using Network;

namespace Server
{
    public class User : IReference
    {
        public long UserId = 0;
        public string Account = string.Empty;
        public string Password = string.Empty;
        public string UserName = string.Empty;
        public Session TcpSession = null;
        public Session KcpSession = null;
        public GameLogicComponent GameLogicComponent = null;
        public Room Room = null;
        public int LocalId = -1;
        public bool IsReady = false;
        public int LoadingProgress = 0;

        public User() 
        {
        }

        public void Clear()
        {
            UserId = 0;
            Account = string.Empty;
            Password = string.Empty;
            UserName = string.Empty;
            TcpSession?.Dispose();
            KcpSession?.Dispose();
            TcpSession = null;
            KcpSession = null;
            GameLogicComponent = null;
            Room = null;
            LocalId = -1;
            IsReady = false;
            LoadingProgress = 0;
        }
    }
}
