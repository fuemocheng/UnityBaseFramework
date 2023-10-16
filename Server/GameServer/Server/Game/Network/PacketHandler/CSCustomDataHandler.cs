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
    public partial class CSCustomDataHandler : PacketHandlerBase
    {
        public override void Handle(object sender, Packet packet)
        {
            CSCustomData packetImpl = (CSCustomData)packet;
            Log.Info("Receive packet '{0}'.", packetImpl.GetType().ToString());


            Session session = (Session)sender;
            // User��
            Server.User user = (Server.User)session.BindInfo;

            SCCustomData sCCustomData = ReferencePool.Acquire<SCCustomData>();
            sCCustomData.CustomData = "S2C:" + packetImpl.CustomData;

            Log.Error($"CSCustomDataHandler User:{user.Account} ChannelID:{user.TcpSession.Channel.Id} Data:{packetImpl.CustomData}");

            user.TcpSession?.Send(sCCustomData);
        }
    }
}
