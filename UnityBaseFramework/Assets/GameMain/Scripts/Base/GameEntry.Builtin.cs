using UnityEngine;
using UnityBaseFramework.Runtime;

namespace XGame
{
    /// <summary>
    /// 游戏入口。
    /// </summary>
    public partial class GameEntry : MonoBehaviour
    {
        /// <summary>
        /// 获取游戏基础组件。
        /// </summary>
        public static BaseComponent Base
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取配置组件。
        /// </summary>
        public static ConfigComponent Config
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取数据结点组件。
        /// </summary>
        public static DataNodeComponent DataNode
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取数据表组件。
        /// </summary>
        public static DataTableComponent DataTable
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取调试组件。
        /// </summary>
        public static DebuggerComponent Debugger
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取下载组件。
        /// </summary>
        public static DownloadComponent Download
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取实体组件。
        /// </summary>
        //public static EntityComponent Entity
        //{
        //    get;
        //    private set;
        //}

        /// <summary>
        /// 获取事件组件。
        /// </summary>
        public static EventComponent Event
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取文件系统组件。
        /// </summary>
        public static FileSystemComponent FileSystem
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取有限状态机组件。
        /// </summary>
        public static FsmComponent Fsm
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取本地化组件。
        /// </summary>
        public static LocalizationComponent Localization
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取网络组件。
        /// </summary>
        //public static NetworkComponent Network
        //{
        //    get;
        //    private set;
        //}

        /// <summary>
        /// 获取对象池组件。
        /// </summary>
        public static ObjectPoolComponent ObjectPool
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取流程组件。
        /// </summary>
        public static ProcedureComponent Procedure
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取资源组件。
        /// </summary>
        public static ResourceComponent Resource
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取场景组件。
        /// </summary>
        public static SceneComponent Scene
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取配置组件。
        /// </summary>
        public static SettingComponent Setting
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取声音组件。
        /// </summary>
        //public static SoundComponent Sound
        //{
        //    get;
        //    private set;
        //}

        /// <summary>
        /// 获取界面组件。
        /// </summary>
        //public static UIComponent UI
        //{
        //    get;
        //    private set;
        //}

        /// <summary>
        /// 获取网络组件。
        /// </summary>
        public static WebRequestComponent WebRequest
        {
            get;
            private set;
        }

        private static void InitBuiltinComponents()
        {
            Base = UnityBaseFramework.Runtime.BaseEntry.GetComponent<BaseComponent>();
            Config = UnityBaseFramework.Runtime.BaseEntry.GetComponent<ConfigComponent>();
            DataNode = UnityBaseFramework.Runtime.BaseEntry.GetComponent<DataNodeComponent>();
            DataTable = UnityBaseFramework.Runtime.BaseEntry.GetComponent<DataTableComponent>();
            Debugger = UnityBaseFramework.Runtime.BaseEntry.GetComponent<DebuggerComponent>();
            Download = UnityBaseFramework.Runtime.BaseEntry.GetComponent<DownloadComponent>();
            //Entity = UnityBaseFramework.Runtime.BaseEntry.GetComponent<EntityComponent>();
            Event = UnityBaseFramework.Runtime.BaseEntry.GetComponent<EventComponent>();
            FileSystem = UnityBaseFramework.Runtime.BaseEntry.GetComponent<FileSystemComponent>();
            Fsm = UnityBaseFramework.Runtime.BaseEntry.GetComponent<FsmComponent>();
            Localization = UnityBaseFramework.Runtime.BaseEntry.GetComponent<LocalizationComponent>();
            //Network = UnityBaseFramework.Runtime.BaseEntry.GetComponent<NetworkComponent>();
            ObjectPool = UnityBaseFramework.Runtime.BaseEntry.GetComponent<ObjectPoolComponent>();
            Procedure = UnityBaseFramework.Runtime.BaseEntry.GetComponent<ProcedureComponent>();
            Resource = UnityBaseFramework.Runtime.BaseEntry.GetComponent<ResourceComponent>();
            Scene = UnityBaseFramework.Runtime.BaseEntry.GetComponent<SceneComponent>();
            Setting = UnityBaseFramework.Runtime.BaseEntry.GetComponent<SettingComponent>();
            //Sound = UnityBaseFramework.Runtime.BaseEntry.GetComponent<SoundComponent>();
            //UI = UnityBaseFramework.Runtime.BaseEntry.GetComponent<UIComponent>();
            WebRequest = UnityBaseFramework.Runtime.BaseEntry.GetComponent<WebRequestComponent>();
        }
    }
}
