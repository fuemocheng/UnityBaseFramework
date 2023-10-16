namespace Network
{
    /// <summary>
    /// 网络Id生成器。
    /// </summary>
    public class NetworkIdGenerator
    {

        private static long m_ChannelId = 0;

        private static long m_SessionId = 0;

        /// <summary>
        /// 生成ChannelId。
        /// </summary>
        /// <returns></returns>
        public static long GenerateChannelId()
        {
            return m_ChannelId++;
        }

        /// <summary>
        /// 生成SessionId。
        /// </summary>
        /// <returns></returns>
        public static long GenerateSessionId()
        {
            return m_SessionId++;
        }
    }
}
