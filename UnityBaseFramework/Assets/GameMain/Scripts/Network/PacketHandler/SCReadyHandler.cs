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
            Log.Info("Receive packet '{0}'.", packetImpl.Id.ToString());
        }
    }
}
