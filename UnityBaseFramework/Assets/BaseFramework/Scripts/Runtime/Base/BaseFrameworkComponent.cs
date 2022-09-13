using UnityEngine;

namespace UnityBaseFramework.Runtime
{
    /// <summary>
    /// 基础框架组件抽象类。
    /// </summary>
    public abstract class BaseFrameworkComponent : MonoBehaviour
    {
        /// <summary>
        /// 组件初始化。
        /// </summary>
        protected virtual void Awake()
        {
            BaseEntry.RegisterComponent(this);
        }
    }
}
