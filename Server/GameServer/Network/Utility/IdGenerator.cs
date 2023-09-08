namespace Network
{
    /// <summary>
    /// Id生成器。
    /// </summary>
    public class IdGenerator
    {
        private static long _id = 0;

        /// <summary>
        /// 生成Id。
        /// </summary>
        /// <returns></returns>
        public static long GenerateId()
        {
            return _id++;
        }
    }
}
