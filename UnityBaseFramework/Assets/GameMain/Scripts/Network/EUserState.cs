namespace XGame
{
    public enum EUserState
    {
        Default,            //默认无状态。
        LoggedIn,           //已登录。
        NotReady,           //在房间中，未准备。
        Ready,              //在房间中，已准备。
        Loading,            //加载中。
        Playing,            //在游戏中，已开始游戏。
    }
}
