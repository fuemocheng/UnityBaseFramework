namespace BaseFramework.Runtime
{
    /// <summary>
    /// 游戏框架组件抽象类。
    /// </summary>
    public abstract class BaseFrameworkComponent
    {
        /// <summary>
        /// 游戏框架组件初始化。
        /// </summary>
        public virtual void Awake()
        {
            BaseEntry.RegisterComponent(this);
        }

        /// <summary>
        /// 游戏框架组件Start。
        /// </summary>
        public virtual void Start()
        {
        }
    }
}
