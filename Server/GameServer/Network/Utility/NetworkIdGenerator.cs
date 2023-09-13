namespace Network
{
    /// <summary>
    /// 网络Id生成器。
    /// </summary>
    public class NetworkIdGenerator
    {
        private static long m_Id = 0;

        /// <summary>
        /// 生成Id。
        /// </summary>
        /// <returns></returns>
        public static long GenerateId()
        {
            return m_Id++;
        }
    }
}
