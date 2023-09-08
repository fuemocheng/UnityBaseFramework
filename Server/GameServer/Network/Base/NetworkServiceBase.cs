using System.Net.Sockets;

namespace Network
{
    public abstract class NetworkServiceBase : INetworkService, IDisposable
    {
        public long Id { get; set; }

        public Action<NetworkChannelBase> NetworkChannelAccept;
        public Action<NetworkChannelBase, object> NetworkChannelConnected;
        public Action<NetworkChannelBase, NetworkErrorCode, SocketError, string> NetworkChannelError;

        #region IdGenerater
        private long m_AcceptIdGenerater = 1;
        /// <summary>
        /// localConn 放在低32bit。
        /// </summary>
        public long CreateAcceptChannelId(uint localConn)
        {
            return (++m_AcceptIdGenerater << 32) | localConn;
        }

        private long m_ConnectIdGenerater = int.MaxValue;
        /// <summary>
        /// localConn 放在低32bit
        /// </summary>
        public long CreateConnectChannelId(uint localConn)
        {
            return (--m_ConnectIdGenerater << 32) | localConn;
        }
        #endregion

        public abstract void Update();

        public abstract NetworkChannelBase GetChannel(long id);

        public abstract void RemoveChannel(long id);

        public abstract void Dispose();
    }
}
