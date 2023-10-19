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
        private List<User> m_LocalUsers = new List<User>();

        public Game Game;

        public Room()
        {
            Game = new Game(this);
        }

        public void Init(int memberCount)
        {
            for (int i = 0; i < memberCount; i++)
            {
                m_LocalUsers.Add(null);
            }
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
            m_LocalUsers.Clear();
            Game?.Clear();
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
                if (kvp.Value.UserState != EUserState.Ready)
                {
                    return false;
                }
            }
            return true;
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
            for (int i = 0; i < m_LocalUsers.Count; i++)
            {
                if (m_LocalUsers[i] == null)
                {
                    m_LocalUsers[i] = user;
                    user.LocalId = i;
                    break;
                }
            }
            user.UserState = EUserState.NotReady;

            Log.Info("User:{0} Join Room:{1}.", user.UserId, RoomId);
        }

        public void LeaveRoom(User user)
        {
            if (user == null)
            {
                return;
            }
            if (!m_Users.ContainsKey(user.UserId))
            {
                return;
            }
            for (int i = 0; i < m_LocalUsers.Count; i++)
            {
                if (m_LocalUsers[i] == user)
                {
                    m_LocalUsers[i] = null;
                    user.LocalId = -1;
                    break;
                }
            }
            user.Room = null;
            user.LocalId = -1;
            user.UserState = EUserState.LoggedIn;
            m_Users.Remove(user.UserId);
            Log.Info("User:{0} Leave Room:{1}.", user.UserId, RoomId);
        }


        public void StopGame()
        {
            Game?.StopGame();
        }


        public void PauseGame(User user)
        {
            //TODO:

        }

        public void ResumeGame(User user)
        {
            //TODO:

        }

        public void QuitGame(User user)
        {
            StopGame();

            //广播停止游戏。
            foreach (KeyValuePair<long, User> kvp in m_Users)
            {
                User sUser = kvp.Value;
                if (sUser == null || sUser.TcpSession == null)
                {
                    continue;
                }
                //状态设置登录状态。
                sUser.UserState = EUserState.LoggedIn;
                sUser.LoadingProgress = 0;

                SCGameControlCode scGameControlCode = ReferencePool.Acquire<SCGameControlCode>();
                scGameControlCode.GameControlCode = (int)EGameControlCode.Quit;
                sUser.TcpSession?.Send(scGameControlCode);
            }
        }
    }
}
