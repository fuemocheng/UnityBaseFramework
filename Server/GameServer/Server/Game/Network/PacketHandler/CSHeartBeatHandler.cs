// <auto-generated>
//   This file will only be generated once
//   and will not be overwritten.
//   You need to implement the 'Handle' function yourself.
// </auto-generated>

using BaseFramework;
using BaseFramework.Runtime;
using Network;
using Server;

namespace GameProto
{
    public partial class CSHeartBeatHandler : PacketHandlerBase
    {
        public override void Handle(object sender, Packet packet)
        {
            CSHeartBeat packetImpl = (CSHeartBeat)packet;
            //Log.Info("Receive Packet Type:'{0}'", packetImpl.GetType().ToString());

            Session session = (Session)sender;

            // 返回 Client 心跳消息包。
            // CSHeartBeat -> SCHeartBeat。
            session?.Send(ReferencePool.Acquire<SCHeartBeat>());
        }
    }
}
