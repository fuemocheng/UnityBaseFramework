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
    public partial class CSLoginHandler : PacketHandlerBase
    {
        public override void Handle(object sender, Packet packet)
        {
            CSLogin packetImpl = (CSLogin)packet;
            Log.Info("Receive Packet Type:'{0}', Id:{1}", packetImpl.GetType().ToString(), packetImpl.Id.ToString());

            // Tcp Session。
            Session session = (Session)sender;
            bool isPasswordCorrect = true;

            // TODO:Get From DB

            // Get User。
            Server.User user = GameEntry.GameLogic.UserManager.GetUser(packetImpl.Account);
            if (user == null)
            {
                // Create user。
                user = ReferencePool.Acquire<Server.User>();
                user.UserId = UserIdGenerator.GenerateId();
                user.Account = packetImpl.Account;
                user.Password = packetImpl.Password;
                user.UserName = packetImpl.Account;
                user.TcpSession = session;

                session.BindInfo = user;

                GameEntry.GameLogic.UserManager.AddUser(user);
            }
            else
            {
                // Reset Session。
                user.TcpSession = session;
                if (packetImpl.Password != user.Password)
                {
                    isPasswordCorrect = false;
                }
            }

            // 回客户端消息。
            SCLogin scLogin = ReferencePool.Acquire<SCLogin>();
            // TODO:暂定 1 为登录成功, 2 密码不对。
            if(isPasswordCorrect)
            {
                scLogin.RetCode = 1;
            }
            else
            {
                scLogin.RetCode = 2;
            }
            session.Send(scLogin);
        }
    }
}
