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
        private List<User> m_LocalIds = new List<User>();

        public Game Game;

        public Room()
        {
            for (int i = 0; i < CommonDefinitions.MaxRoomMemberCount; i++)
            {
                m_LocalIds.Add(null);
            }

            Game = new Game(this);
        }

        public void Update(double elapseSeconds, double realElapseSeconds)
        {
            Game?.Update(elapseSeconds, realElapseSeconds);
        }

        public void Clear()
        {
            RoomId = 0;
            RoomName = string.Empty;
            m_Users.Clear();
            m_LocalIds.Clear();
            Game.Clear();
        }

        /// <summary>
        /// 房间是否满员。
        /// </summary>
        /// <returns></returns>
        public bool IsFull()
        {
            return m_Users.Count >= CommonDefinitions.MaxRoomMemberCount;
        }

        public bool IsAllReady()
        {
            if (!IsFull())
            {
                return false;
            }
            foreach (var kvp in m_Users)
            {
                if (!kvp.Value.IsReady)
                {
                    return false;
                }
            }
            return true;
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

        /// <summary>
        /// 获取玩家字典。
        /// </summary>
        /// <returns></returns>
        public Dictionary<long, User> GetUsersDictionary()
        {
            return m_Users;
        }

        public void JoinRoom(User user)
        {
            if (m_Users.ContainsKey(user.UserId))
            {
                return;
            }
            m_Users.Add(user.UserId, user);
            user.Room = this;
            //LocalId
            for (int i = 0; i < m_LocalIds.Count; i++)
            {
                if (m_LocalIds[i] == null)
                {
                    m_LocalIds[i] = user;
                    user.LocalId = i;
                    break;
                }
            }
            Log.Info("User:{0} join Room:{1}.", user.UserId, RoomId);
        }

        public void LeaveRoom(User user)
        {
            if (!m_Users.ContainsKey(user.UserId))
            {
                return;
            }
            for (int i = 0; i < m_LocalIds.Count; i++)
            {
                if (m_LocalIds[i] == user)
                {
                    m_LocalIds[i] = null;
                    user.LocalId = -1;
                    break;
                }
            }
            user.Room = null;
            m_Users.Remove(user.UserId);
            Log.Info("User:{0} leave Room:{1}.", user.UserId, RoomId);
        }
    }
}
