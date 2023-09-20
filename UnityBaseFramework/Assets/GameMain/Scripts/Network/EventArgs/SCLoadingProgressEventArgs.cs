using BaseFramework;
using BaseFramework.Event;

namespace XGame
{
    /// <summary>
    /// 加载进度。
    /// </summary>
    public sealed class SCLoadingProgressEventArgs : GameEventArgs
    {
        /// <summary>
        /// 加载进度事件编号。
        /// </summary>
        public static readonly int EventId = typeof(SCLoadingProgressEventArgs).GetHashCode();

        /// <summary>
        /// 初始化加载进度返回事件的新实例。
        /// </summary>
        public SCLoadingProgressEventArgs()
        {
            UserData = null;
        }

        /// <summary>
        /// 获取加载进度事件编号。
        /// </summary>
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public int AllProgress
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取用户自定义数据。
        /// </summary>
        public object UserData
        {
            get;
            private set;
        }

        /// <summary>
        /// 创建加载进度事件。
        /// </summary>
        /// <param name="e">内部事件。</param>
        /// <returns></returns>
        public static SCLoadingProgressEventArgs Create(int loadingProgress, object userData = null)
        {
            SCLoadingProgressEventArgs scLoadingProgressEventArgs = ReferencePool.Acquire<SCLoadingProgressEventArgs>();
            scLoadingProgressEventArgs.AllProgress = loadingProgress;
            scLoadingProgressEventArgs.UserData = userData;
            return scLoadingProgressEventArgs;
        }

        /// <summary>
        /// 清理加载进度事件。
        /// </summary>
        public override void Clear()
        {
            AllProgress = 0;
            UserData = null;
        }
    }
}
