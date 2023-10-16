// <auto-generated>
//   This file was generated by a tool;
//   you should avoid making direct changes.
//   These types are extended from proto classes.
// </auto-generated>

using Network;

namespace GameProto
{
    public partial class CSHeartBeat : CSPacketBase
    {
        public override int Id => 1;

        public override void Clear()
        {
        }
    }

    public partial class CSHeartBeatHandler : PacketHandlerBase
    {
        public override int Id => 1;
    }

    public partial class SCHeartBeat : SCPacketBase
    {
        public override int Id => 2;

        public override void Clear()
        {
        }
    }

    public partial class CSPing : CSPacketBase
    {
        public override int Id => 3;

        public override void Clear()
        {
            LocalId = 0;
            SendTimestamp = default;
        }
    }

    public partial class CSPingHandler : PacketHandlerBase
    {
        public override int Id => 3;
    }

    public partial class SCPing : SCPacketBase
    {
        public override int Id => 4;

        public override void Clear()
        {
            LocalId = 0;
            SendTimestamp = default;
            TimeSinceServerStart = default;
        }
    }

    public partial class CSCustomData : CSPacketBase
    {
        public override int Id => 5;

        public override void Clear()
        {
            CustomData = default;
        }
    }

    public partial class CSCustomDataHandler : PacketHandlerBase
    {
        public override int Id => 5;
    }

    public partial class SCCustomData : SCPacketBase
    {
        public override int Id => 6;

        public override void Clear()
        {
            CustomData = default;
        }
    }

    public partial class CSLogin : CSPacketBase
    {
        public override int Id => 7;

        public override void Clear()
        {
            Account = default;
            Password = default;
        }
    }

    public partial class CSLoginHandler : PacketHandlerBase
    {
        public override int Id => 7;
    }

    public partial class SCLogin : SCPacketBase
    {
        public override int Id => 8;

        public override void Clear()
        {
            RetCode = 0;
            UserState = 0;
        }
    }

    public partial class CSJoinRoom : CSPacketBase
    {
        public override int Id => 9;

        public override void Clear()
        {
            RoomId = 0;
        }
    }

    public partial class CSJoinRoomHandler : PacketHandlerBase
    {
        public override int Id => 9;
    }

    public partial class SCJoinRoom : SCPacketBase
    {
        public override int Id => 10;

        public override void Clear()
        {
            RoomId = 0;
            LocalId = 0;
            UserReadyInfos.Clear();
        }
    }

    public partial class CSReady : CSPacketBase
    {
        public override int Id => 11;

        public override void Clear()
        {
            UserState = 0;
        }
    }

    public partial class CSReadyHandler : PacketHandlerBase
    {
        public override int Id => 11;
    }

    public partial class SCReady : SCPacketBase
    {
        public override int Id => 12;

        public override void Clear()
        {
            RoomId = 0;
            LocalId = 0;
            UserReadyInfos.Clear();
        }
    }

    public partial class CSGameStartInfo : CSPacketBase
    {
        public override int Id => 13;

        public override void Clear()
        {
        }
    }

    public partial class CSGameStartInfoHandler : PacketHandlerBase
    {
        public override int Id => 13;
    }

    public partial class SCGameStartInfo : SCPacketBase
    {
        public override int Id => 14;

        public override void Clear()
        {
            RoomId = 0;
            MapId = 0;
            LocalId = 0;
            UserCount = 0;
            Seed = 0;
            Users.Clear();
        }
    }

    public partial class CSLoadingProgress : CSPacketBase
    {
        public override int Id => 15;

        public override void Clear()
        {
            Progress = 0;
        }
    }

    public partial class CSLoadingProgressHandler : PacketHandlerBase
    {
        public override int Id => 15;
    }

    public partial class SCLoadingProgress : SCPacketBase
    {
        public override int Id => 16;

        public override void Clear()
        {
            AllProgress = 0;
        }
    }

    public partial class CSInputFrame : CSPacketBase
    {
        public override int Id => 17;

        public override void Clear()
        {
            InputFrame = default;
        }
    }

    public partial class CSInputFrameHandler : PacketHandlerBase
    {
        public override int Id => 17;
    }

    public partial class SCServerFrame : SCPacketBase
    {
        public override int Id => 18;

        public override void Clear()
        {
            StartTick = 0;
            ServerFrames.Clear();
        }
    }

    public partial class CSReqMissFrame : CSPacketBase
    {
        public override int Id => 19;

        public override void Clear()
        {
            StartTick = 0;
        }
    }

    public partial class CSReqMissFrameHandler : PacketHandlerBase
    {
        public override int Id => 19;
    }

    public partial class SCReqMissFrame : SCPacketBase
    {
        public override int Id => 20;

        public override void Clear()
        {
            StartTick = 0;
            ServerFrames.Clear();
        }
    }

}
