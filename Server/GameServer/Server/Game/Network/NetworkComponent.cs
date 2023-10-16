using System.Net;
using System.Net.Sockets;
using BaseFramework.Runtime;
using Network;

namespace Server
{
    public class NetworkComponent : IDisposable
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
                    m_Service.NetworkChannelAccept += OnNetworkChannelAccept;
                    m_Service.NetworkChannelConnected += OnNetworkChannelConnected;
                    m_Service.NetworkChannelMissHeartBeat += OnNetworkChannelMissHeartBeat;
                    m_Service.NetworkChannelError += OnNetworkChannelError;
                    m_Service.NetworkChannelCustomError += OnNetworkChannelCustomError;
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
            m_Disposed = true;

            m_Service?.Dispose();

            foreach (long id in m_Sessions.Keys.ToArray())
            {
                Session session = m_Sessions[id];
                session.Dispose();
            }
            m_Sessions.Clear();
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            m_Service?.Update(elapseSeconds, realElapseSeconds);
        }

        public Session CreateSession(NetworkComponent networkProxy, NetworkChannelBase channel)
        {
            Session session = new Session(NetworkIdGenerator.GenerateSessionId());
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

        public Session GetSession(long channelId)
        {
            foreach (var kvp in m_Sessions)
            {
                if (kvp.Value != null && kvp.Value.Channel != null && kvp.Value.Channel.Id == channelId)
                {
                    return kvp.Value;
                }
            }
            return null;
        }

        /// <summary>
        /// 接受连接。
        /// </summary>
        /// <param name="channel"></param>
        public void OnNetworkChannelAccept(NetworkChannelBase channel, object arg)
        {
            SocketAsyncEventArgs saea = (SocketAsyncEventArgs)arg;
            string remoteEndPoint = (saea != null) ? ((saea.RemoteEndPoint != null) ? saea.RemoteEndPoint.ToString() : "") : "";
            Log.Info("Network channel '{0}' connected, remote address '{1}'.", channel.Id, remoteEndPoint);

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
            Log.Info("Network channel '{0}' connected remote address '{1}' succeed.", channel.Id, ((SocketAsyncEventArgs)arg).AcceptSocket.RemoteEndPoint.ToString());
        }

        /// <summary>
        /// 心跳丢失回调。
        /// </summary>
        /// <param name="channel">NetworkChannelBase</param>
        /// <param name="arg">ConnectState</param>
        public void OnNetworkChannelMissHeartBeat(NetworkChannelBase channel, int missHeartBeatCount)
        {
            Log.Error($"OnNetworkChannelMissHeartBeat Channel:{channel.Id}  MissHeartBeatCount:{missHeartBeatCount}");
            if (missHeartBeatCount < 2)
            {
                return;
            }
            //TODO:关闭信道，下线处理...

        }

        /// <summary>
        /// 网络信道出错回调。
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="errorCode"></param>
        /// <param name="socketError"></param>
        /// <param name="errorMessage"></param>
        private void OnNetworkChannelError(NetworkChannelBase channel, NetworkErrorCode errorCode, SocketError socketError, string error)
        {
            Log.Info($"Network channel '{channel.Id}' Error, ErrorCode:{errorCode}, SocketError:{socketError}, ErrorMessage:{error}");
            Log.Info($"Network channel '{channel.Id}' closed.");

            Session tSession = GetSession(channel.Id);
            tSession?.OnNetworkChannelError();

            m_Service.RemoveChannel(channel.Id);
            RemoveSession(tSession.Id);
        }

        /// <summary>
        /// 网络信道自定义出错回调。
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="arg"></param>
        private void OnNetworkChannelCustomError(NetworkChannelBase channel, object arg)
        {
            Log.Info($"Network channel '{channel.Id}' custom error.");
        }
    }
}
