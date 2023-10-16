using System.Net.Sockets;

namespace Network
{
    public abstract class NetworkServiceBase : INetworkService, IDisposable
    {
        public long Id { get; set; }

        public Action<NetworkChannelBase, object> NetworkChannelAccept;
        public Action<NetworkChannelBase, object> NetworkChannelConnected;
        public Action<NetworkChannelBase, int> NetworkChannelMissHeartBeat;
        public Action<NetworkChannelBase, NetworkErrorCode, SocketError, string> NetworkChannelError;
        public Action<NetworkChannelBase, object> NetworkChannelCustomError;

        public abstract void Update(float elapseSeconds, float realElapseSeconds);

        public abstract NetworkChannelBase GetChannel(long id);

        public abstract void RemoveChannel(long id);

        public abstract void Dispose();
    }
}
