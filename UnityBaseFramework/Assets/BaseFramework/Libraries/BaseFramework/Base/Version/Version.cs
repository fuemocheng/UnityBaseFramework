namespace BaseFramework
{
    /// <summary>
    /// 版本号类。
    /// </summary>
    public static partial class Version
    {
        private const string BaseFrameworkVersionString = "2022.07.31";

        private static IVersionHelper s_VersionHelper = null;

        /// <summary>
        /// 获取游戏框架版本号。
        /// </summary>
        public static string BaseFrameworkVersion
        {
            get
            {
                return BaseFrameworkVersionString;
            }
        }

        /// <summary>
        /// 获取游戏版本号。
        /// </summary>
        public static string GameVersion
        {
            get
            {
                if (s_VersionHelper == null)
                {
                    return string.Empty;
                }

                return s_VersionHelper.GameVersion;
            }
        }

        /// <summary>
        /// 获取内部游戏版本号。
        /// </summary>
        public static int InternalGameVersion
        {
            get
            {
                if (s_VersionHelper == null)
                {
                    return 0;
                }

                return s_VersionHelper.InternalGameVersion;
            }
        }

        /// <summary>
        /// 设置版本号辅助器。
        /// </summary>
        /// <param name="versionHelper">要设置的版本号辅助器。</param>
        public static void SetVersionHelper(IVersionHelper versionHelper)
        {
            s_VersionHelper = versionHelper;
        }
    }
}
