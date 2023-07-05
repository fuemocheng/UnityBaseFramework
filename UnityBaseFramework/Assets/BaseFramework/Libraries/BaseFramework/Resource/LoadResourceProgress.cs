namespace BaseFramework.Resource
{
    /// <summary>
    /// 加载资源进度类型。
    /// </summary>
    public enum LoadResourceProgress : byte
    {
        /// <summary>
        /// 未知类型。
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// 读取资源包。
        /// </summary>
        ReadResource,

        /// <summary>
        /// 加载资源包。
        /// </summary>
        LoadResource,

        /// <summary>
        /// 加载资源。
        /// </summary>
        LoadAsset,

        /// <summary>
        /// 加载场景。
        /// </summary>
        LoadScene
    }
}
