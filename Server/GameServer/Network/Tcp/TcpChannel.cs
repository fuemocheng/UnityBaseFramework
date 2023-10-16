using System.Net;
using System.Net.Sockets;
using BaseFramework;
using BaseFramework.Runtime;

namespace Network
{
    /// <summary>
    /// TCP信道，封装Socket。
    /// </summary>
    public sealed class TcpChannel : NetworkChannelBase
    {
        private Socket m_Socket;
        private readonly TcpService m_Service;
        private readonly INetworkChannelHelper m_NetworkChannelHelper;

        private SocketAsyncEventArgs m_InnArgs;
        private SocketAsyncEventArgs m_OutArgs;

        // V1 使用 CircularBuffer。
        //private readonly CircularBuffer m_RecvBuffer;
        //private readonly CircularBuffer m_SendBuffer;
        //private readonly PacketParser m_Parser;

        // V2 使用 MemoryStream。
        private readonly Queue<Packet> m_SendPacketPool;
        private readonly SendState m_SendState;
        private readonly ReceiveState m_ReceiveState;

        private IPEndPoint m_RemoteEndPoint;
        private bool m_IsConnected;
        private bool m_IsSending;

        private const float DefaultHeartBeatInterval = 30f;
        private readonly HeartBeatState m_HeartBeatState;
        private bool m_ResetHeartBeatElapseSecondsWhenReceivePacket;
        private float m_HeartBeatInterval;

        private bool m_Disposed = false;

        /// <summary>
        /// 连接到远程主机的信道。
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ipEndPoint"></param>
        /// <param name="service"></param>
        public TcpChannel(long id, IPEndPoint ipEndPoint, TcpService service, INetworkChannelHelper networkChannelHelper)
        {
            Id = id;
            m_Service = service;
            m_RemoteEndPoint = ipEndPoint;
            m_NetworkChannelHelper = networkChannelHelper;

            m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
            m_Socket.NoDelay = true;

            m_InnArgs = new();
            m_OutArgs = new();
            m_InnArgs.Completed += OnComplete;
            m_OutArgs.Completed += OnComplete;

            // V1 使用 CircularBuffer。
            //m_RecvBuffer = new();
            //m_SendBuffer = new();
            //m_Parser = new PacketParser(m_RecvBuffer);

            // V2 使用 MemoryStream。
            m_SendPacketPool = new();
            m_SendState = new();
            m_ReceiveState = new();
            m_SendState.Reset();
            m_ReceiveState.PrepareForPacketHeader(m_NetworkChannelHelper.PacketHeaderLength);

            m_HeartBeatState = new();
            m_HeartBeatInterval = DefaultHeartBeatInterval;
            m_ResetHeartBeatElapseSecondsWhenReceivePacket = false;

            m_IsConnected = false;
            m_IsSending = false;


            m_Disposed = false;

            m_Service.ProcessingQueue.Enqueue(new TcpProcessingArgs() { ChannelId = Id, TcpOperation = TcpOperation.Connect });

        }

        /// <summary>
        /// 创建一个接收发送信息的信道。
        /// </summary>
        /// <param name="id"></param>
        /// <param name="socket"></param>
        /// <param name="service"></param>
        public TcpChannel(long id, Socket socket, TcpService service, INetworkChannelHelper networkChannelHelper)
        {
            Id = id;
            m_Socket = socket;
            m_Socket.NoDelay = true;
            m_Service = service;
            m_RemoteEndPoint = (IPEndPoint)m_Socket.RemoteEndPoint;
            m_NetworkChannelHelper = networkChannelHelper;

            m_InnArgs = new();
            m_OutArgs = new();
            m_InnArgs.Completed += OnComplete;
            m_OutArgs.Completed += OnComplete;

            // V1 使用 CircularBuffer。
            //m_RecvBuffer = new();
            //m_SendBuffer = new();
            //m_Parser = new PacketParser(m_RecvBuffer);

            // V2 使用 MemoryStream。
            m_SendPacketPool = new();
            m_SendState = new();
            m_ReceiveState = new();
            m_SendState.Reset();
            m_ReceiveState.PrepareForPacketHeader(m_NetworkChannelHelper.PacketHeaderLength);

            m_HeartBeatState = new();
            m_HeartBeatInterval = DefaultHeartBeatInterval;
            m_ResetHeartBeatElapseSecondsWhenReceivePacket = false;

            // 接收到远程主机的连接。
            m_IsConnected = true;
            m_IsSending = false;

            m_Disposed = false;

            m_Service.ProcessingQueue.Enqueue(new TcpProcessingArgs() { ChannelId = Id, TcpOperation = TcpOperation.StartRecv });
            m_Service.ProcessingQueue.Enqueue(new TcpProcessingArgs() { ChannelId = Id, TcpOperation = TcpOperation.StartSend });
        }

        public override void Dispose()
        {
            if (m_Disposed)
            {
                return;
            }
            m_Disposed = true;

            Log.Info($"TChannel dispose: {Id} {m_RemoteEndPoint}");

            NetworkPacketHandler = null;
            NetworkChannelConnected = null;
            NetworkChannelClosed = null;
            NetworkChannelMissHeartBeat = null;
            NetworkChannelError = null;
            NetworkChannelCustomError = null;

            long tId = Id;
            Id = 0;
            m_Service?.RemoveChannel(tId);
            m_Socket?.Close();
            m_InnArgs?.Dispose();
            m_OutArgs?.Dispose();
            m_InnArgs = null;
            m_OutArgs = null;
            m_Socket = null;


            lock (m_SendPacketPool)
            {
                m_SendPacketPool?.Clear();
            }

            lock (m_HeartBeatState)
            {
                m_HeartBeatState?.Reset(true);
            }

            m_IsConnected = false;
        }

        public override void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (m_HeartBeatInterval > 0)
            {
                bool sendHeartBeat = false;
                int missHeartBeatCount = 0;
                lock (m_HeartBeatState)
                {
                    if (m_Socket == null)
                    {
                        return;
                    }

                    m_HeartBeatState.HeartBeatElapseSeconds += realElapseSeconds;
                    if (m_HeartBeatState.HeartBeatElapseSeconds >= m_HeartBeatInterval)
                    {
                        sendHeartBeat = true;
                        missHeartBeatCount = m_HeartBeatState.MissHeartBeatCount;
                        m_HeartBeatState.HeartBeatElapseSeconds = 0f;
                        m_HeartBeatState.MissHeartBeatCount++;
                    }
                }

                // 服务器只需接收客户端的心跳，指定时间内没有收到就当作断开连接。
                if (sendHeartBeat /*&& m_NetworkChannelHelper.SendHeartBeat(this)*/)
                {
                    if (missHeartBeatCount > 0 && NetworkChannelMissHeartBeat != null)
                    {
                        NetworkChannelMissHeartBeat(this, missHeartBeatCount);
                    }
                }
            }
        }

        #region CallBack Event
        /// <summary>
        /// Connect、Receive、Send 异步完成的回调。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnComplete(object? sender, SocketAsyncEventArgs e)
        {
            m_Service.ProcessingQueue.Enqueue(new TcpProcessingArgs() { ChannelId = Id, SocketAsyncEventArgs = e });
        }
        #endregion


        #region Connect 连接远程主机
        /// <summary>
        /// 开启异步连接。
        /// </summary>
        public void StartConnect()
        {
            m_OutArgs.RemoteEndPoint = m_RemoteEndPoint;

            // 异步操作是否挂起。false 表示没挂起，则立即执行；true 表示挂起，则等异步完成执行 OnComplete()。
            bool isPending = m_Socket.ConnectAsync(m_OutArgs);
            if (isPending)
            {
                return;
            }
            else
            {
                OnConnectComplete(m_OutArgs);
            }
        }

        /// <summary>
        /// 异步连接完成事件。
        /// </summary>
        /// <param name="e"></param>
        public void OnConnectComplete(SocketAsyncEventArgs e)
        {
            if (m_Socket == null)
            {
                if (NetworkChannelError != null)
                {
                    string errorMessage = $"TChannel.OnConnectComplete Socket is null.";
                    NetworkChannelError(this, NetworkErrorCode.ConnectError, e.SocketError, errorMessage);
                }
                return;
            }

            if (e.SocketError != SocketError.Success)
            {
                if (NetworkChannelError != null)
                {
                    string errorMessage = $"TChannel.OnConnectComplete SocketError : {e.SocketError}";
                    NetworkChannelError(this, NetworkErrorCode.ConnectError, e.SocketError, errorMessage);
                }
                return;
            }

            // 清理参数e，后续其他操作可复用。
            e.RemoteEndPoint = null;

            // 连接成功。
            m_IsConnected = true;

            // 连接远程主机回调。
            if (NetworkChannelConnected != null)
            {
                NetworkChannelConnected(this, new ConnectState(e.ConnectSocket, null));
            }

            // 开始接收、发送消息。
            m_Service.ProcessingQueue.Enqueue(new TcpProcessingArgs { ChannelId = Id, TcpOperation = TcpOperation.StartRecv });
            m_Service.ProcessingQueue.Enqueue(new TcpProcessingArgs { ChannelId = Id, TcpOperation = TcpOperation.StartSend });
        }

        /// <summary>
        /// 连接断开事件。
        /// </summary>
        /// <param name="e"></param>
        public void OnDisconnectComplete(SocketAsyncEventArgs e)
        {
            if (NetworkChannelError != null)
            {
                string errorMessage = $"TChannel.OnDisconnectComplete Disconnected : {e.SocketError}";
                NetworkChannelError(this, NetworkErrorCode.SocketError, e.SocketError, errorMessage);
            }
        }
        #endregion


        #region Receive
        /// <summary>
        /// 开启异步接收。
        /// </summary>
        public void StartRecv()
        {
            while (true)
            {
                try
                {
                    if (m_Socket == null)
                    {
                        return;
                    }

                    // V1 使用 CircularBuffer。
                    //int size = m_RecvBuffer.ChunkSize - m_RecvBuffer.LastIndex;
                    //m_InnArgs.SetBuffer(m_RecvBuffer.Last, m_RecvBuffer.LastIndex, size);

                    // V2 使用 MemoryStream。
                    m_InnArgs.SetBuffer(m_ReceiveState.Stream.GetBuffer(), (int)m_ReceiveState.Stream.Position,
                        (int)(m_ReceiveState.Stream.Length - m_ReceiveState.Stream.Position));
                }
                catch (Exception e)
                {
                    Log.Error($"TChannel.StartRecv SetBuffer Error : {this.Id} {m_RemoteEndPoint} {e}");
                    if (NetworkChannelError != null)
                    {
                        string errorMessage = $"TChannel.StartRecv Exception : {e.Message}";
                        NetworkChannelError(this, NetworkErrorCode.ReceiveError, SocketError.SocketError, errorMessage);
                    }
                    return;
                }

                // 异步操作是否挂起。false 表示没挂起，则立即执行；true 表示挂起，则等异步完成执行 OnComplete()。
                bool isPending = m_Socket.ReceiveAsync(m_InnArgs);
                if (isPending)
                {
                    return;
                }
                else
                {
                    HandleRecv(m_InnArgs);
                }
            }
        }

        /// <summary>
        /// 异步接收完成事件。
        /// </summary>
        /// <param name="e"></param>
        public void OnRecvComplete(SocketAsyncEventArgs e)
        {
            HandleRecv(e);

            if (m_Socket == null)
            {
                return;
            }

            m_Service.ProcessingQueue.Enqueue(new TcpProcessingArgs { ChannelId = Id, TcpOperation = TcpOperation.StartRecv });
        }

        /// <summary>
        /// 处理接收的消息。
        /// </summary>
        /// <param name="e"></param>
        private void HandleRecv(SocketAsyncEventArgs e)
        {
            if (m_Socket == null)
            {
                return;
            }

            if (e.SocketError != SocketError.Success)
            {
                if (NetworkChannelError != null)
                {
                    string errorMessage = $"TChannel.HandleRecv SocketError : {e.SocketError}";
                    NetworkChannelError(this, NetworkErrorCode.SocketError, e.SocketError, errorMessage);
                }
                return;
            }

            if (e.BytesTransferred == 0)
            {
                if (NetworkChannelError != null)
                {
                    string errorMessage = "TChannel.HandleRecv e.BytesTransferred is 0，client closed connection";
                    NetworkChannelError(this, NetworkErrorCode.SocketError, e.SocketError, errorMessage);
                }
                return;
            }

            // V1 使用 CircularBuffer。
            //m_RecvBuffer.LastIndex += e.BytesTransferred;
            //if (m_RecvBuffer.LastIndex == m_RecvBuffer.ChunkSize)
            //{
            //    m_RecvBuffer.AddLast();
            //    m_RecvBuffer.LastIndex = 0;
            //}
            //while (true)
            //{
            //    // 这里循环解析消息执行，有可能，执行消息的过程中断开了session
            //    if (m_Socket == null)
            //    {
            //        return;
            //    }
            //    try
            //    {
            //        // V1 使用 CircularBuffer。
            //        //bool isFinished = m_Parser.Parse();
            //        //if (!isFinished)
            //        //{
            //        //    break;
            //        //}
            //        //Packet packet = this.m_Parser.GetPacket();
            //        // 处理消息
            //        //m_Service.ReadCallback(packet);
            //    }
            //    catch (Exception exception)
            //    {
            //        Log.Error($"TChannel.HandleRecv Exception: {this.Id} {m_RemoteEndPoint} {exception}");
            //        string errorMessage = $"TChannel.HandleRecv Exception : {exception}";
            //        NetworkChannelError(this, NetworkErrorCode.DeserializePacketError, SocketError.Success, errorMessage);
            //        return;
            //    }
            //}

            // V2 使用 MemoryStream。
            m_ReceiveState.Stream.Position += e.BytesTransferred;
            if (m_ReceiveState.Stream.Position < m_ReceiveState.Stream.Length)
            {
                return;
            }

            m_ReceiveState.Stream.Position = 0L;

            bool processSuccess = false;
            if (m_ReceiveState.PacketHeader != null)
            {
                processSuccess = ProcessPacket();
            }
            else
            {
                processSuccess = ProcessPacketHeader();
            }

            if (processSuccess)
            {
                return;
            }
        }

        private bool ProcessPacketHeader()
        {
            try
            {
                IPacketHeader packetHeader = m_NetworkChannelHelper.DeserializePacketHeader(m_ReceiveState.Stream);

                if (packetHeader == null)
                {
                    string errorMessage = "TChannel.ProcessPacketHeader Packet header is invalid.";
                    if (NetworkChannelError != null)
                    {
                        NetworkChannelError(this, NetworkErrorCode.DeserializePacketHeaderError, SocketError.Success, errorMessage);
                        return false;
                    }

                    throw new Exception(errorMessage);
                }

                m_ReceiveState.PrepareForPacket(packetHeader);
                if (packetHeader.PacketLength <= 0)
                {
                    bool processSuccess = ProcessPacket();
                    return processSuccess;
                }
            }
            catch (Exception exception)
            {
                if (NetworkChannelError != null)
                {
                    SocketException socketException = exception as SocketException;
                    NetworkChannelError(this, NetworkErrorCode.DeserializePacketHeaderError, socketException != null ? socketException.SocketErrorCode : SocketError.Success, exception.ToString());
                    return false;
                }

                throw;
            }

            return true;
        }

        private bool ProcessPacket()
        {
            lock (m_HeartBeatState)
            {
                m_HeartBeatState.Reset(m_ResetHeartBeatElapseSecondsWhenReceivePacket);
            }

            try
            {
                Packet packet = m_NetworkChannelHelper.DeserializePacket(m_ReceiveState.PacketHeader, m_ReceiveState.Stream);

                if (packet != null)
                {
                    // 处理消息包。
                    if (NetworkPacketHandler != null)
                    {
                        NetworkPacketHandler(this, packet);
                    }
                    else
                    {
                        throw new Exception("TChannel.ProcessPacket NetworkPacketHandler is null.");
                    }
                }

                m_ReceiveState.PrepareForPacketHeader(m_NetworkChannelHelper.PacketHeaderLength);
            }
            catch (Exception exception)
            {
                if (NetworkChannelError != null)
                {
                    SocketException socketException = exception as SocketException;
                    NetworkChannelError(this, NetworkErrorCode.DeserializePacketError, socketException != null ? socketException.SocketErrorCode : SocketError.Success, exception.ToString());
                    return false;
                }

                throw;
            }

            return true;
        }
        #endregion


        #region Send
        /// <summary>
        /// V1 使用 CircularBuffer。发送消息。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <exception cref="Exception"></exception>
        //public override void Send(byte[] buffer, int index, int length)
        //{
        //    if (IsDisposed)
        //    {
        //        throw new Exception("TChannel has been disposed, can not send message.");
        //    }

        //    byte[] size = BytesHelper.GetBytes((ushort)buffer.Length);
        //    m_SendBuffer.Write(size, 0, size.Length);
        //    m_SendBuffer.Write(buffer, index, length);

        //    if (!m_IsSending)
        //    {
        //        m_Service.ProcessingQueue.Enqueue(new TArgs() { ChannelId = Id, TcpOperation = ETcpOp.StartSend });
        //    }
        //}

        /// <summary>
        /// V2 使用 MemoryStream。发送消息。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="packet"></param>
        /// <exception cref="Exception"></exception>
        public override bool Send<T>(T packet)
        {
            if (m_Socket == null)
            {
                string errorMessage = "TChannel.Send You must connect first.";
                if (NetworkChannelError != null)
                {
                    NetworkChannelError(this, NetworkErrorCode.SendError, SocketError.Success, errorMessage);
                    return false;
                }

                throw new Exception(errorMessage);
            }

            if (packet == null)
            {
                string errorMessage = "TChannel.Send Packet is invalid.";
                if (NetworkChannelError != null)
                {
                    NetworkChannelError(this, NetworkErrorCode.SendError, SocketError.Success, errorMessage);
                    return false;
                }

                throw new Exception(errorMessage);
            }

            // 缓存发送包。
            lock (m_SendPacketPool)
            {
                m_SendPacketPool.Enqueue(packet);
            }

            if (!m_IsSending)
            {
                m_Service.ProcessingQueue.Enqueue(new TcpProcessingArgs() { ChannelId = Id, TcpOperation = TcpOperation.StartSend });
            }

            return true;
        }

        /// <summary>
        /// 开启异步发送消息。
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void StartSend()
        {
            if (!m_IsConnected)
            {
                return;
            }

            if (m_IsSending)
            {
                return;
            }

            while (true)
            {
                try
                {
                    if (m_Socket == null)
                    {
                        m_IsSending = false;
                        return;
                    }

                    // V1 使用 CircularBuffer。
                    // 没有数据需要发送
                    //if (m_SendBuffer.Length == 0)
                    //{
                    //    m_IsSending = false;
                    //    return;
                    //}

                    // V2 使用 MemoryStream。
                    // 没有数据需要发送
                    if (m_SendState.Stream.Length == 0 && m_SendPacketPool.Count == 0)
                    {
                        m_IsSending = false;
                        return;
                    }

                    m_IsSending = true;

                    // V1 使用 CircularBuffer。
                    //int sendSize = m_SendBuffer.ChunkSize - m_SendBuffer.FirstIndex;
                    //if (sendSize > m_SendBuffer.Length)
                    //{
                    //    sendSize = (int)m_SendBuffer.Length;
                    //}
                    //m_OutArgs.SetBuffer(m_SendBuffer.First, m_SendBuffer.FirstIndex, sendSize);

                    // V2 使用 MemoryStream。
                    // TODO:这里要考虑一次性发送数据超过缓存区大小的问题。
                    while(m_SendPacketPool.Count > 0)
                    {
                        Packet packet = null;
                        lock(m_SendPacketPool)
                        {
                            packet = m_SendPacketPool.Dequeue();
                        }

                        // 将消息序列化到 m_SendState.Stream
                        bool serializeResult = false;
                        try
                        {
                            serializeResult = m_NetworkChannelHelper.Serialize(packet, m_SendState.Stream);
                        }
                        catch (Exception exception)
                        {
                            if (NetworkChannelError != null)
                            {
                                SocketException socketException = exception as SocketException;
                                NetworkChannelError(this, NetworkErrorCode.SerializeError, socketException != null ? socketException.SocketErrorCode : SocketError.Success, exception.ToString());
                                return;
                            }
                            throw;
                        }

                        if (!serializeResult)
                        {
                            string errorMessage = "TChannel.Send Serialized packet failure.";
                            if (NetworkChannelError != null)
                            {
                                NetworkChannelError(this, NetworkErrorCode.SerializeError, SocketError.Success, errorMessage);
                                return;
                            }

                            throw new Exception(errorMessage);
                        }
                    }
                    
                    m_SendState.Stream.Position = 0;

                    m_OutArgs.SetBuffer(m_SendState.Stream.GetBuffer(), (int)m_SendState.Stream.Position,
                        (int)(m_SendState.Stream.Length - m_SendState.Stream.Position));

                    // 异步操作是否挂起。false 表示没挂起，则立即执行；true 表示挂起，则等异步完成执行 OnComplete()。
                    bool isPending = m_Socket.SendAsync(m_OutArgs);
                    if (isPending)
                    {
                        return;
                    }
                    else
                    {
                        HandleSend(m_OutArgs);
                    }
                }
                catch (Exception e)
                {
                    throw new Exception($"TChannel.StartSend Error", e);
                }
            }
        }

        /// <summary>
        /// 异步发送完成事件。
        /// </summary>
        /// <param name="o"></param>
        public void OnSendComplete(SocketAsyncEventArgs e)
        {
            HandleSend(e);

            m_IsSending = false;

            m_Service.ProcessingQueue.Enqueue(new TcpProcessingArgs() { ChannelId = Id, TcpOperation = TcpOperation.StartSend });
        }

        /// <summary>
        /// 处理发送完成事件。
        /// </summary>
        /// <param name="e"></param>
        private void HandleSend(SocketAsyncEventArgs e)
        {
            if (m_Socket == null)
            {
                return;
            }

            if (e.SocketError != SocketError.Success)
            {
                if (NetworkChannelError != null)
                {
                    string errorMessage = $"TChannel.HandleSend SocketError : {e.SocketError}";
                    NetworkChannelError(this, NetworkErrorCode.SocketError, e.SocketError, errorMessage);
                }
                return;
            }

            if (e.BytesTransferred == 0)
            {
                if (NetworkChannelError != null)
                {
                    string errorMessage = "TChannel.HandleRecv e.BytesTransferred is 0.";
                    NetworkChannelError(this, NetworkErrorCode.SocketError, e.SocketError, errorMessage);
                }
                return;
            }

            // V1 使用 CircularBuffer。
            //m_SendBuffer.FirstIndex += e.BytesTransferred;
            //if (m_SendBuffer.FirstIndex == m_SendBuffer.ChunkSize)
            //{
            //    m_SendBuffer.FirstIndex = 0;
            //    m_SendBuffer.RemoveFirst();
            //}

            // V2 使用 MemoryStream。
            m_SendState.Stream.Position += e.BytesTransferred;
            if (m_SendState.Stream.Position < m_SendState.Stream.Length)
            {
                return;
            }
            m_SendState.Reset();
        }
        #endregion
    }
}
