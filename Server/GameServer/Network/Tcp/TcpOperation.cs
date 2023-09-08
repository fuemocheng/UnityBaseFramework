namespace Network
{
    public enum TcpOperation
    {
        /// <summary>
        /// 服务器主动连接操作（连接客户端、或者其他服务器）。
        /// </summary>
        Connect,

        /// <summary>
        /// 开始接收消息操作。
        /// </summary>
        StartRecv,

        /// <summary>
        /// 开始发送消息操作
        /// </summary>
        StartSend,
    }
}
