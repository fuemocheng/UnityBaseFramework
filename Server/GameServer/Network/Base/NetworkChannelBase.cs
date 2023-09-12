using System.Net.Sockets;

namespace Network
{
    /// <summary>
    /// 网络频道基类。
    /// </summary>
    public abstract class NetworkChannelBase : INetworkChannel, IDisposable
    {
        public long Id { get; set; }

        public Action<NetworkChannelBase, Packet> NetworkPacketHandler;
        public Action<NetworkChannelBase, object> NetworkChannelConnected;
        public Action<NetworkChannelBase> NetworkChannelClosed;
        public Action<NetworkChannelBase, int> NetworkChannelMissHeartBeat;
        public Action<NetworkChannelBase, NetworkErrorCode, SocketError, string> NetworkChannelError;
        public Action<NetworkChannelBase, object> NetworkChannelCustomError;

        public abstract void Update(float elapseSeconds, float realElapseSeconds);

        public abstract void Dispose();

        public abstract bool Send<T>(T packet) where T : Packet;
    }
}
