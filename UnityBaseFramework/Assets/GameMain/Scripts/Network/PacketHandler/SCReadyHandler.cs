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
    public partial class SCReadyHandler : PacketHandlerBase
    {
        public override void Handle(object sender, Packet packet)
        {
            SCReady packetImpl = (SCReady)packet;
            Log.Info("Receive Packet Type:'{0}', Id:{1}", packetImpl.GetType().ToString(), packetImpl.Id.ToString());

            GameEntry.Event.Fire(sender, SCReadyEventArgs.Create(packetImpl.UserReadyInfos));
        }
    }
}