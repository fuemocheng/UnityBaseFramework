using BaseFramework;
using BaseFramework.Event;

namespace XGame
{
    /// <summary>
    /// 开始游戏。
    /// </summary>
    public sealed class SCStartEventArgs : GameEventArgs
    {
        /// <summary>
        /// 开始游戏事件编号。
        /// </summary>
        public static readonly int EventId = typeof(SCStartEventArgs).GetHashCode();

        /// <summary>
        /// 初始化开始游戏返回事件的新实例。
        /// </summary>
        public SCStartEventArgs()
        {
            UserData = null;
        }

        /// <summary>
        /// 获取开始游戏成功事件编号。
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
        /// 创建开始游戏成功事件。
        /// </summary>
        /// <param name="e">内部事件。</param>
        /// <returns>创建的网络连接成功事件。</returns>
        public static SCStartEventArgs Create(object userData = null)
        {
            SCStartEventArgs scStartEventArgs = ReferencePool.Acquire<SCStartEventArgs>();
            return scStartEventArgs;
        }

        /// <summary>
        /// 清理开始游戏成功事件。
        /// </summary>
        public override void Clear()
        {
            UserData = null;
        }
    }
}
