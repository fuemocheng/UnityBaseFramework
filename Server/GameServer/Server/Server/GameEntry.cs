using BaseFramework.Runtime;
using Network;
using System.Net;

namespace Server
{
    public static class GameEntry
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
        /// 获取事件组件。
        /// </summary>
        public static EventComponent Event
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
        /// 网络组件，不继承 BaseFrameworkComponent。
        /// </summary>
        public static NetworkComponent Network
        {
            get;
            private set;
        }

        /// <summary>
        /// 游戏逻辑，不继承 BaseFrameworkComponent。
        /// </summary>
        public static GameLogicComponent GameLogic
        {
            get;
            private set;
        }

        private static void InitBuiltinComponents()
        {
            Base = BaseEntry.GetComponent<BaseComponent>();
            Event = BaseEntry.GetComponent<EventComponent>();
            Fsm = BaseEntry.GetComponent<FsmComponent>();
            ObjectPool = BaseEntry.GetComponent<ObjectPoolComponent>();
            Procedure = BaseEntry.GetComponent<ProcedureComponent>();
        }

        public static void Awake()
        {
            BaseEntry.Awake();

            // 网络组件初始化
            Network = new NetworkComponent();
            Network.Awake();

            // 游戏逻辑初始化。
            GameLogic = new GameLogicComponent();
            GameLogic.Awake();
        }

        public static void Start()
        {
            BaseEntry.Start();
            InitBuiltinComponents();

            // 创建TcpService。
            IPEndPoint serverIPEndpoint = new IPEndPoint(IPAddress.Any, 9001);
            Network.Start(NetworkProtocolType.TCP, serverIPEndpoint);

            // 游戏逻辑 Start。
            GameLogic.Start();
        }

        /// <summary>
        /// 每2ms更新一次。
        /// </summary>
        /// <param name="elapseSeconds"></param>
        /// <param name="realElapseSeconds"></param>
        public static void FastUpdate(float elapseSeconds, float realElapseSeconds)
        {
            BaseEntry.Update(elapseSeconds, realElapseSeconds);

            // 网络更新。
            Network.Update((float)elapseSeconds, (float)realElapseSeconds);
        }

        /// <summary>
        /// 每30ms更新一次。
        /// </summary>
        /// <param name="elapseSeconds"></param>
        /// <param name="realElapseSeconds"></param>
        public static void SlowUpdate(float elapseSeconds, float realElapseSeconds)
        {
            // 游戏逻辑更新。
            GameLogic.Update(elapseSeconds, realElapseSeconds);
        }
    }
}
