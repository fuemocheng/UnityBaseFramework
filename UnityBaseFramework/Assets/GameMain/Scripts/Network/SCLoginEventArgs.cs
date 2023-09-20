using BaseFramework;
using BaseFramework.Event;

namespace XGame
{
    /// <summary>
    /// 登录返回。
    /// </summary>
    public sealed class SCLoginEventArgs : GameEventArgs
    {
        /// <summary>
        /// 登录返回事件编号。
        /// </summary>
        public static readonly int EventId = typeof(SCLoginEventArgs).GetHashCode();

        /// <summary>
        /// 初始化登录返回事件的新实例。
        /// </summary>
        public SCLoginEventArgs()
        {
            UserData = null;
        }

        /// <summary>
        /// 获取登录返回成功事件编号。
        /// </summary>
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        /// <summary>
        /// 获取登录返回码。
        /// </summary>
        public int RetCode
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
        /// 创建登录返回成功事件。
        /// </summary>
        /// <param name="e">内部事件。</param>
        /// <returns>创建的网络连接成功事件。</returns>
        public static SCLoginEventArgs Create(int retCode, object userData = null)
        {
            SCLoginEventArgs scLoginEventArgs = ReferencePool.Acquire<SCLoginEventArgs>();
            scLoginEventArgs.RetCode = retCode;
            scLoginEventArgs.UserData = userData;
            return scLoginEventArgs;
        }

        /// <summary>
        /// 清理登录返回成功事件。
        /// </summary>
        public override void Clear()
        {
            RetCode = 0;
            UserData = null;
        }
    }
}
