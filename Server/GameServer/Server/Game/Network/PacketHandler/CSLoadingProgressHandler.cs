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
    public partial class CSLoadingProgressHandler : PacketHandlerBase
    {
        public override void Handle(object sender, Packet packet)
        {
            CSLoadingProgress packetImpl = (CSLoadingProgress)packet;
            //Log.Info("Receive Packet Type:'{0}'", packetImpl.GetType().ToString());

            // Tcp Session。
            Session session = (Session)sender;
            // User。
            Server.User user = (Server.User)session.BindInfo;
            user.UserState = EUserState.Loading;
            user.LoadingProgress = packetImpl.Progress;

            Room room = user.Room;

            int totalProgress = room.GetCurrCount() * 100;
            int currProgress = 0;
            // 计算目前加载进度。
            foreach (KeyValuePair<long, Server.User> kvp in room.GetUsersDictionary())
            {
                Server.User sUser = kvp.Value;
                if (sUser == null || sUser.TcpSession == null)
                {
                    continue;
                }
                currProgress += sUser.LoadingProgress;
            }

            currProgress = (int)(currProgress / totalProgress) * 100;

            // 广播进度。
            foreach (KeyValuePair<long, Server.User> kvp in room.GetUsersDictionary())
            {
                Server.User sUser = kvp.Value;
                if(sUser == null || sUser.TcpSession == null)
                {
                    continue;
                }    
                SCLoadingProgress scLoadingProgress = ReferencePool.Acquire<SCLoadingProgress>();
                scLoadingProgress.AllProgress = currProgress;
                sUser.TcpSession?.Send(scLoadingProgress);
            }

            //所有客户端加载完成。
            if (currProgress >= 100)
            {
                // 广播加载完成，开始游戏。
                room.Game.SetLoadingFinished();
                
                // 设置为Playing状态。
                foreach (KeyValuePair<long, Server.User> kvp in room.GetUsersDictionary())
                {
                    Server.User sUser = kvp.Value;
                    if (sUser != null)
                    {
                        sUser.UserState = EUserState.Playing;
                    }
                }
            }
        }
    }
}
