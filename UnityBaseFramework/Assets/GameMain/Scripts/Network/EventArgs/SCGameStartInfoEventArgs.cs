using BaseFramework;
using BaseFramework.Event;

namespace XGame
{
    /// <summary>
    /// 游戏开始数据。
    /// </summary>
    public sealed class SCGameStartInfoEventArgs : GameEventArgs
    {
        /// <summary>
        /// 游戏开始数据事件编号。
        /// </summary>
        public static readonly int EventId = typeof(SCGameStartInfoEventArgs).GetHashCode();

        /// <summary>
        /// 初始化游戏开始数据返回事件的新实例。
        /// </summary>
        public SCGameStartInfoEventArgs()
        {
            UserData = null;
        }

        /// <summary>
        /// 获取游戏开始数据事件编号。
        /// </summary>
        public override int Id
        {
            get
            {
                return EventId;
            }
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
        /// 创建游戏开始数据事件。
        /// </summary>
        /// <param name="e">内部事件。</param>
        /// <returns></returns>
        public static SCGameStartInfoEventArgs Create(object userData = null)
        {
            SCGameStartInfoEventArgs scGameStartInfoEventArgs = ReferencePool.Acquire<SCGameStartInfoEventArgs>();
            scGameStartInfoEventArgs.UserData = userData;
            return scGameStartInfoEventArgs;
        }

        /// <summary>
        /// 清理游戏开始数据事件。
        /// </summary>
        public override void Clear()
        {
            UserData = null;
        }
    }
}
