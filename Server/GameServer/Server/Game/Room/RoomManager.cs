using BaseFramework;
using BaseFramework.Runtime;

namespace Server
{
    public class RoomManager
    {
        private Dictionary<int, Room> m_Rooms = new Dictionary<int, Room>();
        private List<Room> m_WaitToRemoveRooms = new List<Room>();


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

                if (kvp.Value != null && kvp.Value.IsEmpty())
                {
                    m_WaitToRemoveRooms.Add(kvp.Value);
                }
            }

            foreach (Room tRoom in m_WaitToRemoveRooms)
            {
                if (tRoom != null)
                {
                    tRoom.StopGame();
                    RemoveRoom(tRoom);
                }
            }

            if (m_WaitToRemoveRooms.Count > 0)
            {
                m_WaitToRemoveRooms.Clear();
            }
        }

        public void Destroy()
        {
            foreach (KeyValuePair<int, Room> kvp in m_Rooms)
            {
                m_WaitToRemoveRooms.Add(kvp.Value);
            }

            foreach (Room tRoom in m_WaitToRemoveRooms)
            {
                if (tRoom != null)
                {
                    tRoom.StopGame();
                    RemoveRoom(tRoom);
                }
            }

            m_WaitToRemoveRooms.Clear();
            m_Rooms.Clear();
        }

        public Room CreateNewRoom(User creater)
        {
            Room room = ReferencePool.Acquire<Room>();
            
            //Room初始化。
            room.Init(CommonDefinitions.MaxRoomMemberCount);
            room.RoomId = RoomIdGenerator.GenerateId();
            room.RoomName = creater.Account;

            m_Rooms.Add(room.RoomId, room);
            Log.Info($"CreateNewRoom:{room.RoomId}");
            return room;
        }

        private void RemoveRoom(Room room)
        {
            if (room == null)
            {
                return;
            }
            if (m_Rooms.ContainsKey(room.RoomId))
            {
                m_Rooms.Remove(room.RoomId);
            }

            Log.Info($"RemoveRoom:{room.RoomId}");
            ReferencePool.Release(room);
        }

        public Room GetAvailableRoom()
        {
            // TODO:匹配规则

            foreach (KeyValuePair<int, Room> kvp in m_Rooms)
            {
                if (!kvp.Value.IsFull())
                {
                    return kvp.Value;
                }
            }
            return null;
        }
    }
}
