using BaseFramework;
using Network;

namespace Server
{
    /// <summary>
    /// 会话。
    /// </summary>
    public sealed class Session : IDisposable
    {
        public long Id;

        private NetworkComponent m_NetworkComponent; //网络代理
        private NetworkChannelBase m_Channel; //网络信道u
        private NetworkChannelHelper m_ChannelHelper; //网络信道处理

        public Action NetworkChannelError;

        public object BindInfo; //会话绑定信息，一般是User;

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

        public void Awake(NetworkComponent networkComponent, NetworkChannelBase channel, NetworkChannelHelper channelHelper)
        {
            m_NetworkComponent = networkComponent;
            m_Channel = channel;
            m_ChannelHelper = channelHelper;

            // 消息处理分发；
            m_Channel.NetworkPacketHandler = OnNetworkPacketHandler;
        }

        public void Dispose()
        {
            if (m_Disposed)
            {
                return;
            }
            m_Disposed = true;

            Id = 0;
            m_Channel.Dispose();
            m_Channel = null;
        }

        public NetworkChannelBase Channel
        {
            get
            {
                return m_Channel;
            }
            set
            {
                m_Channel = value;
            }
        }

        #region Receive & Handle

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

        public void OnNetworkChannelError()
        {
            if (NetworkChannelError != null)
            {
                NetworkChannelError();
            }

            User user = (User)BindInfo;
            if (user != null)
            {
                //如果不是加载中，也不是游戏中，断线或者离线，退出房间。
                if(user.UserState != EUserState.Loading && user.UserState != EUserState.Playing)
                {
                    user.Room?.LeaveRoom(user);
                }

                user.TcpSession = null;
                user.KcpSession = null;
            }
        }

        #endregion


        #region Send
        /// <summary>
        /// 发送消息。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="packet"></param>
        /// <returns></returns>
        /// <exception cref="BaseFrameworkException"></exception>
        public bool Send<T>(T packet) where T : Packet
        {
            if (m_Channel == null)
            {
                throw new BaseFrameworkException($"Session.Send Channel is null.");
            }
            return m_Channel.Send(packet);
        }
        #endregion
    }
}
