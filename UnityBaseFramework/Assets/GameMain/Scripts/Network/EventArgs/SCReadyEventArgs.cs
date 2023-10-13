using BaseFramework;
using BaseFramework.Event;
using GameProto;
using System.Collections.Generic;

namespace XGame
{
    /// <summary>
    /// 准备游戏。
    /// </summary>
    public sealed class SCReadyEventArgs : GameEventArgs
    {
        /// <summary>
        /// 准备事件编号。
        /// </summary>
        public static readonly int EventId = typeof(SCReadyEventArgs).GetHashCode();

        /// <summary>
        /// 初始化准备返回事件的新实例。
        /// </summary>
        public SCReadyEventArgs()
        {
            UserReadyInfos = new();
            UserData = null;
        }

        /// <summary>
        /// 获取准备事件编号。
        /// </summary>
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public int RoomId
        {
            get;
            private set;
        }

        public int LocalId
        {
            get;
            private set;
        }

        public List<UserReadyInfo> UserReadyInfos
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
        /// 创建准备事件。
        /// </summary>
        /// <param name="userReadyInfos"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        public static SCReadyEventArgs Create(int roomId, int localId, List<UserReadyInfo> userReadyInfos, object userData = null)
        {
            SCReadyEventArgs scReadyEventArgs = ReferencePool.Acquire<SCReadyEventArgs>();
            scReadyEventArgs.RoomId = roomId;
            scReadyEventArgs.LocalId = localId;
            scReadyEventArgs.UserReadyInfos.AddRange(userReadyInfos);
            scReadyEventArgs.UserData = userData;
            return scReadyEventArgs;
        }

        /// <summary>
        /// 清理准备事件。
        /// </summary>
        public override void Clear()
        {
            UserReadyInfos.Clear();
            UserData = null;
        }
    }
}
