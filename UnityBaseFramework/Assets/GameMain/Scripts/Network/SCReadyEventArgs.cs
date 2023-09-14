using BaseFramework;
using BaseFramework.Event;

namespace XGame
{
    /// <summary>
    /// 准备游戏。
    /// </summary>
    public sealed class SCReadyEventArgs : GameEventArgs
    {
        /// <summary>
        /// 准备游戏事件编号。
        /// </summary>
        public static readonly int EventId = typeof(SCReadyEventArgs).GetHashCode();

        /// <summary>
        /// 初始化准备游戏返回事件的新实例。
        /// </summary>
        public SCReadyEventArgs()
        {
            CurrCount = 0;
            UserData = null;
        }

        /// <summary>
        /// 获取准备游戏成功事件编号。
        /// </summary>
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        /// <summary>
        /// 获取当前房间人数。
        /// </summary>
        public int CurrCount
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
        /// 创建准备游戏成功事件。
        /// </summary>
        /// <param name="e">内部事件。</param>
        /// <returns>创建的网络连接成功事件。</returns>
        public static SCReadyEventArgs Create(int currCount, object userData = null)
        {
            SCReadyEventArgs scReadyEventArgs = ReferencePool.Acquire<SCReadyEventArgs>();
            scReadyEventArgs.CurrCount = currCount;
            scReadyEventArgs.UserData = userData;
            return scReadyEventArgs;
        }

        /// <summary>
        /// 清理准备游戏成功事件。
        /// </summary>
        public override void Clear()
        {
            CurrCount = 0;
            UserData = null;
        }
    }
}
