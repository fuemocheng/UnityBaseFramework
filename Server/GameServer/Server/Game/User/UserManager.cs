using BaseFramework;

namespace Server
{
    public class UserManager
    {
        private Dictionary<long, User> m_Users = new Dictionary<long, User>();

        public void Awake()
        {

        }

        public void Start()
        {

        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {

        }

        public void Destroy()
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

        public User GetUser(string account)
        {
            foreach (KeyValuePair<long, User> pair in m_Users)
            {
                if (pair.Value.Account == account)
                {
                    return pair.Value;
                }
            }
            return null;
        }
    }
}
