using System.Reflection;

namespace BaseFramework.Runtime
{
    /// <summary>
    /// 框架入口。
    /// </summary>
    public static class BaseEntry
    {
        private static readonly BaseFrameworkLinkedList<BaseFrameworkComponent> s_BaseFrameworkComponents = new BaseFrameworkLinkedList<BaseFrameworkComponent>();

        /// <summary>
        /// 游戏框架所在的场景编号。
        /// </summary>
        internal const int BaseFrameworkSceneId = 0;

        /// <summary>
        /// 获取游戏框架组件。
        /// </summary>
        /// <typeparam name="T">要获取的游戏框架组件类型。</typeparam>
        /// <returns>要获取的游戏框架组件。</returns>
        public static T GetComponent<T>() where T : BaseFrameworkComponent
        {
            return (T)GetComponent(typeof(T));
        }

        /// <summary>
        /// 获取游戏框架组件。
        /// </summary>
        /// <param name="type">要获取的游戏框架组件类型。</param>
        /// <returns>要获取的游戏框架组件。</returns>
        public static BaseFrameworkComponent GetComponent(Type type)
        {
            LinkedListNode<BaseFrameworkComponent> current = s_BaseFrameworkComponents.First;
            while (current != null)
            {
                if (current.Value.GetType() == type)
                {
                    return current.Value;
                }

                current = current.Next;
            }

            return null;
        }

        /// <summary>
        /// 获取游戏框架组件。
        /// </summary>
        /// <param name="typeName">要获取的游戏框架组件类型名称。</param>
        /// <returns>要获取的游戏框架组件。</returns>
        public static BaseFrameworkComponent GetComponent(string typeName)
        {
            LinkedListNode<BaseFrameworkComponent> current = s_BaseFrameworkComponents.First;
            while (current != null)
            {
                Type type = current.Value.GetType();
                if (type.FullName == typeName || type.Name == typeName)
                {
                    return current.Value;
                }

                current = current.Next;
            }

            return null;
        }

        /// <summary>
        /// 关闭游戏框架。
        /// </summary>
        /// <param name="shutdownType">关闭游戏框架类型。</param>
        public static void Shutdown(ShutdownType shutdownType)
        {
            Log.Info("Shutdown Base Framework ({0})...", shutdownType);
            BaseComponent baseComponent = GetComponent<BaseComponent>();
            if (baseComponent != null)
            {
                baseComponent.Shutdown();
                baseComponent = null;
            }

            s_BaseFrameworkComponents.Clear();

            if (shutdownType == ShutdownType.None)
            {
                return;
            }

            if (shutdownType == ShutdownType.Restart)
            {
                return;
            }

            if (shutdownType == ShutdownType.Quit)
            {
                return;
            }
        }

        /// <summary>
        /// 注册游戏框架组件。
        /// </summary>
        /// <param name="baseFrameworkComponent">要注册的游戏框架组件。</param>
        internal static void RegisterComponent(BaseFrameworkComponent baseFrameworkComponent)
        {
            if (baseFrameworkComponent == null)
            {
                Log.Error("Base Framework component is invalid.");
                return;
            }

            Type type = baseFrameworkComponent.GetType();

            LinkedListNode<BaseFrameworkComponent> current = s_BaseFrameworkComponents.First;
            while (current != null)
            {
                if (current.Value.GetType() == type)
                {
                    Log.Error("Base Framework component type '{0}' is already exist.", type.FullName);
                    return;
                }

                current = current.Next;
            }

            s_BaseFrameworkComponents.AddLast(baseFrameworkComponent);
        }

        /// <summary>
        /// BaseFramework 入口 Awake。
        /// </summary>
        public static void Awake()
        {
            // 反射注册组件。
            Type baseFrameworkComponentType = typeof(BaseFrameworkComponent);
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                if (!types[i].IsClass || types[i].IsAbstract)
                {
                    continue;
                }

                if (types[i].BaseType == baseFrameworkComponentType)
                {
                    BaseFrameworkComponent component = (BaseFrameworkComponent)Activator.CreateInstance(types[i]);
                    //将 继承 BaseFrameworkComponent 的组件注册进 BaseEntry。
                    component.Awake();
                }
            }
        }

        /// <summary>
        /// BaseFramework 入口 Start。
        /// </summary>
        public static void Start()
        {
            foreach(BaseFrameworkComponent component in s_BaseFrameworkComponents)
            {
                component.Start();
            }
        }

        /// <summary>
        /// BaseFramework 入口 Update。
        /// </summary>
        public static void Update(float elapseSeconds, float realElapseSeconds)
        {
            BaseFrameworkEntry.Update(elapseSeconds, realElapseSeconds);
        }

        /// <summary>
        /// BaseFramework 入口 Shutdown。
        /// </summary>
        public static void Shutdown()
        {
            BaseFrameworkEntry.Shutdown();
            BaseEntry.Shutdown(ShutdownType.Quit);
        }
    }
}
