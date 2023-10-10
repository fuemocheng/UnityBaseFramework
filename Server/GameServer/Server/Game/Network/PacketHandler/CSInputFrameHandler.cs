// <auto-generated>
//   This file will only be generated once
//   and will not be overwritten.
//   You need to implement the 'Handle' function yourself.
// </auto-generated>

using BaseFramework.Runtime;
using Network;
using Server;

namespace GameProto
{
    public partial class CSInputFrameHandler : PacketHandlerBase
    {
        public override void Handle(object sender, Packet packet)
        {
            CSInputFrame packetImpl = (CSInputFrame)packet;
            //Log.Info("Receive Packet Type:'{0}', Id:{1}", packetImpl.GetType().ToString(), packetImpl.Id.ToString());

            // Tcp Session��
            Session session = (Session)sender;
            // Server.User��
            Server.User user = (Server.User)session.BindInfo;

            user.Room.Game.ReceiveInput(user, packetImpl);
        }
    }
}
