using BaseFramework;

namespace Network
{
    /// <summary>
    /// 会话。
    /// </summary>
    public sealed class Session : IDisposable
    {
        public long Id;

        private NetworkProxy m_NetworkProxy; //网络代理
        private NetworkChannelBase m_Channel; //网络信道
        private NetworkChannelHelper m_ChannelHelper; //网络信道处理

        public object BindInfo; //会话持有者

        private bool m_Disposed = false;

        public Session()
        {
            m_Disposed = false;
        }

        public Session(long id)
        {
            Id = id;
            m_Disposed = false;
        }

        public void Awake(NetworkProxy networkProxy, NetworkChannelBase channel, NetworkChannelHelper channelHelper)
        {
            m_NetworkProxy = networkProxy;
            m_Channel = channel;
            m_ChannelHelper = channelHelper;

            // 消息处理分发；
            m_Channel.NetworkPacketHandler = OnNetworkPacketHandler;
        }

        public void Dispose()
        {
            if(m_Disposed)
            {
                return;
            }

            Id = 0;
            m_Channel.Dispose();
            m_Channel = null;

            m_Disposed = true;
        }

        public bool Send<T>(T packet) where T : Packet
        {
            if(m_Channel == null) 
            {
                throw new BaseFrameworkException($"Session.Send Channel is null.");
            }
            return m_Channel.Send(packet);
        }

        /// <summary>
        /// 消息包处理。
        /// </summary>
        /// <param name="base"></param>
        /// <param name="packet"></param>
        private void OnNetworkPacketHandler(NetworkChannelBase channel, Packet packet)
        {
            if (m_Channel == null)
            {
                throw new BaseFrameworkException($"Session.OnNetworkPacketHandler Channel is null.");
            }

            if (channel.Id != m_Channel.Id)
            {
                throw new BaseFrameworkException($"Session.OnNetworkPacketHandler Channel is wrong.");
            }

            IPacketHandler handler = m_ChannelHelper.GetPacketHandler(packet.Id);
            handler.Handle(this, packet);
        }

    }
}
