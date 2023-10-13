using Lockstep.Game;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace XGame
{
    public class DumpHelper
    {
        public int Tick => _world.Tick;
        protected World _world;

        private HashHelper _hashHelper;
        private StringBuilder _curSb;
        public bool enable = false;

        public Dictionary<int, StringBuilder> _tick2RawFrameData = new Dictionary<int, StringBuilder>();
        public Dictionary<int, StringBuilder> _tick2OverrideFrameData = new Dictionary<int, StringBuilder>();

#if UNITY_EDITOR
        private string dumpPath => "d:\\Log\\DumpLog"; //Path.Combine(UnityEngine.Application.dataPath, GameEntry.Service.GetService<ConstStateService>().DumpStrPath);
#endif
#if UNITY_STANDALONE_WIN
        private string dumpAllPath => "d:\\Log\\DumpLog";
#else
        private string dumpAllPath => "/tmp/Tutorial/LockstepTutorial/DumpLog";
#endif  

        public DumpHelper(World world, HashHelper hashHelper)
        {
            _world = world;
            _hashHelper = hashHelper;
        }

        public void DumpFrame(bool isNewFrame)
        {
            if (!enable) return;
            _curSb = DumpFrame();
            if (isNewFrame)
            {
                _tick2RawFrameData[Tick] = _curSb;
                _tick2OverrideFrameData[Tick] = _curSb;
            }
            else
            {
                _tick2OverrideFrameData[Tick] = _curSb;
            }
        }

        public void DumpToFile(bool withCurFrame = false)
        {
            if (!enable) return;
#if UNITY_EDITOR
            var path = dumpPath + "/cur.txt";
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            //var minTick = _tick2OverrideFrameData.Keys.Min();
            StringBuilder sbResume = new StringBuilder();
            StringBuilder sbRaw = new StringBuilder();
            for (int i = 0; i <= Tick; i++)
            {
                sbRaw.AppendLine(_tick2RawFrameData[i].ToString());
                sbResume.AppendLine(_tick2OverrideFrameData[i].ToString());
            }

            File.WriteAllText(dumpPath + "/resume.txt", sbResume.ToString());
            File.WriteAllText(dumpPath + "/raw.txt", sbRaw.ToString());
            if (withCurFrame)
            {
                _curSb = DumpFrame();
                var curHash = _hashHelper.CalcHash(true);
                File.WriteAllText(dumpPath + "/cur_single.txt", _curSb.ToString());
                File.WriteAllText(dumpPath + "/raw_single.txt", _tick2RawFrameData[Tick].ToString());
            }

            UnityEngine.Debug.Break();
#endif 
        }


        public void OnFrameEnd()
        {
            _curSb = null;
        }

        //public void Trace(string msg, bool isNewLine = false, bool isNeedLogTrace = false)
        //{
        //    if (_curSb == null) return;
        //    if (isNewLine)
        //    {
        //        _curSb.AppendLine(msg);
        //    }
        //    else
        //    {
        //        _curSb.Append(msg);
        //    }

        //    if (isNeedLogTrace)
        //    {
        //        StackTrace st = new StackTrace(true);
        //        StackFrame[] sf = st.GetFrames();
        //        for (int i = 2; i < sf.Length; ++i)
        //        {
        //            var frame = sf[i];
        //            _curSb.AppendLine(frame.GetMethod().DeclaringType.FullName + "::" + frame.GetMethod().Name);
        //        }
        //    }
        //}

        public void DumpAll()
        {
            if (!enable) return;
            var path = dumpAllPath + "/cur.txt";
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            StringBuilder sbRaw = new StringBuilder();
            for (int i = 0; i <= Tick; i++)
            {
                if (_tick2RawFrameData.TryGetValue(i, out var data))
                {
                    sbRaw.AppendLine(data.ToString());
                }
            }
            File.WriteAllText(dumpAllPath + $"/All_{GameEntry.Service.GetService<ConstStateService>().LocalActorId}.txt", sbRaw.ToString());
        }

        private StringBuilder DumpFrame()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Tick : " + Tick + "--------------------");
            _DumpStr(sb, "");
            return sb;
        }

        private void _DumpStr(System.Text.StringBuilder sb, string prefix)
        {
            foreach (var svc in GameEntry.Service.GetAllServices())
            {
                if (svc is IDumpStr hashSvc)
                {
                    sb.AppendLine(svc.GetType() + " --------------------");
                    hashSvc.DumpStr(sb, "\t" + prefix);
                }
            }
        }
    }
}