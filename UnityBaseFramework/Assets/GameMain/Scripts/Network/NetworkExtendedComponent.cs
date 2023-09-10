using BaseFramework.Network;
using UnityBaseFramework.Runtime;

namespace XGame
{
    public class NetworkExtendedComponent : BaseFrameworkComponent
    {
        private bool m_IsDelayStarted = false;
        public INetworkChannelHelper NetworkChannelHelper = null;
        public INetworkChannel TcpChannel = null;

        protected override void Awake()
        {
            base.Awake();
        }


        void Start()
        {
            NetworkChannelHelper = new NetworkChannelHelper();
        }

        private void DelayStart()
        {
            TcpChannel = GameEntry.Network.CreateNetworkChannel("TcpChannel", ServiceType.Tcp, NetworkChannelHelper);
        }

        void Update()
        {
            if(!m_IsDelayStarted)
            {
                m_IsDelayStarted = true;
                DelayStart();
            }
        }
    }
}
