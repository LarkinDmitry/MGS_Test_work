using System;
using System.IO;
using UnityEngine;

public static class Logger
{
    private static string _logPath = string.Empty;

    public static void SetLogPath(string logPath)
    {
        _logPath = Path.Combine(logPath, "log.txt");
    }

    public static void WriteLog(string log)
    {
        if(_logPath == string.Empty)
        {
            Debug.LogError("logPath not set!");
            return;
        }

#if UNITY_EDITOR
        Debug.Log(log);
#endif
        File.AppendAllText(_logPath, $"{DateTime.Now} - {log}\n");
    }
}