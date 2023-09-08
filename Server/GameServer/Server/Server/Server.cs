using System.Net;
using Network;
using BaseFramework.Runtime;

namespace Server
{
    public class Server
    {
        public IPEndPoint ServerIPEndpoint = new IPEndPoint(IPAddress.Any, 9001);

        private NetworkProxy m_NetworkProxy = new NetworkProxy(); //网络代理

        private const double UpdateInterval = NetworkDefine.UPDATE_DELTATIME / 1000.0f; //更新间隔
        private DateTime m_LastUpdateTimeStamp; //最后一次更新的时间戳
        private DateTime m_StartupTimeStamp; //开始时间戳
        private double m_RealtimeSinceStartup;
        private double m_DeltaTime;

        public void Start()
        {
            Console.WriteLine("ServerStart");
            Log.Info("ServerStart");

            // 框架初始化。
            BaseEntry.Awake();
            // 框架Start。
            BaseEntry.Start();

            m_NetworkProxy.Awake();
            // 创建TcpService。
            m_NetworkProxy.Start(NetworkProtocolType.TCP, ServerIPEndpoint);

            m_StartupTimeStamp = m_LastUpdateTimeStamp = DateTime.Now;
        }

        public void Update()
        {
            DateTime now = DateTime.Now;
            m_DeltaTime = (now - m_LastUpdateTimeStamp).TotalSeconds;
            if (m_DeltaTime > UpdateInterval)
            {
                m_LastUpdateTimeStamp = now;
                m_RealtimeSinceStartup = (now - m_StartupTimeStamp).TotalSeconds;
                DoUpdate();
            }

            // 框架更新。
            BaseEntry.Update((float)m_DeltaTime, (float)m_DeltaTime);

            // 网络更新。
            m_NetworkProxy.Update();
        }

        public void DoUpdate()
        {
            //Console.WriteLine("DoUpdate");
        }
    }
}
