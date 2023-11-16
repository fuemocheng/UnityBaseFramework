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
        private Dictionary<int, List<User>> m_Camps = new Dictionary<int, List<User>>();

        public Game Game;

        public Room()
        {
            Game = new Game(this);
        }

        public void Init(int memberCount)
        {
            //初始化LocalId列表。
            for (int i = 0; i < memberCount; i++)
            {
                m_LocalUsers.Add(null);
            }

            //初始化分组。
            for (int i = 1; i < (int)ECamp.EnumCount; i++)
            {
                if(m_Camps.ContainsKey(i))
                {
                    m_Camps[i].Clear();
                }
                else
                {
                    m_Camps.Add(i, new List<User>());
                }
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
            for (int i = 1; i < (int)ECamp.EnumCount; i++)
            {
                if (m_Camps.ContainsKey(i))
                {
                    m_Camps[i].Clear();
                }
            }
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

        /// <summary>
        /// 此阵营是否已经满员。
        /// </summary>
        /// <param name="camp"></param>
        /// <returns></returns>
        public bool IsCampFull(ECamp camp)
        {
            if (camp == ECamp.Default)
            {
                return false;
            }
            if (!m_Camps.ContainsKey((int)camp))
            {
                return false;
            }
            if (m_Camps[(int)camp].Count < CommonDefinitions.MaxRoomMemberCount / 2.0f)
            {
                return false;
            }
            return true;
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

            //执行到这里，说明此阵营没有满，或者随机分配阵营。
            if (user.Camp == ECamp.Default)
            {
                for (int i = 1; i < (int)ECamp.EnumCount; i++)
                {
                    //当此阵营未满，则加入此阵营。
                    if(!IsCampFull((ECamp)i))
                    {
                        m_Camps[i].Add(user);
                        user.Camp = (ECamp)i;
                        break;
                    }
                }
            }
            else
            {
                //当此阵营未满，则加入此阵营。
                if (!IsCampFull(user.Camp))
                {
                    m_Camps[(int)user.Camp].Add(user);
                }
                else
                {
                    //如果此阵营满了，则出错了。
                    Log.Error("User:{0} Join Camp Error, Camp:{1} is Full.", user.UserId, user.Camp);
                }
            }
            Log.Info("User:{0} LocalId:{1}  Join Room:{2}, Join Camp:{3}", user.UserId, user.LocalId, RoomId, user.Camp);
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
            //LocalId 删除；
            for (int i = 0; i < m_LocalUsers.Count; i++)
            {
                if (m_LocalUsers[i] == user)
                {
                    m_LocalUsers[i] = null;
                    user.LocalId = -1;
                    break;
                }
            }
            //阵营删除；
            if(user.Camp != ECamp.Default && m_Camps.ContainsKey((int)user.Camp))
            {
                m_Camps[(int)user.Camp].Remove(user);
            }
            user.Room = null;
            user.LocalId = -1;
            user.UserState = EUserState.LoggedIn;
            user.Camp = ECamp.Default;
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
