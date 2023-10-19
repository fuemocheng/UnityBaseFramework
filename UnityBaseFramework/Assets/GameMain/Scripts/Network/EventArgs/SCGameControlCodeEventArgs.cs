using BaseFramework;
using BaseFramework.Event;

namespace XGame
{
    /// <summary>
    /// 游戏控制。
    /// </summary>
    public sealed class SCGameControlCodeEventArgs : GameEventArgs
    {
        /// <summary>
        /// 游戏控制事件编号。
        /// </summary>
        public static readonly int EventId = typeof(SCGameControlCodeEventArgs).GetHashCode();

        /// <summary>
        /// 初始化游戏控制返回事件的新实例。
        /// </summary>
        public SCGameControlCodeEventArgs()
        {
            UserData = null;
        }

        /// <summary>
        /// 获取游戏控制事件编号。
        /// </summary>
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public int GameControlCode
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
        /// 创建游戏控制事件。
        /// </summary>
        /// <param name="e">内部事件。</param>
        /// <returns></returns>
        public static SCGameControlCodeEventArgs Create(int gameControlCode, object userData = null)
        {
            SCGameControlCodeEventArgs scGameControlCodeEventArgs = ReferencePool.Acquire<SCGameControlCodeEventArgs>();
            scGameControlCodeEventArgs.GameControlCode = gameControlCode;
            scGameControlCodeEventArgs.UserData = userData;
            return scGameControlCodeEventArgs;
        }

        /// <summary>
        /// 清理游戏控制事件。
        /// </summary>
        public override void Clear()
        {
            GameControlCode = 0;
            UserData = null;
        }
    }
}
