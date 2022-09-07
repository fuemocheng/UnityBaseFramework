using BaseFramework;
using UnityEngine;

namespace UnityBaseFramework.Runtime
{
    /// <summary>
    /// 日志辅助器。
    /// </summary>
    public class DefaultLogHelper : BaseFrameworkLog.ILogHelper
    {
        /// <summary>
        /// 打印日志。
        /// </summary>
        /// <param name="level">日志等级。</param>
        /// <param name="message">日志内容。</param>
        public void Log(BaseFrameworkLogLevel level, object message)
        {
            switch (level)
            {
                case BaseFrameworkLogLevel.Debug:
                    Debug.Log(Utility.Text.Format("<color=#888888>{0}</color>", message));
                    break;

                case BaseFrameworkLogLevel.Info:
                    Debug.Log(message.ToString());
                    break;

                case BaseFrameworkLogLevel.Warning:
                    Debug.LogWarning(message.ToString());
                    break;

                case BaseFrameworkLogLevel.Error:
                    Debug.LogError(message.ToString());
                    break;

                default:
                    throw new BaseFrameworkException(message.ToString());
            }
        }
    }
}
