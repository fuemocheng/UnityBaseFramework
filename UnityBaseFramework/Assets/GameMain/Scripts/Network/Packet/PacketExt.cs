// <auto-generated>
//   This file was generated by a tool;
//   you should avoid making direct changes.
//   These types are extended from proto classes.
// </auto-generated>

using XGame;

namespace GameProto
{
    public partial class CSHeartBeat : CSPacketBase
    {
        public override int Id => 1;

        public override void Clear()
        {
        }
    }

    public partial class SCHeartBeat : SCPacketBase
    {
        public override int Id => 2;

        public override void Clear()
        {
        }
    }

    public partial class SCHeartBeatHandler : PacketHandlerBase
    {
        public override int Id => 2;
    }

    public partial class CSLogin : CSPacketBase
    {
        public override int Id => 3;

        public override void Clear()
        {
            Account = default;
            Password = default;
        }
    }

    public partial class SCLogin : SCPacketBase
    {
        public override int Id => 4;

        public override void Clear()
        {
            State = 0;
        }
    }

    public partial class SCLoginHandler : PacketHandlerBase
    {
        public override int Id => 4;
    }

}
