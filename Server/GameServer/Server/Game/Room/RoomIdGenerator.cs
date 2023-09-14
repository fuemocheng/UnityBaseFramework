namespace Server
{
    /// <summary>
    /// 房间Id生成器
    /// </summary>
    public class RoomIdGenerator
    {
        /// <summary>
        /// 从10000开始，预留10000个。
        /// </summary>
        private static int m_Id = 10000;

        /// <summary>
        /// 生成Id。
        /// </summary>
        /// <returns></returns>
        public static int GenerateId()
        {
            return m_Id++;
        }
    }
}
