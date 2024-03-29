// <auto-generated>
//   This file will only be generated once
//   and will not be overwritten.
//   You need to implement the 'Handle' function yourself.
// </auto-generated>

using BaseFramework;
using BaseFramework.Runtime;
using Network;
using Server;

namespace GameProto
{
    public partial class CSReadyHandler : PacketHandlerBase
    {
        public override void Handle(object sender, Packet packet)
        {
            CSReady packetImpl = (CSReady)packet;
            //Log.Info("Receive Packet Type:'{0}'", packetImpl.GetType().ToString());

            // Tcp Session。
            Session session = (Session)sender;
            // User。
            Server.User user = (Server.User)session.BindInfo;

            if (packetImpl.UserState == (int)EUserState.LoggedIn)
            {
                Log.Info("CSReadyHandler Reconnected...");

                if (user.Room == null)
                {
                    Log.Error("CSReadyHandler Error : Room is null.");
                    return;
                }

                // 重连，只给自己重新同步信息。
                SCReady scReady = ReferencePool.Acquire<SCReady>();
                scReady.RoomId = user.Room.RoomId;
                scReady.LocalId = user.LocalId;
                // 遍历添加所有人信息，添加到 SCReady
                foreach (KeyValuePair<long, Server.User> kvp2 in user.Room.GetUsersDictionary())
                {
                    UserGameInfo userGameInfo = new UserGameInfo();
                    userGameInfo.LocalId = kvp2.Value.LocalId;
                    userGameInfo.UserState = (int)kvp2.Value.UserState;
                    userGameInfo.Camp = (int)kvp2.Value.Camp;
                    userGameInfo.User = new User();
                    userGameInfo.User.UserId = kvp2.Value.UserId;
                    userGameInfo.User.UserName = kvp2.Value.UserName;
                    scReady.UserGameInfos.Add(userGameInfo);
                }
                user.TcpSession?.Send(scReady);
            }
            else
            {
                // 设置为准备状态或者非准备状态。
                user.UserState = (EUserState)packetImpl.UserState;

                Room room = user.Room;
                if (room == null)
                {
                    Log.Error("CSReadyHandler Error : Room is null.");
                    return;
                }

                if (room.IsAllReady())
                {
                    // 全部准备，广播客户端开始游戏的消息。
                    foreach (KeyValuePair<long, Server.User> kvp in room.GetUsersDictionary())
                    {
                        Server.User sUser = kvp.Value;
                        if (sUser == null || sUser.TcpSession == null)
                        {
                            continue;
                        }
                        SCGameStartInfo gameStartInfo = ReferencePool.Acquire<SCGameStartInfo>();
                        gameStartInfo.RoomId = room.RoomId;
                        gameStartInfo.MapId = 1;
                        gameStartInfo.LocalId = sUser.LocalId;
                        gameStartInfo.UserCount = room.GetCurrCount();
                        gameStartInfo.Seed = 0;
                        // 遍历添加所有人信息，添加到 SCGameStartInfo
                        foreach (KeyValuePair<long, Server.User> kvp2 in room.GetUsersDictionary())
                        {
                            UserGameInfo userGameInfo = new UserGameInfo();
                            userGameInfo.LocalId = kvp2.Value.LocalId;
                            userGameInfo.UserState = (int)kvp2.Value.UserState;
                            userGameInfo.Camp = (int)kvp2.Value.Camp;
                            userGameInfo.User = new User();
                            userGameInfo.User.UserId = kvp2.Value.UserId;
                            userGameInfo.User.UserName = kvp2.Value.UserName;

                            gameStartInfo.UserGameInfos.Add(userGameInfo);
                        }
                        sUser.TcpSession?.Send(gameStartInfo);
                    }

                    room.Game.SetLoading();
                }
                else
                {
                    // 非全部准备，广播客户端有人准备或取消准备。
                    foreach (KeyValuePair<long, Server.User> kvp in room.GetUsersDictionary())
                    {
                        Server.User sUser = kvp.Value;
                        if (sUser == null || sUser.TcpSession == null)
                        {
                            continue;
                        }
                        SCReady scReady = ReferencePool.Acquire<SCReady>();
                        scReady.RoomId = sUser.Room.RoomId;
                        scReady.LocalId = sUser.LocalId;
                        // 遍历添加所有人信息，添加到 SCReady
                        foreach (KeyValuePair<long, Server.User> kvp2 in room.GetUsersDictionary())
                        {
                            UserGameInfo userGameInfo = new UserGameInfo();
                            userGameInfo.LocalId = kvp2.Value.LocalId;
                            userGameInfo.UserState = (int)kvp2.Value.UserState;
                            userGameInfo.Camp = (int)kvp2.Value.Camp;
                            userGameInfo.User = new User();
                            userGameInfo.User.UserId = kvp2.Value.UserId;
                            userGameInfo.User.UserName = kvp2.Value.UserName;
                            scReady.UserGameInfos.Add(userGameInfo);
                        }
                        sUser.TcpSession?.Send(scReady);
                    }
                }
            }
        }
    }
}
