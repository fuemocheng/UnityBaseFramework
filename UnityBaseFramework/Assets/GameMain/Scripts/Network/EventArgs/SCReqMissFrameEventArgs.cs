using BaseFramework;
using BaseFramework.Event;
using GameProto;
using System.Collections.Generic;

namespace XGame
{
    /// <summary>
    /// 服务器帧数据。
    /// </summary>
    public sealed class SCReqMissFrameEventArgs : GameEventArgs
    {
        /// <summary>
        /// 服务器帧数据事件编号。
        /// </summary>
        public static readonly int EventId = typeof(SCReqMissFrameEventArgs).GetHashCode();

        /// <summary>
        /// 初始化服务器帧数据返回事件的新实例。
        /// </summary>
        public SCReqMissFrameEventArgs()
        {
            ServerFrames = new();
            UserData = null;
        }

        /// <summary>
        /// 获取服务器帧数据事件编号。
        /// </summary>
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public int StartTick
        {
            get;
            private set;
        }

        public List<ServerFrame> ServerFrames
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
        /// 创建服务器帧数据事件。
        /// </summary>
        /// <param name="e">内部事件。</param>
        /// <returns></returns>
        public static SCReqMissFrameEventArgs Create(int startTick, List<ServerFrame> serverFrames, object userData = null)
        {
            SCReqMissFrameEventArgs SCReqMissFrameEventArgs = ReferencePool.Acquire<SCReqMissFrameEventArgs>();
            SCReqMissFrameEventArgs.StartTick = startTick;
            SCReqMissFrameEventArgs.ServerFrames.AddRange(serverFrames);
            SCReqMissFrameEventArgs.UserData = userData;
            return SCReqMissFrameEventArgs;
        }

        /// <summary>
        /// 清理服务器帧数据事件。
        /// </summary>
        public override void Clear()
        {
            StartTick = 0;
            ServerFrames.Clear();
            UserData = null;
        }
    }
}
