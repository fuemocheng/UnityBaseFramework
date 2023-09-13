namespace Server
{
    /// <summary>
    /// 用户Id生成器。
    /// </summary>
    public class UserIdGenerator
    {
        /// <summary>
        /// 从10000开始，预留10000个。
        /// </summary>
        private static long m_Id = 10000;

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
