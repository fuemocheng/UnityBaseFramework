using System;

namespace XGame
{
    public class GameTime
    {
        /// <summary>
        /// 开始时间戳，毫秒。
        /// </summary>
        private static long m_StartTimeStamp = 0;

        /// <summary>
        /// 初始化开始的时间戳。
        /// </summary>
        public static void InitStartTimeStamp()
        {
            m_StartTimeStamp = ((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// 初始化开始的时间戳。
        /// 重连的时候，需要根据服务器的开始时间设置当前游戏的时间戳。
        /// </summary>
        /// <param name="startTimeStamp">时间戳，毫秒。</param>
        public static void InitStartTimeStamp(long startTimeStamp)
        {
            m_StartTimeStamp = startTimeStamp;
        }

        /// <summary>
        /// 从游戏开始到现在的毫秒数。
        /// </summary>
        public static long RealtimeSinceStartupMS
        {
            get
            {
                return ((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds() - m_StartTimeStamp;
            }
        }

        /// <summary>
        /// 当前的时间戳。
        /// </summary>
        public static long CurrTimeStamp
        {
            get
            {
                return ((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds();
            }
        }
    }
}
