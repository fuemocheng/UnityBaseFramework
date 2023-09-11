using System.Reflection;
using ProtoBuf;
using ProtoBuf.Meta;
using BaseFramework;
using BaseFramework.Event;
using BaseFramework.Runtime;
using Network;
using GameProto;

namespace Network
{
    public class NetworkChannelHelper : INetworkChannelHelper
    {
        private readonly Dictionary<int, Type> m_PacketTypes = new();
        private readonly Dictionary<int, IPacketHandler> m_PacketHandlerTypes = new();
        private readonly MemoryStream m_HeaderCachedStream = new MemoryStream(4 + 5 + 5);
        private readonly MemoryStream m_PacketCachedStream = new MemoryStream(1024 * 8);

        /// <summary>
        /// 获取消息包头长度。
        /// </summary>
        public int PacketHeaderLength
        {
            get
            {
                // 4(预留长度) + 5(Fixed32 + 1) + 5(Fixed32 + 1)
                return 14;
            }
        }

        /// <summary>
        /// 初始化网络频道辅助器。
        /// </summary>
        public void Initialize()
        {
            // 反射注册包和包处理函数。
            Type packetBaseType = typeof(CSPacketBase);
            Type packetHandlerBaseType = typeof(PacketHandlerBase);
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                if (!types[i].IsClass || types[i].IsAbstract)
                {
                    continue;
                }

                if (types[i].BaseType == packetBaseType)
                {
                    PacketBase packetBase = (PacketBase)Activator.CreateInstance(types[i]);
                    Type packetType = GetPacketType(packetBase.Id);
                    if (packetType != null)
                    {
                        Log.Warning("Already exist packet type '{0}', check '{1}' or '{2}'?.", packetBase.Id.ToString(), packetType.Name, packetBase.GetType().Name);
                        continue;
                    }

                    m_PacketTypes.Add(packetBase.Id, types[i]);
                }
                else if (types[i].BaseType == packetHandlerBaseType)
                {
                    IPacketHandler packetHandler = (IPacketHandler)Activator.CreateInstance(types[i]);
                    IPacketHandler handler = GetPacketHandler(packetHandler.Id);
                    if (handler != null)
                    {
                        Log.Warning("Already exist PacketHandler '{0}', check '{1}'?.", handler.Id.ToString(), handler.GetType().Name);
                        continue;
                    }

                    m_PacketHandlerTypes.Add(packetHandler.Id, packetHandler);
                }
            }

            //GameEntry.Event.Subscribe(UnityBaseFramework.Runtime.NetworkConnectedEventArgs.EventId, OnNetworkConnected);
            //GameEntry.Event.Subscribe(UnityBaseFramework.Runtime.NetworkClosedEventArgs.EventId, OnNetworkClosed);
            //GameEntry.Event.Subscribe(UnityBaseFramework.Runtime.NetworkMissHeartBeatEventArgs.EventId, OnNetworkMissHeartBeat);
            //GameEntry.Event.Subscribe(UnityBaseFramework.Runtime.NetworkErrorEventArgs.EventId, OnNetworkError);
            //GameEntry.Event.Subscribe(UnityBaseFramework.Runtime.NetworkCustomErrorEventArgs.EventId, OnNetworkCustomError);
        }

        /// <summary>
        /// 关闭并清理网络频道辅助器。
        /// </summary>
        public void Shutdown()
        {
            //GameEntry.Event.Unsubscribe(UnityBaseFramework.Runtime.NetworkConnectedEventArgs.EventId, OnNetworkConnected);
            //GameEntry.Event.Unsubscribe(UnityBaseFramework.Runtime.NetworkClosedEventArgs.EventId, OnNetworkClosed);
            //GameEntry.Event.Unsubscribe(UnityBaseFramework.Runtime.NetworkMissHeartBeatEventArgs.EventId, OnNetworkMissHeartBeat);
            //GameEntry.Event.Unsubscribe(UnityBaseFramework.Runtime.NetworkErrorEventArgs.EventId, OnNetworkError);
            //GameEntry.Event.Unsubscribe(UnityBaseFramework.Runtime.NetworkCustomErrorEventArgs.EventId, OnNetworkCustomError);
        }

        /// <summary>
        /// 序列化消息包。
        /// </summary>
        /// <typeparam name="T">消息包类型。</typeparam>
        /// <param name="packet">要序列化的消息包。</param>
        /// <param name="destination">要序列化的目标流。</param>
        /// <returns>是否序列化成功。</returns>
        public bool Serialize<T>(T packet, Stream destination) where T : Packet
        {
            PacketBase packetImpl = packet as PacketBase;
            if (packetImpl == null)
            {
                Log.Warning("Packet is invalid.");
                return false;
            }

            if (packetImpl.PacketType != PacketType.ClientToServer)
            {
                Log.Warning("Send packet invalid.");
                return false;
            }

            m_HeaderCachedStream.SetLength(m_HeaderCachedStream.Capacity); // 此行防止 Array.Copy 的数据无法写入
            m_HeaderCachedStream.Position = 0L;

            m_PacketCachedStream.SetLength(m_PacketCachedStream.Capacity); // 此行防止 Array.Copy 的数据无法写入
            m_PacketCachedStream.Position = 0L;

            // 先将消息序列化到缓存。
            RuntimeTypeModel.Default.SerializeWithLengthPrefix(m_PacketCachedStream, packet, packet.GetType(), PrefixStyle.Fixed32, 0);

            // 再将序列化后的长度保存在Header中，然后将Header序列化到缓存。
            CSPacketHeader packetHeader = ReferencePool.Acquire<CSPacketHeader>();
            packetHeader.Id = packetImpl.Id;
            packetHeader.PacketLength = (int)m_PacketCachedStream.Length;
            RuntimeTypeModel.Default.SerializeWithLengthPrefix(m_HeaderCachedStream, packetHeader, packetHeader.GetType(), PrefixStyle.Fixed32, 0);

            // 释放 Header 和 Packet。
            ReferencePool.Release(packetHeader);
            ReferencePool.Release((IReference)packet);

            // 先将Header缓存写入目标流，再消息缓存写入目标流。
            m_HeaderCachedStream.WriteTo(destination);
            m_PacketCachedStream.WriteTo(destination);
            return true;
        }

        /// <summary>
        /// 反序列化消息包头。
        /// </summary>
        /// <param name="source">要反序列化的来源流。</param>
        /// <param name="customErrorData">用户自定义错误数据。</param>
        /// <returns>反序列化后的消息包头。</returns>
        public IPacketHeader DeserializePacketHeader(Stream source)
        {
            // 注意：此函数并不在主线程调用！
            return (IPacketHeader)RuntimeTypeModel.Default.DeserializeWithLengthPrefix(source, (object)ReferencePool.Acquire<CSPacketHeader>(), typeof(CSPacketHeader), PrefixStyle.Fixed32, 0);
        }

        /// <summary>
        /// 反序列化消息包。
        /// </summary>
        /// <param name="packetHeader">消息包头。</param>
        /// <param name="source">要反序列化的来源流。</param>
        /// <param name="customErrorData">用户自定义错误数据。</param>
        /// <returns>反序列化后的消息包。</returns>
        public Packet DeserializePacket(IPacketHeader packetHeader, Stream source)
        {
            // 注意：此函数并不在主线程调用！
            CSPacketHeader csPacketHeader = packetHeader as CSPacketHeader;
            if (csPacketHeader == null)
            {
                Log.Warning("Packet header is invalid.");
                return null;
            }

            Packet packet = null;
            if (csPacketHeader.IsValid)
            {
                Type packetType = GetPacketType(csPacketHeader.Id);
                if (packetType != null)
                {
                    packet = (Packet)RuntimeTypeModel.Default.DeserializeWithLengthPrefix(source, ReferencePool.Acquire(packetType), packetType, PrefixStyle.Fixed32, 0);
                }
                else
                {
                    Log.Warning("Can not deserialize packet for packet id '{0}'.", csPacketHeader.Id.ToString());
                }
            }
            else
            {
                Log.Warning("Packet header is invalid.");
            }

            ReferencePool.Release(csPacketHeader);
            return packet;
        }

        private Type GetPacketType(int id)
        {
            Type type = null;
            if (m_PacketTypes.TryGetValue(id, out type))
            {
                return type;
            }

            return null;
        }

        public IPacketHandler GetPacketHandler(int id)
        {
            IPacketHandler handler = null;
            if (m_PacketHandlerTypes.TryGetValue(id, out handler))
            {
                return handler;
            }

            return null;
        }

        private void OnNetworkConnected(object sender, GameEventArgs e)
        {
            //UnityBaseFramework.Runtime.NetworkConnectedEventArgs ne = (UnityBaseFramework.Runtime.NetworkConnectedEventArgs)e;
            //if (ne.NetworkChannel != m_NetworkChannel)
            //{
            //    return;
            //}

            //Log.Info("Network channel '{0}' connected, local address '{1}', remote address '{2}'.", ne.NetworkChannel.Name, ne.NetworkChannel.Socket.LocalEndPoint.ToString(), ne.NetworkChannel.Socket.RemoteEndPoint.ToString());
        }

        private void OnNetworkClosed(object sender, GameEventArgs e)
        {
            //UnityBaseFramework.Runtime.NetworkClosedEventArgs ne = (UnityBaseFramework.Runtime.NetworkClosedEventArgs)e;
            //if (ne.NetworkChannel != m_NetworkChannel)
            //{
            //    return;
            //}

            //Log.Info("Network channel '{0}' closed.", ne.NetworkChannel.Name);
        }

        private void OnNetworkMissHeartBeat(object sender, GameEventArgs e)
        {
            //UnityBaseFramework.Runtime.NetworkMissHeartBeatEventArgs ne = (UnityBaseFramework.Runtime.NetworkMissHeartBeatEventArgs)e;
            //if (ne.NetworkChannel != m_NetworkChannel)
            //{
            //    return;
            //}

            //Log.Info("Network channel '{0}' miss heart beat '{1}' times.", ne.NetworkChannel.Name, ne.MissCount.ToString());

            //if (ne.MissCount < 2)
            //{
            //    return;
            //}

            //ne.NetworkChannel.Close();
        }

        private void OnNetworkError(object sender, GameEventArgs e)
        {
            //UnityBaseFramework.Runtime.NetworkErrorEventArgs ne = (UnityBaseFramework.Runtime.NetworkErrorEventArgs)e;
            //if (ne.NetworkChannel != m_NetworkChannel)
            //{
            //    return;
            //}

            //Log.Info("Network channel '{0}' error, error code is '{1}', error message is '{2}'.", ne.NetworkChannel.Name, ne.ErrorCode.ToString(), ne.ErrorMessage);

            //ne.NetworkChannel.Close();
        }

        private void OnNetworkCustomError(object sender, GameEventArgs e)
        {
            //UnityBaseFramework.Runtime.NetworkCustomErrorEventArgs ne = (UnityBaseFramework.Runtime.NetworkCustomErrorEventArgs)e;
            //if (ne.NetworkChannel != m_NetworkChannel)
            //{
            //    return;
            //}
        }
    }
}
