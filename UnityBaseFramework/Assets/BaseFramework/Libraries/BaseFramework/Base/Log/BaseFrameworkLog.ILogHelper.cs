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
            /// <param name="level">日志等级。</param>
            /// <param name="message">日志内容。</param>
            void Log(BaseFrameworkLogLevel level, object message);
        }
    }
}