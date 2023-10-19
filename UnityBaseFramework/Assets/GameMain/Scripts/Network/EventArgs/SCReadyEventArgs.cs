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
            UserGameInfos = new();
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

        public List<UserGameInfo> UserGameInfos
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
        /// <param name="userGameInfos"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        public static SCReadyEventArgs Create(int roomId, int localId, List<UserGameInfo> userGameInfos, object userData = null)
        {
            SCReadyEventArgs scReadyEventArgs = ReferencePool.Acquire<SCReadyEventArgs>();
            scReadyEventArgs.RoomId = roomId;
            scReadyEventArgs.LocalId = localId;
            scReadyEventArgs.UserGameInfos.AddRange(userGameInfos);
            scReadyEventArgs.UserData = userData;
            return scReadyEventArgs;
        }

        /// <summary>
        /// 清理准备事件。
        /// </summary>
        public override void Clear()
        {
            UserGameInfos.Clear();
            UserData = null;
        }
    }
}
