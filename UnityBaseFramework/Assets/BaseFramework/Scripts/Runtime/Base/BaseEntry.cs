using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using BaseFramework;

namespace UnityBaseFramework.Runtime
{
    /// <summary>
    /// 框架入口。
    /// </summary>
    public static class BaseEntry
    {
        private static readonly BaseFrameworkLinkedList<BaseFrameworkComponent> s_BaseFrameworkCompomemts = new BaseFrameworkLinkedList<BaseFrameworkComponent>();

        /// <summary>
        /// 基础框架所在的场景编号。
        /// </summary>
        internal const int BaseFrameworkSceneId = 0;

        /// <summary>
        /// 获取框架组件。
        /// </summary>
        /// <typeparam name="T">要获取的组件类型。</typeparam>
        /// <returns>要获取的组件。</returns>
        public static T GetComponent<T>() where T : BaseFrameworkComponent
        {
            return (T)GetComponent(typeof(T));
        }

        /// <summary>
        /// 获取框架组件。
        /// </summary>
        /// <param name="type">要获取的组件类型。</param>
        /// <returns>要获取的组件。</returns>
        public static BaseFrameworkComponent GetComponent(Type type)
        {
            LinkedListNode<BaseFrameworkComponent> current = s_BaseFrameworkCompomemts.First;
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
        /// 获取框架组件。
        /// </summary>
        /// <param name="typeName">要获取的组件类型名称。</param>
        /// <returns>要获取的组件。</returns>
        public static BaseFrameworkComponent GetComponent(string typeName)
        {
            LinkedListNode<BaseFrameworkComponent> current = s_BaseFrameworkCompomemts.First;
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
        /// 关闭框架。
        /// </summary>
        /// <param name="shutdownType">关闭类型。</param>
        public static void Shutdown(ShutdownType shutdownType)
        {
            Log.Info("Shutdown Base Framework {0}...", shutdownType);
            BaseComponent baseComponent = GetComponent<BaseComponent>();
            if (baseComponent != null)
            {
                baseComponent.Shutdown();
                baseComponent = null;
            }

            s_BaseFrameworkCompomemts.Clear();

            if (shutdownType == ShutdownType.None)
            {
                return;
            }

            if (shutdownType == ShutdownType.Restart)
            {
                SceneManager.LoadScene(BaseFrameworkSceneId);
                return;
            }

            if (shutdownType == ShutdownType.Quit)
            {
                Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
                return;
            }
        }

        /// <summary>
        /// 注册基础框架组件。
        /// </summary>
        /// <param name="baseFrameworkComponent">要注册的组件。</param>
        internal static void RegisterComponent(BaseFrameworkComponent baseFrameworkComponent)
        {
            if (baseFrameworkComponent == null)
            {
                Log.Error("Base Framework component is invalid.");
                return;
            }

            Type type = baseFrameworkComponent.GetType();

            LinkedListNode<BaseFrameworkComponent> current = s_BaseFrameworkCompomemts.First;
            while (current != null)
            {
                if (current.Value.GetType() == type)
                {
                    Log.Error("Base Framework component type '{0}' is already exist.", type.FullName);
                    return;
                }

                current = current.Next;
            }

            s_BaseFrameworkCompomemts.AddLast(baseFrameworkComponent);
        }
    }
}
