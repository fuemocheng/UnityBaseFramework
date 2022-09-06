using System;
using System.Collections.Generic;

namespace BaseFramework
{
    /// <summary>
    /// 基础框架入口
    /// </summary>
    public static class BaseFrameworkEntry
    {
        private static readonly BaseFrameworkLinkedList<BaseFrameworkModule> s_BaseFrameworkModules = new BaseFrameworkLinkedList<BaseFrameworkModule>();

        /// <summary>
        /// 所有基础框架模块轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public static void Update(float elapseSeconds, float realElapseSeconds)
        {
            foreach (BaseFrameworkModule module in s_BaseFrameworkModules)
            {
                module.Update(elapseSeconds, realElapseSeconds);
            }
        }

        /// <summary>
        /// 关闭并清理所有基础框架模块。
        /// </summary>
        public static void ShutDown()
        {
            for (LinkedListNode<BaseFrameworkModule> current = s_BaseFrameworkModules.Last; current != null; current = current.Previous)
            {
                current.Value.ShotDown();
            }

            s_BaseFrameworkModules.Clear();
            ReferencePool.ClearAll();
            //TODO:释放缓存中的从进程的非托管内存中分配的内存。

            BaseFrameworkLog.SetLogHelper(null);
        }

        /// <summary>
        /// 获取基础框架模块。
        /// </summary>
        /// <typeparam name="T">要获取的模块类型。</typeparam>
        /// <returns>要获取的模块。</returns>
        /// <remarks>如果要获取的模块不存在，则自动创建该模块。</remarks>
        public static T GetModule<T>() where T : class
        {
            Type interfaceType = typeof(T);
            if (!interfaceType.IsInterface)
            {
                throw new BaseFrameworkException(Utility.Text.Format("You must get module by interface, but {0} is not.", interfaceType.FullName));
            }

            if (!interfaceType.FullName.StartsWith("BaseFramework.", StringComparison.Ordinal))
            {
                throw new BaseFrameworkException(Utility.Text.Format("You must get a Base Framework module, but {0} is not.", interfaceType.FullName));
            }

            string moduleName = Utility.Text.Format("{0}.{1}", interfaceType.Namespace, interfaceType.Name.Substring(1));
            Type moduleType = Type.GetType(moduleName);
            if (moduleType == null)
            {
                throw new BaseFrameworkException(Utility.Text.Format("Can not find Game Framework module type '{0}'.", moduleName));
            }

            return GetModule(moduleType) as T;
        }

        /// <summary>
        /// 获取基础框架模块。
        /// </summary>
        /// <param name="moduleType">要获取的模块类型。</param>
        /// <returns>要获取的模块。</returns>
        /// <remarks>如果要获取的模块不存在，则自动创建该模块。</remarks>
        private static BaseFrameworkModule GetModule(Type moduleType)
        {
            foreach (BaseFrameworkModule module in s_BaseFrameworkModules)
            {
                if (module.GetType() == moduleType)
                {
                    return module;
                }
            }

            return CreateModule(moduleType);
        }

        /// <summary>
        /// 创建模块。
        /// </summary>
        /// <param name="moduleType">要创建的模块类型。</param>
        /// <returns>要创建的模块。</returns>
        private static BaseFrameworkModule CreateModule(Type moduleType)
        {
            BaseFrameworkModule module = (BaseFrameworkModule)Activator.CreateInstance(moduleType);
            if (module == null)
            {
                throw new BaseFrameworkException(Utility.Text.Format("Can not create module '{0}'.", moduleType.FullName));
            }

            LinkedListNode<BaseFrameworkModule> current = s_BaseFrameworkModules.First;
            while (current != null)
            {
                if (module.Priority > current.Value.Priority)
                {
                    break;
                }

                current = current.Next;
            }

            if (current != null)
            {
                s_BaseFrameworkModules.AddBefore(current, module);
            }
            else
            {
                s_BaseFrameworkModules.AddLast(module);
            }

            return module;
        }
    }
}
