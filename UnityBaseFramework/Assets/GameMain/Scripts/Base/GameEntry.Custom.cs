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

        public static NetworkExtendedComponent NetworkExtended
        {
            get;
            private set;
        }

        private static void InitCustomComponents()
        {
            BuiltinData = UnityBaseFramework.Runtime.BaseEntry.GetComponent<BuiltinDataComponent>();
            NetworkExtended = UnityBaseFramework.Runtime.BaseEntry.GetComponent<NetworkExtendedComponent>();
        }
    }
}
