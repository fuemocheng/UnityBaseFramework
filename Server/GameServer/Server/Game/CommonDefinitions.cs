namespace Server
{
    /// <summary>
    /// 通用定义。
    /// </summary>
    public class CommonDefinitions
    {
        /// <summary>
        /// 游戏房间最大人数。
        /// </summary>
        public const int MaxRoomMemberCount = 1;

        /// <summary>
        /// 游戏逻辑帧更新间隔。
        /// </summary>
        public const int UpdateDeltatime = 30;

        /// <summary>
        /// 延迟发送加载进度时间。
        /// </summary>
        public const float DelaySendLoadingProgressTime = 2;

        /// <summary>
        /// 服务器比客户端延迟执行的帧数。
        /// </summary>
        public const int ServerDelayTick = 10;
    }
}
