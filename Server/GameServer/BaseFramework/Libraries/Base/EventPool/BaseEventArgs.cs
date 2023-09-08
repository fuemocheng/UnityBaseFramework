namespace BaseFramework
{
    /// <summary>
    /// 事件基类。
    /// </summary>
    public abstract class BaseEventArgs : BaseFrameworkEventArgs
    {
        /// <summary>
        /// 获取类型编号。
        /// </summary>
        public abstract int Id
        {
            get;
        }
    }
}
