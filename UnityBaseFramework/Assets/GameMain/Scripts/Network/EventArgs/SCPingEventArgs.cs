using BaseFramework;
using BaseFramework.Event;
using GameProto;
using System.Collections.Generic;

namespace XGame
{
    /// <summary>
    /// Ping。
    /// </summary>
    public sealed class SCPingEventArgs : GameEventArgs
    {
        /// <summary>
        /// Ping事件编号。
        /// </summary>
        public static readonly int EventId = typeof(SCPingEventArgs).GetHashCode();

        /// <summary>
        /// 初始化Ping返回事件的新实例。
        /// </summary>
        public SCPingEventArgs()
        {
            UserData = null;
        }

        /// <summary>
        /// 获取Ping事件编号。
        /// </summary>
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public int LocalId
        {
            get;
            private set;
        }

        public long SendTimestamp
        {
            get;
            private set;
        }

        public long TimeSinceServerStart
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
        /// 创建Ping事件。
        /// </summary>
        /// <param name="userReadyInfos"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        public static SCPingEventArgs Create(int localId, long sendTimestamp, long timeSinceServerStart, object userData = null)
        {
            SCPingEventArgs SCPingEventArgs = ReferencePool.Acquire<SCPingEventArgs>();
            SCPingEventArgs.LocalId = localId;
            SCPingEventArgs.SendTimestamp = sendTimestamp;
            SCPingEventArgs.TimeSinceServerStart = timeSinceServerStart;
            SCPingEventArgs.UserData = userData;
            return SCPingEventArgs;
        }

        /// <summary>
        /// 清理Ping事件。
        /// </summary>
        public override void Clear()
        {
            UserData = null;
        }
    }
}
