using BaseFramework;
using BaseFramework.Runtime;
using GameProto;

namespace Server
{
    public class Room : IReference
    {
        public int RoomId;
        public string RoomName;
        private Dictionary<long, User> m_Users = new Dictionary<long, User>();

        public Room()
        {
        }

        public void Clear()
        {
            RoomId = 0;
            RoomName = string.Empty;
            m_Users.Clear();
        }

        /// <summary>
        /// 房间是否满员。
        /// </summary>
        /// <returns></returns>
        public bool IsFull()
        {
            return m_Users.Count >= CommonDefinitions.MaxRoomMemberCount;
        }

        /// <summary>
        /// 是否是空房间。
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return m_Users.Count == 0;
        }

        /// <summary>
        /// 获取当前房间人数。
        /// </summary>
        /// <returns></returns>
        public int GetCurrCount()
        {
            return m_Users.Count;
        }

        public void JoinRoom(User user)
        {
            if (m_Users.ContainsKey(user.UserId))
            {
                return;
            }
            user.Room = this;
            m_Users.Add(user.UserId, user);
            Log.Info("User:{0} join Room:{1}.", user.UserId, RoomId);
        }

        public void LeaveRoom(User user)
        {
            if (!m_Users.ContainsKey(user.UserId))
            {
                return;
            }
            user.Room = null;
            m_Users.Remove(user.UserId);
            Log.Info("User:{0} leave Room:{1}.", user.UserId, RoomId);
        }

        public bool IsAllStarted()
        {
            if(m_Users.Count< CommonDefinitions.MaxRoomMemberCount)
            {
                return false;
            }

            foreach(KeyValuePair<long, User> kvp in m_Users)
            {
                if (!kvp.Value.IsStarted)
                {
                    return false;
                }
            }

            return true;
        }

        public void SendAllClientStartGame()
        {
            foreach (KeyValuePair<long, User> kvp in m_Users)
            {
                User tUser = kvp.Value;
                SCStart scStart = ReferencePool.Acquire<SCStart>();
                tUser.TcpSession.Send(scStart);
            }
        }
    }
}
