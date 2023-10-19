using GameProto;
using ProtoBuf;
using ProtoBuf.Meta;
using System;
using System.IO;
using UnityBaseFramework.Runtime;

namespace XGame
{
    public class RecordUtility
    {

        public static void WriteRecord(string path, SCGameStartInfo gameStartInfo, SCServerFrame serverFrame)
        {
            if (string.IsNullOrEmpty(path) || gameStartInfo == null || serverFrame == null)
            {
                Log.Error("RecordUtility write record error.");
                return;
            }
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            //游戏整体信息。
            MemoryStream msGameStartInfo = new MemoryStream();
            RuntimeTypeModel.Default.SerializeWithLengthPrefix(msGameStartInfo, gameStartInfo, gameStartInfo.GetType(), PrefixStyle.Fixed32, 0);
            //对 MemoryStream 长度进行编码, 占8个byte。
            byte[] gameStartInfoLengthBytes = BitConverter.GetBytes(msGameStartInfo.Length);

            //游戏帧数据。
            MemoryStream msServerFrame = new MemoryStream();
            RuntimeTypeModel.Default.SerializeWithLengthPrefix(msServerFrame, serverFrame, serverFrame.GetType(), PrefixStyle.Fixed32, 0);
            //对 MemoryStream 长度进行编码，占8个byte。
            byte[] serverFrameLengthBytes = BitConverter.GetBytes(msServerFrame.Length);

            //目标内存流。
            MemoryStream msTarget = new MemoryStream();
            //写入Packet长度，再写入字节流
            msTarget.Write(gameStartInfoLengthBytes);
            msGameStartInfo.WriteTo(msTarget);
            //写入Packet长度，再写入字节流
            msTarget.Write(serverFrameLengthBytes);
            msServerFrame.WriteTo(msTarget);

            //写入文件。
            File.WriteAllBytes(path, msTarget.GetBuffer());

            //释放 MemoryStream。
            msTarget.Dispose();
            msServerFrame.Dispose();
            msGameStartInfo.Dispose();

            Log.Info("Create Record " + path);
        }

        public static void ReadRecord(string path, ref SCGameStartInfo gameStartInfo, ref SCServerFrame serverFrame)
        {
            if (string.IsNullOrEmpty(path) || gameStartInfo == null || serverFrame == null)
            {
                Log.Error("RecordUtility read record error.");
                return;
            }

            //读取文件
            byte[] bytes = File.ReadAllBytes(path);
            MemoryStream msSource = new MemoryStream(bytes);
            msSource.Position = 0;

            byte[] gameStartInfoLengthBytes = new byte[8];
            msSource.Read(gameStartInfoLengthBytes, 0, 8);
            int gameStartInfoLength = BitConverter.ToInt32(gameStartInfoLengthBytes, 0);

            byte[] gameStartInfoBuffer = new byte[gameStartInfoLength];
            msSource.Read(gameStartInfoBuffer, 0, gameStartInfoLength);

            byte[] serverFrameLengthBytes = new byte[8];
            msSource.Read(serverFrameLengthBytes, 0, 8);
            int serverFrameLength = BitConverter.ToInt32(serverFrameLengthBytes, 0);

            byte[] serverFrameBuffer = new byte[serverFrameLength];
            msSource.Read(serverFrameBuffer, 0, serverFrameLength);

            MemoryStream msGameStartInfo = new MemoryStream(gameStartInfoBuffer);
            MemoryStream msServerFrame = new MemoryStream(serverFrameBuffer);

            gameStartInfo = (SCGameStartInfo)RuntimeTypeModel.Default.DeserializeWithLengthPrefix(msGameStartInfo, Activator.CreateInstance(gameStartInfo.GetType()), gameStartInfo.GetType(), PrefixStyle.Fixed32, 0);
            serverFrame = (SCServerFrame)RuntimeTypeModel.Default.DeserializeWithLengthPrefix(msServerFrame, Activator.CreateInstance(serverFrame.GetType()), serverFrame.GetType(), PrefixStyle.Fixed32, 0);

            Log.Info("ReadRecord " + path);
        }
    }
}
