using BaseFramework;

namespace Server
{
    public class RoomManager
    {
        private Dictionary<int, Room> m_Rooms = new Dictionary<int, Room>();

        public void Awake()
        {

        }

        public void Start()
        {

        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            foreach (KeyValuePair<int, Room> kvp in m_Rooms)
            {
                kvp.Value?.Update(elapseSeconds, realElapseSeconds);
            }
        }

        public void Destroy()
        {

        }

        public Room GetAvailableRoom()
        {
            // TODO:匹配规则

            foreach (KeyValuePair<int, Room> kvp in m_Rooms)
            {
                if(!kvp.Value.IsFull())
                {
                    return kvp.Value;
                }
            }
            return null;
        }

        public void AddRoom(Room room)
        {
            if (!m_Rooms.ContainsKey(room.RoomId))
            {
                m_Rooms.Add(room.RoomId, room);
            }
        }

        public void RemoveRoom(Room room)
        {
            if (m_Rooms.ContainsKey(room.RoomId))
            {
                m_Rooms.Remove(room.RoomId);
            }
        }
    }
}
