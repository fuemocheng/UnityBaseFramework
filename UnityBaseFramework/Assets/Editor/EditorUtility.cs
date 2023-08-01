using UnityEngine;
using System.Diagnostics;

public static class EditorUtility
{
    /// <summary>
    /// 执行Bat文件
    /// </summary>
    /// <param name="batFilePath"></param>
    public static void ExecuteBat(string batFilePath, string args = "", string workingPath = "")
    {
        UnityEngine.Debug.Log(string.Format($"ExecuteBat: {batFilePath}"));

        // 创建一个 ProcessStartInfo 对象来配置进程的启动信息
        ProcessStartInfo processInfo = new ProcessStartInfo();
        // 参数
        if (!string.IsNullOrEmpty(args))
        {
            processInfo.Arguments = args;
        }
        processInfo.FileName = batFilePath;            // 设置要执行的 BAT 文件路径
        processInfo.CreateNoWindow = true;             // 不创建新窗口显示 BAT 文件执行过程
        processInfo.UseShellExecute = false;           // 不使用操作系统的 shell 执行
        processInfo.RedirectStandardError = true;      // 重定向标准错误输出
        processInfo.RedirectStandardOutput = true;     // 重定向标准输出
        if (!string.IsNullOrEmpty(workingPath))
        {
            processInfo.WorkingDirectory = workingPath;
        }

        // 创建一个进程对象并启动
        Process process = new Process();
        process.StartInfo = processInfo;
        process.Start();

        // 等待进程执行完成
        process.WaitForExit();

        // 获取 BAT 文件的标准输出和标准错误输出信息
        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();

        // 关闭进程
        process.Close();

        // 输出 BAT 文件的执行结果
        if (!string.IsNullOrEmpty(output))
            UnityEngine.Debug.Log(string.Format($"ExecuteBat Output:\n{output}"));
        if (!string.IsNullOrEmpty(error))
            UnityEngine.Debug.Log(string.Format($"ExecuteBat Error:\n{error}"));
    }
}
