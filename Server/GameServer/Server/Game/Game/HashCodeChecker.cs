using BaseFramework;

namespace Server
{
    /// <summary>
    /// HashCode 校验器。
    /// </summary>
    public class HashCodeChecker : IReference
    {
        public int HashCode;
        public int CheckedCount;
        public bool[] ReceivedResult;

        public HashCodeChecker()
        {
            HashCode = 0;
            CheckedCount = 0;
            ReceivedResult = new bool[CommonDefinitions.MaxRoomMemberCount];
        }

        public bool IsMatched
        {
            get
            {
                return CheckedCount == ReceivedResult.Length;
            }
        }

        public void Clear()
        {
            HashCode = 0;
            CheckedCount = 0;
            if (ReceivedResult != null)
            {
                for (int i = 0; i < ReceivedResult.Length; i++)
                {
                    ReceivedResult[i] = false;
                }
            }
        }
    }
}
