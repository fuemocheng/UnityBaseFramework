namespace XGame
{
    /// <summary>
    /// 界面编号。
    /// </summary>
    public enum UIFormId : byte
    {
        Undefined = 0,

        /// <summary>
        /// 弹出框。
        /// </summary>
        DialogForm = 1,

        /// <summary>
        /// 登录。
        /// </summary>
        LoginForm = 10,

        /// <summary>
        /// 单机模式。
        /// </summary>
        ClientModeForm = 11,

        /// <summary>
        /// 大厅。
        /// </summary>
        LobbyForm = 20,

        /// <summary>
        /// 主界面。
        /// </summary>
        MainUIForm = 30,
    }
}
