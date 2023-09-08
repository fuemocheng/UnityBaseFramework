namespace Network
{
    //internal enum ParserState
    //{
    //    PacketSize,
    //    PacketBody
    //}

    //public struct Packet
    //{
    //    public const int MinSize = 2;
    //    public const int FlagIndex = 0;
    //    public const int OpcodeIndex = 1;
    //    public const int Index = 3;

    //    /// <summary>
    //    /// 只读，不允许修改
    //    /// </summary>
    //    public byte[] Bytes { get; }
    //    public ushort Length { get; set; }

    //    public Packet(int length)
    //    {
    //        this.Length = 0;
    //        this.Bytes = new byte[length];
    //    }

    //    public Packet(byte[] bytes)
    //    {
    //        this.Bytes = bytes;
    //        this.Length = (ushort)bytes.Length;
    //    }

    //    public byte Flag()
    //    {
    //        return this.Bytes[0];
    //    }

    //    public ushort Opcode()
    //    {
    //        return BitConverter.ToUInt16(this.Bytes, OpcodeIndex);
    //    }
    //}

    //public class PacketParser
    //{
    //    private bool _isOK;
    //    private ushort _packetSize;
    //    private ParserState _state;
    //    private Packet _packet = new Packet(ushort.MaxValue);

    //    private readonly CircularBuffer _buffer;

    //    public PacketParser(CircularBuffer buffer)
    //    {
    //        _buffer = buffer;
    //        _isOK = false;
    //        _state = ParserState.PacketSize;
    //    }

    //    public Packet GetPacket()
    //    {
    //        _isOK = false;
    //        return _packet;
    //    }

    //    public bool Parse()
    //    {
    //        if (_isOK)
    //        {
    //            return true;
    //        }

    //        bool isFinished = false;
    //        while (!isFinished)
    //        {
    //            switch (_state)
    //            {
    //                case ParserState.PacketSize:
    //                    if (_buffer.Length < 2)
    //                    {
    //                        isFinished = true;
    //                    }
    //                    else
    //                    {
    //                        _buffer.Read(_packet.Bytes, 0, 2);
    //                        _packetSize = BytesHelper.ToUInt16(_packet.Bytes, 0);
    //                        if (_packetSize > 60000) //UInt16.MaxValue 最大64kb
    //                        {
    //                            throw new Exception($"packet too large, size: {_packetSize}");
    //                        }
    //                        _state = ParserState.PacketBody;
    //                    }
    //                    break;
    //                case ParserState.PacketBody:
    //                    if (_buffer.Length < _packetSize)
    //                    {
    //                        isFinished = true;
    //                    }
    //                    else
    //                    {
    //                        _buffer.Read(_packet.Bytes, 0, _packetSize);
    //                        _packet.Length = _packetSize;
    //                        _isOK = true;
    //                        _state = ParserState.PacketSize; ;
    //                        isFinished = true;
    //                    }
    //                    break;
    //            }
    //        }

    //        return _isOK;
    //    }
    //}
}
