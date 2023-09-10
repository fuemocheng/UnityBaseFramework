using System.Net;
using System.Net.Sockets;
using BaseFramework;
using BaseFramework.Runtime;

namespace Network
{
    public class NetworkProxy : IDisposable
    {
        private NetworkServiceBase m_Service;
        private NetworkChannelHelper m_NetworkChannelHelper;
        private readonly Dictionary<long, Session> m_Sessions = new();
        private bool m_Disposed = false;

        public void Awake()
        {
            m_NetworkChannelHelper = new NetworkChannelHelper();
            m_NetworkChannelHelper.Initialize();
        }

        public void Start(NetworkProtocolType protocol, IPEndPoint ipEndPoint)
        {
            switch (protocol)
            {
                case NetworkProtocolType.TCP:
                    m_Service = new TcpService(ipEndPoint, m_NetworkChannelHelper);
                    m_Service.NetworkChannelAccept = OnNetworkChannelAccept;
                    m_Service.NetworkChannelConnected = OnNetworkChannelConnected;
                    m_Service.NetworkChannelError = OnNetworkChannelError;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Dispose()
        {
            if (m_Disposed)
            {
                return;
            }

            m_Service?.Dispose();

            foreach (long id in m_Sessions.Keys.ToArray())
            {
                Session session = m_Sessions[id];
                session.Dispose();
            }
            m_Sessions.Clear();

            m_Disposed = true;
        }

        public void Update()
        {
            m_Service?.Update();
        }

        public Session CreateSession(NetworkProxy networkProxy, NetworkChannelBase channel)
        {
            Session session = new Session(IdGenerator.GenerateId());
            session.Awake(networkProxy, channel, m_NetworkChannelHelper);
            return session;
        }

        public void RemoveSession(long id)
        {
            Session session;
            if (!m_Sessions.TryGetValue(id, out session))
            {
                return;
            }

            m_Sessions.Remove(id);
            session.Dispose();
        }

        /// <summary>
        /// 接受连接。
        /// </summary>
        /// <param name="channel"></param>
        public void OnNetworkChannelAccept(NetworkChannelBase channel)
        {
            //创建Session
            Session session = CreateSession(this, channel);
            m_Sessions.Add(session.Id, session);
        }

        /// <summary>
        /// 连接远程主机回调。
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="arg"></param>
        private void OnNetworkChannelConnected(NetworkChannelBase channel, object arg)
        {
            
        }

        /// <summary>
        /// 网络信道出错回调。
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="errorCode"></param>
        /// <param name="socketError"></param>
        /// <param name="errorMessage"></param>
        private void OnNetworkChannelError(NetworkChannelBase channel, NetworkErrorCode errorCode, SocketError socketError, string errorMessage)
        {
            
        }

    }
}
