using BaseFramework;

namespace BaseFramework.Runtime
{
    /// <summary>
    /// 引用池组件。
    /// </summary>
    public sealed class ReferencePoolComponent : BaseFrameworkComponent
    {
        private ReferenceStrictCheckType m_EnableStrictCheck = ReferenceStrictCheckType.AlwaysEnable;

        /// <summary>
        /// 获取或设置是否开启强制检查。
        /// </summary>
        public bool EnableStrictCheck
        {
            get
            {
                return ReferencePool.EnableStrictCheck;
            }
            set
            {
                ReferencePool.EnableStrictCheck = value;
                if (value)
                {
                    Log.Info("Strict checking is enabled for the Reference Pool. It will drastically affect the performance.");
                }
            }
        }

        /// <summary>
        /// 游戏框架组件初始化。
        /// </summary>
        public override void Awake()
        {
            base.Awake();
        }

        public override void Start()
        {
            switch (m_EnableStrictCheck)
            {
                case ReferenceStrictCheckType.AlwaysEnable:
                    EnableStrictCheck = true;
                    break;

                case ReferenceStrictCheckType.OnlyEnableWhenDevelopment:
                    EnableStrictCheck = true;
                    break;

                case ReferenceStrictCheckType.OnlyEnableInEditor:
                    EnableStrictCheck = false;
                    break;

                default:
                    EnableStrictCheck = false;
                    break;
            }
        }
    }
}
