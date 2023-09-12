using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using BaseFramework.Runtime;

namespace Network
{
    /// <summary>
    /// TcpService，管理多个Tcp信道。
    /// </summary>
    public sealed class TcpService : NetworkServiceBase
    {
        private Socket m_ListenSocket;
        private INetworkChannelHelper m_ChannelHelper;
        private readonly SocketAsyncEventArgs m_InnArgs = new();
        private readonly Dictionary<long, TcpChannel> m_NetworkChannels = new();
        public ConcurrentQueue<TcpProcessingArgs> ProcessingQueue = new();

        private bool m_Disposed = false;

        /// <summary>
        /// TCPService，管理多个TCP信道。
        /// </summary>
        /// <param name="ipEndPoint"></param>
        /// <exception cref="Exception"></exception>
        public TcpService(IPEndPoint ipEndPoint, INetworkChannelHelper channelHelper)
        {
            m_ListenSocket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
            // 小心在多个套接字绑定到相同的地址和端口时可能引发的冲突。
            //m_ListenSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            m_ChannelHelper = channelHelper;

            m_InnArgs.Completed += OnComplete;

            try
            {
                m_ListenSocket.Bind(ipEndPoint);
            }
            catch (Exception e)
            {
                throw new Exception($"TService ListenSocket Bind Error : {ipEndPoint}", e);
            }

            m_ListenSocket.Listen(1000);

            m_Disposed = false;

            StartAccept();
        }

        public override void Dispose()
        {
            if (m_Disposed)
            {
                return;
            }
            m_Disposed = true;

            m_ListenSocket?.Dispose();
            m_ListenSocket = null;
            m_InnArgs?.Dispose();

            foreach (long id in m_NetworkChannels.Keys.ToArray())
            {
                TcpChannel channel = m_NetworkChannels[id];
                channel.Dispose();
            }
            m_NetworkChannels.Clear();
        }

        #region Accept 接受连接
        /// <summary>
        /// 开启异步接受连接。
        /// </summary>
        private void StartAccept()
        {
            m_InnArgs.AcceptSocket = null;

            // 异步操作是否挂起。false 表示没挂起，则立即执行；true 表示挂起，则等异步完成执行 OnComplete()。
            bool isPending = m_ListenSocket.AcceptAsync(m_InnArgs);
            if (isPending)
            {
                return;
            }
            else
            {
                OnAcceptComplete(m_InnArgs);
            }
        }

        /// <summary>
        /// 接受远程主机连接的异步回调。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="Exception"></exception>
        private void OnComplete(object? sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Accept:
                    ProcessingQueue.Enqueue(new TcpProcessingArgs() { SocketAsyncEventArgs = e });
                    break;
                default:
                    throw new Exception($"TService.OnComplete Socket Error : {e.LastOperation}");
            }
        }

        /// <summary>
        /// 接受到远程主机连接完成事件。
        /// </summary>
        /// <param name="e"></param>
        private void OnAcceptComplete(SocketAsyncEventArgs e)
        {
            if (m_ListenSocket == null)
            {
                return;
            }

            if (e.SocketError != SocketError.Success)
            {
                Log.Error($"TService.OnAcceptComplete Error: {e.SocketError}");

                // 开启新的Accept。
                StartAccept();
                return;
            }

            try
            {
                long id = CreateAcceptChannelId(0);
                TcpChannel channel = new TcpChannel(id, e.AcceptSocket, this, m_ChannelHelper);
                channel.NetworkChannelConnected += OnNetworkChannelConnected;
                channel.NetworkChannelMissHeartBeat += OnNetworkChannelMissHeartBeat;
                channel.NetworkChannelError += OnNetworkChannelError;

                m_NetworkChannels.Add(channel.Id, channel);

                // 成功接收回调。
                if (NetworkChannelAccept != null)
                {
                    NetworkChannelAccept(channel, e);
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception.ToString());
            }

            // 开始新的Accept
            StartAccept();
        }
        #endregion

        /// <summary>
        /// 更新处理队列事件。
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public override void Update(float elapseSeconds, float realElapseSeconds)
        {
            while (true)
            {
                if (!ProcessingQueue.TryDequeue(out TcpProcessingArgs arg))
                {
                    break;
                }

                SocketAsyncEventArgs e = arg.SocketAsyncEventArgs;
                if (e == null)
                {
                    switch (arg.TcpOperation)
                    {
                        case TcpOperation.Connect:
                            {
                                TcpChannel tChannel = (TcpChannel)GetChannel(arg.ChannelId);
                                if (tChannel != null)
                                {
                                    tChannel.StartConnect();
                                }
                                break;
                            }
                        case TcpOperation.StartSend:
                            {
                                TcpChannel tChannel = (TcpChannel)GetChannel(arg.ChannelId);
                                if (tChannel != null)
                                {
                                    tChannel.StartSend();
                                }
                                break;
                            }
                        case TcpOperation.StartRecv:
                            {
                                TcpChannel tChannel = (TcpChannel)GetChannel(arg.ChannelId);
                                if (tChannel != null)
                                {
                                    tChannel.StartRecv();
                                }
                                break;
                            }
                    }
                    continue;
                }

                switch (e.LastOperation)
                {
                    case SocketAsyncOperation.Accept:
                        {
                            OnAcceptComplete(e);
                            break;
                        }
                    case SocketAsyncOperation.Connect:
                        {
                            TcpChannel tChannel = (TcpChannel)GetChannel(arg.ChannelId);
                            if (tChannel != null)
                            {
                                tChannel.OnConnectComplete(e);
                            }
                            break;
                        }
                    case SocketAsyncOperation.Disconnect:
                        {
                            TcpChannel tChannel = (TcpChannel)GetChannel(arg.ChannelId);
                            if (tChannel != null)
                            {
                                tChannel.OnDisconnectComplete(e);
                            }
                            break;
                        }
                    case SocketAsyncOperation.Receive:
                        {
                            TcpChannel tChannel = (TcpChannel)GetChannel(arg.ChannelId);
                            if (tChannel != null)
                            {
                                tChannel.OnRecvComplete(e);
                            }
                            break;
                        }
                    case SocketAsyncOperation.Send:
                        {
                            TcpChannel tChannel = (TcpChannel)GetChannel(arg.ChannelId);
                            if (tChannel != null)
                            {
                                tChannel.OnSendComplete(e);
                            }
                            break;
                        };
                    default:
                        throw new ArgumentOutOfRangeException($"{e.LastOperation}");
                }
            }

            foreach (KeyValuePair<long, TcpChannel> tcpChannel in m_NetworkChannels)
            {
                tcpChannel.Value.Update(elapseSeconds, realElapseSeconds);
            }
        }

        public override NetworkChannelBase GetChannel(long id)
        {
            m_NetworkChannels.TryGetValue(id, out TcpChannel channel);
            return channel;
        }

        public override void RemoveChannel(long id)
        {
            if (m_NetworkChannels.TryGetValue(id, out TcpChannel channel))
            {
                channel.Dispose();
            }
            m_NetworkChannels.Remove(id);
        }

        /// <summary>
        /// 连接远程主机的回调。
        /// </summary>
        /// <param name="channel">NetworkChannelBase</param>
        /// <param name="arg">ConnectState</param>
        public void OnNetworkChannelConnected(NetworkChannelBase channel, object arg)
        {
            if (NetworkChannelConnected != null)
            {
                NetworkChannelConnected(channel, arg);
            }
        }

        /// <summary>
        /// 心跳丢失回调。
        /// </summary>
        /// <param name="channel">NetworkChannelBase</param>
        /// <param name="arg">ConnectState</param>
        public void OnNetworkChannelMissHeartBeat(NetworkChannelBase channel, int missHeartBeatCount)
        {
            if (NetworkChannelMissHeartBeat != null)
            {
                NetworkChannelMissHeartBeat(channel, missHeartBeatCount);
            }
        }

        /// <summary>
        /// 网络信道出错。
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="errorCode"></param>
        /// <param name="socketError"></param>
        /// <param name="error"></param>
        public void OnNetworkChannelError(NetworkChannelBase channel, NetworkErrorCode errorCode, SocketError socketError, string error)
        {
            if (NetworkChannelError != null)
            {
                NetworkChannelError(channel, errorCode, socketError, error);
            }
        }

        /// <summary>
        /// 网络信道自定义错误。
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="arg"></param>
        public void OnNetworkChannelCustomError(NetworkChannelBase channel, object arg)
        {
            if (NetworkChannelCustomError != null)
            {
                NetworkChannelCustomError(channel, arg);
            }
        }
    }
}
