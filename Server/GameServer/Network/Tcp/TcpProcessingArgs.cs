using System.Net.Sockets;

namespace Network
{
    public struct TcpProcessingArgs
    {
        /// <summary>
        /// ChannelId。
        /// </summary>
        public long ChannelId;

        /// <summary>
        /// TcpOperation。
        /// </summary>
        public TcpOperation TcpOperation;

        /// <summary>
        /// SocketAsyncEventArgs。
        /// </summary>
        public SocketAsyncEventArgs SocketAsyncEventArgs;
    }
}
