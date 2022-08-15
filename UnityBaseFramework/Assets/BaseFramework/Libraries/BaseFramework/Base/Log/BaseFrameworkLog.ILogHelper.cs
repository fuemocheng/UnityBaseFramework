namespace BaseFramework
{
    public static partial class BaseFrameworkLog
    {
        /// <summary>
        /// 基础模块日志辅助器接口
        /// </summary>
        public interface ILogHelper
        {
            /// <summary>
            /// 打印日志
            /// </summary>
            /// <param name="level"></param>
            /// <param name="message"></param>
            void Log(BaseFrameworkLogLevel level, object message);
        }
    }
}