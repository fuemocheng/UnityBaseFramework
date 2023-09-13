using BaseFramework;

namespace Server
{
    public class Game
    {
        private Dictionary<long, User> m_Users = new Dictionary<long, User>();


        public void Awake()
        {

        }

        public void Start()
        {

        }

        /// <summary>
        /// 每30ms更新一次。
        /// </summary>
        /// <param name="elapseSeconds"></param>
        /// <param name="realElapseSeconds"></param>
        public void Update(float elapseSeconds, float realElapseSeconds)
        {

        }


        public void AddUser(User user)
        {
            if (!m_Users.ContainsKey(user.UserId))
            {
                m_Users.Add(user.UserId, user);
            }
        }

        public void RemoveUser(User user)
        {
            if (m_Users.ContainsKey(user.UserId))
            {
                m_Users.Remove(user.UserId);
            }
            ReferencePool.Release(user);
        }

        public User GetUser(long userId)
        {
            if (m_Users.ContainsKey(userId))
            {
                return m_Users[userId];
            }
            return null;
        }
    }
}
