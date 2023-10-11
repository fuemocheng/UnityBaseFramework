// <auto-generated>
//   This file was generated by a tool; you should avoid making direct changes.
//   Consider using 'partial classes' to extend these types
//   Input: GameProto.proto
// </auto-generated>

#region Designer generated code
#pragma warning disable CS0612, CS0618, CS1591, CS3021, IDE0079, IDE1006, RCS1036, RCS1057, RCS1085, RCS1192
namespace GameProto
{

    [global::ProtoBuf.ProtoContract()]
    public partial class User : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public long UserId { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        [global::System.ComponentModel.DefaultValue("")]
        public string UserName { get; set; } = "";

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class UserReadyInfo : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int LocalId { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public int Status { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public User User { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class Input : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int InputH { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public int InputV { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public int SkillId { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class InputFrame : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int Tick { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public int LocalId { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public bool IsMiss { get; set; }

        [global::ProtoBuf.ProtoMember(4)]
        public Input Input { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class ServerFrame : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int Tick { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public global::System.Collections.Generic.List<InputFrame> InputFrames { get; } = new global::System.Collections.Generic.List<InputFrame>();

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class CSHeartBeat : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class SCHeartBeat : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class CSPing : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int LocalId { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public long SendTimestamp { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class SCPing : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int LocalId { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public long SendTimestamp { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public long TimeSinceServerStart { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class CSLogin : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        [global::System.ComponentModel.DefaultValue("")]
        public string Account { get; set; } = "";

        [global::ProtoBuf.ProtoMember(2)]
        [global::System.ComponentModel.DefaultValue("")]
        public string Password { get; set; } = "";

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class SCLogin : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int RetCode { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class CSJoinRoom : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int RoomId { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class SCJoinRoom : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int RoomId { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public int LocalId { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public global::System.Collections.Generic.List<UserReadyInfo> UserReadyInfos { get; } = new global::System.Collections.Generic.List<UserReadyInfo>();

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class CSReady : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int Status { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class SCReady : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public global::System.Collections.Generic.List<UserReadyInfo> UserReadyInfos { get; } = new global::System.Collections.Generic.List<UserReadyInfo>();

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class SCGameStartInfo : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int RoomId { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public int MapId { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public int LocalId { get; set; }

        [global::ProtoBuf.ProtoMember(4)]
        public int UserCount { get; set; }

        [global::ProtoBuf.ProtoMember(5)]
        public int Seed { get; set; }

        [global::ProtoBuf.ProtoMember(6)]
        public global::System.Collections.Generic.List<User> Users { get; } = new global::System.Collections.Generic.List<User>();

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class CSLoadingProgress : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int Progress { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class SCLoadingProgress : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int AllProgress { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class CSInputFrame : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public InputFrame InputFrame { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class SCServerFrame : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int StartTick { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public global::System.Collections.Generic.List<ServerFrame> ServerFrames { get; } = new global::System.Collections.Generic.List<ServerFrame>();

    }

}

#pragma warning restore CS0612, CS0618, CS1591, CS3021, IDE0079, IDE1006, RCS1036, RCS1057, RCS1085, RCS1192
#endregion
