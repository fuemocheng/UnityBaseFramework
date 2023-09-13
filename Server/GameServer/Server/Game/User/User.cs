using BaseFramework;
using Network;

namespace Server
{
    public class User : IReference
    {
        public long UserId = 0;
        public string Account = string.Empty;
        public string Password = string.Empty;
        public Session TcpSession = null;
        public Session KcpSession = null;
        public Game Game = null;



        public void Clear()
        {
            UserId = 0;
            Account = string.Empty;
            Password = string.Empty;
            TcpSession?.Dispose();
            KcpSession?.Dispose();
            TcpSession = null;
            KcpSession = null;

        }
    }
}
