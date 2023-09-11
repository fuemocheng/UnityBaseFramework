using ProtoBuf;
using XGame;

namespace GameProto
{
    [ProtoContract]
    public sealed class SCPacketHeader : PacketHeaderBase
    {
        public override PacketType PacketType
        {
            get
            {
                return PacketType.ServerToClient;
            }
        }

        [ProtoMember(1, DataFormat = DataFormat.FixedSize)]
        public override int Id
        {
            get;
            set;
        }

        [ProtoMember(2, DataFormat = DataFormat.FixedSize)]
        public override int PacketLength
        {
            get;
            set;
        }
    }
}
