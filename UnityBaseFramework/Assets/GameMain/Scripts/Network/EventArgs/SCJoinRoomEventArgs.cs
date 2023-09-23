using BaseFramework;
using BaseFramework.Event;
using GameProto;
using System.Collections.Generic;

namespace XGame
{
    /// <summary>
    /// 加入游戏房间。
    /// </summary>
    public sealed class SCJoinRoomEventArgs : GameEventArgs
    {
        /// <summary>
        /// 加入游戏房间事件编号。
        /// </summary>
        public static readonly int EventId = typeof(SCJoinRoomEventArgs).GetHashCode();

        /// <summary>
        /// 初始化加入游戏房间返回事件的新实例。
        /// </summary>
        public SCJoinRoomEventArgs()
        {
            UserData = null;
            UserReadyInfos = new();
        }

        /// <summary>
        /// 获取加入游戏房间成功事件编号。
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
        /// 创建加入游戏房间成功事件。
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="localId"></param>
        /// <param name="userReadyInfos"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        public static SCJoinRoomEventArgs Create(int roomId, int localId, List<UserReadyInfo> userReadyInfos, object userData = null)
        {
            SCJoinRoomEventArgs scReadyEventArgs = ReferencePool.Acquire<SCJoinRoomEventArgs>();
            scReadyEventArgs.RoomId = roomId;
            scReadyEventArgs.LocalId = localId;
            scReadyEventArgs.UserReadyInfos.AddRange(userReadyInfos);
            scReadyEventArgs.UserData = userData;
            return scReadyEventArgs;
        }

        /// <summary>
        /// 清理加入游戏房间成功事件。
        /// </summary>
        public override void Clear()
        {
            UserReadyInfos.Clear();
            UserData = null;
            RoomId = 0;
            LocalId = 0;
        }
    }
}
