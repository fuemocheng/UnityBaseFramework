using UnityEditor;
using UnityEngine;

namespace UnityBaseFramework.Editor
{
    /// <summary>
    /// 帮助相关的实用函数。
    /// </summary>
    public static class Help
    {
        //[MenuItem("Game Framework/Documentation", false, 90)]
        //public static void ShowDocumentation()
        //{
        //    ShowHelp("https://gameframework.cn/document/");
        //}

        //[MenuItem("Game Framework/API Reference", false, 91)]
        //public static void ShowApiReference()
        //{
        //    ShowHelp("https://gameframework.cn/api/");
        //}

        [MenuItem("Base Framework/Tutorial", false, 91)]
        public static void ShowApiReference()
        {
            ShowHelp("https://github.com/fuemocheng/UnityBaseFramework");
        }

        private static void ShowHelp(string uri)
        {
            Application.OpenURL(uri);
        }
    }
}
