using BaseFramework;
using BaseFramework.Event;
using GameProto;
using System.Collections.Generic;

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
            UserGameInfos = new();
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

        public int RoomId
        {
            get;
            private set;
        }

        public int MapId
        {
            get;
            private set;
        }

        public int LocalId
        {
            get;
            private set;
        }

        public int UserCount
        {
            get;
            private set;
        }

        public int Seed
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
        /// 创建游戏开始数据事件。
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="mapId"></param>
        /// <param name="userCount"></param>
        /// <param name="seed"></param>
        /// <param name="userGameInfos"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        public static SCGameStartInfoEventArgs Create(int roomId, int mapId, int localId, int userCount, int seed, List<UserGameInfo> userGameInfos, object userData = null)
        {
            SCGameStartInfoEventArgs scGameStartInfoEventArgs = ReferencePool.Acquire<SCGameStartInfoEventArgs>();
            scGameStartInfoEventArgs.RoomId = roomId;
            scGameStartInfoEventArgs.MapId = mapId;
            scGameStartInfoEventArgs.LocalId = localId;
            scGameStartInfoEventArgs.UserCount = userCount;
            scGameStartInfoEventArgs.Seed = seed;
            scGameStartInfoEventArgs.UserGameInfos.AddRange(userGameInfos);
            scGameStartInfoEventArgs.UserData = userData;
            return scGameStartInfoEventArgs;
        }

        /// <summary>
        /// 清理游戏开始数据事件。
        /// </summary>
        public override void Clear()
        {
            RoomId = 0;
            MapId = 0;
            UserCount = 0;
            Seed = 0;
            UserGameInfos.Clear();
            UserData = null;
        }
    }
}
