using BaseFramework;

namespace Server
{
    public class Game
    {
        public UserManager UserManager = new UserManager();
        public RoomManager RoomManager = new RoomManager();

        public void Awake()
        {
            UserManager.Awake();
            RoomManager.Awake();
        }

        public void Start()
        {
            UserManager.Start();
            RoomManager.Start();
        }

        /// <summary>
        /// 每30ms更新一次。
        /// </summary>
        /// <param name="elapseSeconds"></param>
        /// <param name="realElapseSeconds"></param>
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            UserManager.Update(elapseSeconds, realElapseSeconds);
            RoomManager.Update(elapseSeconds, realElapseSeconds);
        }
    }
}
