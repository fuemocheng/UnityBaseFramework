using System;

namespace BaseFramework
{
    /// <summary>
    /// 基础框架中包含事件数据的类的基类。
    /// </summary>
    public abstract class BaseFrameworkEventArgs : EventArgs, IReference
    {
        /// <summary>
        /// 初始化基础框架中包含事件数据的类的新实例。
        /// </summary>
        public BaseFrameworkEventArgs()
        { 
        }

        /// <summary>
        /// 清理引用。
        /// </summary>
        public abstract void Clear();
    }
}
