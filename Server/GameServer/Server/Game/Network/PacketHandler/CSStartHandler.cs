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
    public partial class CSStartHandler : PacketHandlerBase
    {
        public override void Handle(object sender, Packet packet)
        {
            CSStart packetImpl = (CSStart)packet;
            Log.Info("Receive Packet Type:'{0}', Id:{1}", packetImpl.GetType().ToString(), packetImpl.Id.ToString());

            // Tcp Session。
            Session session = (Session)sender;
            // User。
            User user = (User)session.BindInfo;
            user.IsStarted = true;

            Room room = user.Room;
            if (room == null)
            {
                throw new Exception("CSStartHandler User Started Game, but room is null");
            }

            //所有人都准备完成，通知所有客户端开始游戏。
            if(room.IsAllStarted())
            {
                room.SendAllClientStartGame();
            }
        }
    }
}
