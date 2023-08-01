using System.IO;
using UnityEditor;
using BaseFramework;

namespace XGame.Editor.Tools
{
    public sealed class ProtoTools
    {
        [MenuItem("Tools/Gen Proto")]
        private static void ExportProto()
        {
            // 设置批处理文件工作目录
            string workingPath = Utility.Text.Format("{0}{1}", Directory.GetCurrentDirectory(), "/../Common/Proto");

            // 批处理文件路径
            string batPath = Utility.Text.Format("{0}{1}", Directory.GetCurrentDirectory(), "/../Common/Proto/proto2cs.bat");

#if UNITY_EDITOR_WIN
            workingPath = workingPath.Replace("/", "\\");
            batPath = batPath.Replace("/", "\\");
#elif UNITY_EDITOR_OSX
            workingPath = workingPath.Replace("\\", "/");    
            batPath = batPath.Replace("\\", "/");    
#endif
            //执行bat文件
            EditorUtility.ExecuteBat(batPath, "", workingPath);

            AssetDatabase.Refresh();
        }
    }
}
