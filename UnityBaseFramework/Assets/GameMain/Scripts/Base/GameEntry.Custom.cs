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

        public static ServiceComponent Service
        {
            get;
            private set;
        }

        public static GameLogicComponent GameLogic;

        private static void InitCustomComponents()
        {
            BuiltinData = UnityBaseFramework.Runtime.BaseEntry.GetComponent<BuiltinDataComponent>();
            NetworkExtended = UnityBaseFramework.Runtime.BaseEntry.GetComponent<NetworkExtendedComponent>();
            Service = UnityBaseFramework.Runtime.BaseEntry.GetComponent<ServiceComponent>();
            GameLogic = UnityBaseFramework.Runtime.BaseEntry.GetComponent<GameLogicComponent>();
        }
    }
}
