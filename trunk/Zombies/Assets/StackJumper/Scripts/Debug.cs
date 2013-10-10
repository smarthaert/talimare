///-----------------------------------------------------------------------------------------------
/// author Ivan Murashko iclickable@gmail.com
///-----------------------------------------------------------------------------------------------
#if UNITY_EDITOR
using UnityEngine;

using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

using UnityDebug = UnityEngine.Debug;
using Object = UnityEngine.Object;

public sealed class Debug
{
    public static void Check(bool condition)
    {
        Check(condition, "Wrong condition!");
    }

    public static void Check(bool condition, object message)
    {
        if (!condition)
        {
            LogError(string.Format("Assert! {0}", message));
        }
    }

    [STAThread]
    private static void AddLog(LogType type, string message)
    {
        StackTrace stackTrace = new StackTrace(true);
        List<StackFrame> stackFrames = stackTrace.GetFrames().ToList();
        while (stackFrames.Count != 0 && stackFrames.First().GetMethod().ReflectedType == typeof(Debug))
        {
            stackFrames.RemoveAt(0);
        }
        if (stackFrames.Count == 0) return;
        Container.Log msg = new Container.Log(type, message, stackFrames);
        Container.AddLog(msg);
    }

    [Serializable]
    public enum LogType
    {
        Error, Assert, Warning, Log
    }

    #region LogContainer
    public sealed class Container
    {
        [Serializable]
        public class Log
        {
            public Log(LogType type, string message, List<FrameInfo> stackFrames)
            {
                Type = type;
                Text = message;
                Frames = stackFrames;
            }

            public Log(LogType type, string message, List<StackFrame> stackFrames)
            {
                Type = type;
                Text = message;
                Frames = new List<FrameInfo>();
                foreach (StackFrame frame in stackFrames)
                {
                    if (frame.GetFileName() != null)
                        Frames.Add(new FrameInfo(frame));
                }
            }

            public override string ToString()
            {
                return string.Format("[{0}:{1}:{2}] {3}", DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond, Text);
            }

            public LogType Type { get; set; }
            public string Text { get; set; }
            public List<FrameInfo> Frames { get; set; }
        }

        [Serializable]
        public class FrameInfo
        {
            public FrameInfo(StackFrame frame)
            {
                FileName = frame.GetFileName();
                MethodName = frame.GetMethod().Name;
                LineNumber = frame.GetFileLineNumber();
            }

            public FrameInfo(string fileName, string methodName, int lineNumber)
            {
                FileName = fileName;
                MethodName = methodName;
                LineNumber = lineNumber;
            }

            public override string ToString()
            {
                return string.Format("{1}, Line: {2}; File {0}", FileName, MethodName, LineNumber);
            }

            public string FileName { get; set; }
            public string MethodName { get; set; }
            public int LineNumber { get; set; }
        }

        public static event Action<Log> OnLogAdded;
        public static event Action OnLogsCleared;

        public static Log GetLog(int index)
        {
            return Logs[index];
        }

        public static void AddLog(Log item)
        {
            Logs.Add(item);
            SaveLog(item);
            if (OnLogAdded != null) OnLogAdded(item);
        }

        public static int GetLength()
        {
            return Logs.Count;
        }

        public static void Clear()
        {
            UnityDebug.Log("Clear");
            m_logs = new List<Log>();
            Save();
            if (OnLogsCleared != null) OnLogsCleared();
        }

        private static List<Log> Logs
        {
            get
            {
                if (m_logs == null)
                {
                    Load();
                }
                return m_logs;
            }
        }

        private static void SaveLog(Log newLog)
        {
            FileStream fs = new FileStream(m_filePath, FileMode.Append);
            StreamWriter swriter = new StreamWriter(fs);
            try
            {
                swriter.WriteLine("[Log]");
                swriter.WriteLine(newLog.Text);
                swriter.WriteLine(newLog.Type.ToString());
                foreach (FrameInfo frameInfo in newLog.Frames)
                {
                    swriter.WriteLine(frameInfo.ToString());
                }
                swriter.WriteLine();
            }
            catch { }
            finally
            {
                swriter.Close();
            }
        }

        private static void Save()
        {
            if (File.Exists(m_filePath))
            {
                File.Delete(m_filePath);
            }
            foreach (Log log in m_logs)
            {
                SaveLog(log);
            }
        }

        private static void Load()
        {
            m_logs = new List<Log>();
            if (!File.Exists(m_filePath))
            {
                return;
            }
            FileStream fs = new FileStream(m_filePath, FileMode.Open);
            StreamReader sreader = new StreamReader(fs);
            string line = "";
            while (!sreader.EndOfStream)
            {
                line = sreader.ReadLine();
                if (line == "[Log]")
                {
                    string text = sreader.ReadLine();
                    LogType type = (LogType)Enum.Parse(typeof(LogType), sreader.ReadLine());
                    List<FrameInfo> infos = new List<FrameInfo>();
                    char[] splitters = { ' ', ',', ';' };
                    while ((line = sreader.ReadLine()) != "")
                    {
                        string[] parts = line.Split(splitters);
                        string fileName = parts.Last();
                        string methodName = parts[0];
                        int lineNumber = int.Parse(parts[3]);
                        infos.Add(new FrameInfo(fileName, methodName, lineNumber));
                    }
                    Log log = new Log(type, text, infos);
                    m_logs.Add(log);
                }
            }
        }

        private const string m_filePath = "StackJumper.log";

        private static Container m_container;
        private static List<Log> m_logs;
    }
    #endregion LogContainer

    #region Unity Debug implementation
    public static bool isDebugBuild
    {
        get
        {
            return UnityDebug.isDebugBuild;
        }
    }

    public static void Break()
    {
        UnityDebug.Break();
        AddLog(LogType.Error, "Break");
    }

    public static void DebugBreak()
    {
        UnityDebug.DebugBreak();
        AddLog(LogType.Error, "DebugBreak");
    }

    public static void DrawLine(Vector3 start, Vector3 end)
    {
        UnityDebug.DrawLine(start, end);
    }

    public static void DrawLine(Vector3 start, Vector3 end, Color color)
    {
        UnityDebug.DrawLine(start, end, color);
    }

    public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
    {
        UnityDebug.DrawLine(start, end, color, duration);
    }

    public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration, bool depthTest)
    {
        UnityDebug.DrawLine(start, end, color, duration, depthTest);
    }

    public static void DrawRay(Vector3 start, Vector3 dir)
    {
        UnityDebug.DrawRay(start, dir);
    }

    public static void DrawRay(Vector3 start, Vector3 dir, Color color)
    {
        UnityDebug.DrawRay(start, dir, color);
    }

    public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration)
    {
        UnityDebug.DrawRay(start, dir, color, duration);
    }

    public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration, bool depthTest)
    {
        UnityDebug.DrawRay(start, dir, color, duration, depthTest);
    }

    public static void Log(object message)
    {
        UnityDebug.Log(message);
        AddLog(LogType.Log, message.ToString());
    }

    public static void Log(object message, Object context)
    {
        UnityDebug.Log(message, context);
        AddLog(LogType.Log, message.ToString());
    }

    public static void LogError(object message)
    {
        UnityDebug.LogError(message);
        AddLog(LogType.Error, message.ToString());
    }

    public static void LogError(object message, Object context)
    {
        UnityDebug.LogError(message, context);
        AddLog(LogType.Error, message.ToString());
    }

    public static void LogWarning(object message)
    {
        UnityDebug.LogWarning(message);
        AddLog(LogType.Warning, message.ToString());
    }

    public static void LogWarning(object message, Object context)
    {
        UnityDebug.LogWarning(message, context);
        AddLog(LogType.Warning, message.ToString());
    }
    #endregion Unity Debug implementation
}
#endif