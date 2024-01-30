using System.IO;
using UnityEditor;
using UnityEngine;

namespace FixMath
{
    public class GenerateLut
    {
        [MenuItem("Tools/FixedPointMath/GenerateAllLut")]
        public static void GenerateAllLut()
        {
            GenerateSinLut();
            GenerateTanLut();
            GenerateAcosLut();

            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/FixedPointMath/GenerateSinLut")]
        public static void GenerateSinLut()
        {
            //写入当前工作目录。
            Fix64.GenerateSinLut();
            //删除旧文件。
            File.Delete(Application.dataPath + "/FixedPointPhysics/FixMath/Fix64SinLut.cs");
            //移动新文件。
            File.Move(Application.dataPath + "/../Fix64SinLut.cs", Application.dataPath + "/FixedPointPhysics/FixMath/Fix64SinLut.cs");

            //AssetDatabase.Refresh();
        }

        [MenuItem("Tools/FixedPointMath/GenerateTanLut")]
        public static void GenerateTanLut()
        {
            //写入当前工作目录。
            Fix64.GenerateTanLut();
            //删除旧文件。
            File.Delete(Application.dataPath + "/FixedPointPhysics/FixMath/Fix64TanLut.cs");
            //移动新文件。
            File.Move(Application.dataPath + "/../Fix64TanLut.cs", Application.dataPath + "/FixedPointPhysics/FixMath/Fix64TanLut.cs");

            //AssetDatabase.Refresh();
        }

        [MenuItem("Tools/FixedPointMath/GenerateAcosLut")]
        public static void GenerateAcosLut()
        {
            //写入当前工作目录。
            Fix64.GenerateAcosLut();
            //删除旧文件。
            File.Delete(Application.dataPath + "/FixedPointPhysics/FixMath/Fix64AcosLut.cs");
            //移动新文件。
            File.Move(Application.dataPath + "/../Fix64AcosLut.cs", Application.dataPath + "/FixedPointPhysics/FixMath/Fix64AcosLut.cs");

            //AssetDatabase.Refresh();
        }
    }
}
