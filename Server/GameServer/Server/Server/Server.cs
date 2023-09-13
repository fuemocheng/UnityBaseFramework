using System.Net;
using Network;
using BaseFramework.Runtime;

namespace Server
{
    public class Server
    {
        private const double UpdateInterval = NetworkDefine.UPDATE_DELTATIME / 1000.0f; //更新间隔
        private DateTime m_LastUpdateTimeStamp; //最后一次更新的时间戳
        private DateTime m_LastUpdateFrameTimeStamp; //最后一帧更新的时间戳
        private DateTime m_StartupTimeStamp; //开始时间戳
        private double m_RealtimeSinceStartup;
        private double m_DeltaTime;

        public void Start()
        {
            Console.WriteLine("ServerStart");

            // 框架初始化。
            GameEntry.Awake();
            // 框架Start。
            GameEntry.Start();

            m_StartupTimeStamp = m_LastUpdateTimeStamp = m_LastUpdateFrameTimeStamp = DateTime.Now;
        }

        public void Update()
        {
            DateTime now = DateTime.Now;
            m_DeltaTime = (now - m_LastUpdateFrameTimeStamp).TotalSeconds;
            // 每30ms更新一次。
            if (m_DeltaTime > UpdateInterval)
            {
                m_LastUpdateFrameTimeStamp = now;
                m_RealtimeSinceStartup = (now - m_StartupTimeStamp).TotalSeconds;
                DoSlowUpdate(m_DeltaTime, m_DeltaTime);
            }

            // 每2ms更新一次。
            m_DeltaTime = (now - m_LastUpdateTimeStamp).TotalSeconds;
            DoFastUpdate(m_DeltaTime, m_DeltaTime);

            // 记录时间戳。
            m_LastUpdateTimeStamp = now;
        }


        /// <summary>
        /// 每2ms更新一次。
        /// </summary>
        /// <param name="elapseSeconds">帧间隔逻辑时间（可以缩放）。</param>
        /// <param name="realElapseSeconds">帧间隔真实时间（无缩放）。</param>
        public void DoFastUpdate(double elapseSeconds, double realElapseSeconds)
        {
            // 框架更新。
            GameEntry.FastUpdate((float)elapseSeconds, (float)realElapseSeconds);
        }

        /// <summary>
        /// 每30ms更新一次。
        /// </summary>
        public void DoSlowUpdate(double elapseSeconds, double realElapseSeconds)
        {
            GameEntry.SlowUpdate((float)elapseSeconds, (float)realElapseSeconds);
        }
    }
}
