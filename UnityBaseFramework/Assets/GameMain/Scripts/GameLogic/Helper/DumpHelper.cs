using Lockstep.Game;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityBaseFramework.Runtime;

namespace XGame
{
    public class DumpHelper
    {
        public bool Enable = false;

        private int Tick => m_World.Tick;
        private World m_World;
        private HashHelper m_HashHelper;

        private StringBuilder m_CurSb;

        private Dictionary<int, StringBuilder> m_Tick2RawFrameData = new Dictionary<int, StringBuilder>();
        private Dictionary<int, StringBuilder> m_Tick2OverrideFrameData = new Dictionary<int, StringBuilder>();

        private string m_DumpPath => Path.Combine(UnityEngine.Application.dataPath, CommonDefinitions.DumpPath);

        private string m_DumpAllPath => Path.Combine(UnityEngine.Application.dataPath, CommonDefinitions.DumpPath);

        public DumpHelper(World world, HashHelper hashHelper)
        {
            m_World = world;
            m_HashHelper = hashHelper;
        }

        public void Clear()
        {
            m_Tick2RawFrameData.Clear();
            m_Tick2OverrideFrameData.Clear();
            m_CurSb = null;
        }

        public void DumpFrame(bool isNewFrame)
        {
            if (!Enable)
            {
                return;
            }

            m_CurSb = new StringBuilder();
            DumpCurrFrame(m_CurSb);

            //把每一帧的StringBuilder都缓存下来。
            if (isNewFrame)
            {
                m_Tick2RawFrameData[Tick] = m_CurSb;
                m_Tick2OverrideFrameData[Tick] = m_CurSb;
            }
            else
            {
                m_Tick2OverrideFrameData[Tick] = m_CurSb;
            }
        }

        private void DumpCurrFrame(StringBuilder sb, string prefix = "")
        {
            sb.AppendLine($"Tick: {Tick} --------------------");
            foreach (var svc in GameEntry.Service.GetAllServices())
            {
                if (svc is IDumpStr dump)
                {
                    sb.AppendLine($"{svc.GetType()} --------------------");
                    dump.DumpStr(sb, "\t" + prefix);
                }
            }
        }

        public void OnFrameEnd()
        {
            m_CurSb = null;
        }

        public void DumpToFile(bool withCurFrame = false)
        {
            if (!Enable)
            {
                return;
            }

#if UNITY_EDITOR
            string path = m_DumpPath + "/resume.txt";
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            StringBuilder sbResume = new StringBuilder();
            StringBuilder sbRaw = new StringBuilder();
            for (int i = 0; i <= Tick; i++)
            {
                sbRaw.AppendLine(m_Tick2RawFrameData[i].ToString());
                sbResume.AppendLine(m_Tick2OverrideFrameData[i].ToString());
            }

            File.WriteAllText(m_DumpPath + "/raw.txt", sbRaw.ToString());
            File.WriteAllText(m_DumpPath + "/resume.txt", sbResume.ToString());
            if (withCurFrame)
            {
                m_CurSb = new StringBuilder();
                DumpCurrFrame(m_CurSb);
                int curHash = m_HashHelper.CalculateHash(true);
                File.WriteAllText(m_DumpPath + "/raw_single.txt", m_Tick2RawFrameData[Tick].ToString());
                File.WriteAllText(m_DumpPath + "/cur_single.txt", m_CurSb.ToString());
            }

            UnityEngine.Debug.Break();
#endif 
        }

        public void DumpAll()
        {
            if (!Enable)
            {
                return;
            }
            
            string dumpFilePath = m_DumpAllPath + $"/All_{GameEntry.Service.GetService<ConstStateService>().LocalActorId}.txt";
            string dir = Path.GetDirectoryName(dumpFilePath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            StringBuilder sbRaw = new StringBuilder();
            for (int i = 0; i <= Tick; i++)
            {
                if (m_Tick2RawFrameData.TryGetValue(i, out var data))
                {
                    sbRaw.AppendLine(data.ToString());
                }
            }

            File.WriteAllText(dumpFilePath, sbRaw.ToString());

            Log.Info($"DumpAll {dumpFilePath}");
        }
    }
}
