using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGame
{
    /// <summary>
    /// 游戏入口。
    /// </summary>
    public partial class GameEntry : MonoBehaviour
    {
        public static BuiltinDataComponent BuiltinData
        {
            get;
            private set;
        }

        //public static HPBarComponent HPBar
        //{
        //    get;
        //    private set;
        //}

        private static void InitCustomComponents()
        {
            BuiltinData = UnityBaseFramework.Runtime.BaseEntry.GetComponent<BuiltinDataComponent>();
            //HPBar = UnityBaseFramework.Runtime.BaseEntry.GetComponent<HPBarComponent>();
        }
    }
}
