// <auto-generated>
//   This file will only be generated once
//   and will not be overwritten.
//   You need to implement the 'Handle' function yourself.
// </auto-generated>

using BaseFramework.Network;
using UnityBaseFramework.Runtime;
using XGame;

namespace GameProto
{
    public partial class SCGameStartInfoHandler : PacketHandlerBase
    {
        public override void Handle(object sender, Packet packet)
        {
            SCGameStartInfo packetImpl = (SCGameStartInfo)packet;
            Log.Info("Receive Packet Type:'{0}'", packetImpl.GetType().ToString());

            GameEntry.Event.Fire(sender, SCGameStartInfoEventArgs.Create(packetImpl.RoomId, packetImpl.MapId, packetImpl.LocalId, packetImpl.UserCount, packetImpl.Seed, packetImpl.UserGameInfos));
        }
    }
}
