// ------------------------------------------------------------
// FILENAME:        DefaultScriptHeaderComments.cs
// AUTHOR:          xgc
// DESCRIPTION:     默认 C# 脚本头部注释
// ------------------------------------------------------------

/*
using System.IO;
using UnityEngine;

/// <summary>
/// 自定义默认脚本头部注释
/// </summary>
public class DefaultScriptHeaderComments : UnityEditor.AssetModificationProcessor
{
    // 脚本头部注释模板
    private static string m_commentContent =
          "// ------------------------------------------------------------\n"
        + "// FILENAME:        #FILENAME#\n"
        + "// AUTHOR:          #AUTHOR#\n"
        + "// DESCRIPTION:     \n"
        + "// ------------------------------------------------------------\n\n";

    public static void OnWillCreateAsset(string assetPath)
    {
        //去掉.meta
        string filePath = assetPath.Replace(".meta", "");
        //获取扩展名
        string fileExt = Path.GetExtension(filePath);

        //判断是否是CSharp脚本
        if (fileExt != ".cs") return;

        //脚本全路径
        string scriptFullPath = Application.dataPath.Replace("Assets", "") + filePath;

        //读取脚本内容
        string scriptContent = File.ReadAllText(scriptFullPath);

        //添加头部注释
        scriptContent = scriptContent.Insert(0, m_commentContent);

        //修改注释内容
        scriptContent = scriptContent.Replace("#FILENAME#", Path.GetFileName(scriptFullPath));
        scriptContent = scriptContent.Replace("#AUTHOR#", "xgc");
        //scriptContent = scriptContent.Replace("#DATE#", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        //写入文件
        File.WriteAllText(scriptFullPath, scriptContent);
    }
}
*/
