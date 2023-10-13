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

        public int LocalId = -1;
        public int LoadingProgress = 0;
        public EUserState UserState = EUserState.Default;
        public Room Room = null;

        public Session TcpSession = null;
        public Session KcpSession = null;
        public GameLogicComponent GameLogicComponent = null;

        public User()
        {
        }

        public void Clear()
        {
            UserId = 0;
            Account = string.Empty;
            Password = string.Empty;
            UserName = string.Empty;

            LocalId = -1;
            LoadingProgress = 0;
            UserState = EUserState.Default;
            Room = null;

            TcpSession?.Dispose();
            KcpSession?.Dispose();
            TcpSession = null;
            KcpSession = null;
            GameLogicComponent = null;
        }
    }
}
