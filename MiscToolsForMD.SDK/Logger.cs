using System;
using System.IO;

namespace MiscToolsForMD.SDK
{
    internal static class Logger
    {
        public static string logPath = InternalDefines.logPath;
        public static LogLevel logLevel = LogLevel.INFO;
        public static string dateTimeFmt = "HH:mm:ss.fff";
        public static string logStrFmt = "{0}-{1}-{2}";

        public static void Debug(string str)
        {
            if (logLevel >= LogLevel.DEBUG)
            {
                string fullStr = Format(LogLevel.DEBUG, str);
                Console.Write(fullStr);
                File.AppendAllText(logPath, fullStr);
            }
        }

        public static void Info(string str)
        {
            if (logLevel >= LogLevel.INFO)
            {
                string fullStr = Format(LogLevel.INFO, str);
                Console.Write(fullStr);
                File.AppendAllText(logPath, fullStr);
            }
        }

        public static void Warn(string str)
        {
            if (logLevel >= LogLevel.WARN)
            {
                string fullStr = Format(LogLevel.WARN, str);
                Console.Write(fullStr);
                File.AppendAllText(logPath, fullStr);
            }
        }

        public static void Error(string str)
        {
            if (logLevel >= LogLevel.ERROR)
            {
                string fullStr = Format(LogLevel.ERROR, str);
                Console.Write(fullStr);
                File.AppendAllText(logPath, fullStr);
            }
        }

        public static void Fatal(string str)
        {
            if (logLevel >= LogLevel.FATAL)
            {
                string fullStr = Format(LogLevel.FATAL, str);
                Console.Write(fullStr);
                File.AppendAllText(logPath, fullStr);
            }
        }

        private static string Format(LogLevel logLevel, string str)
        {
            return string.Format(logStrFmt, DateTime.Now.ToString(dateTimeFmt), logLevel, string.Format("[{0}] {1}\n", PublicDefines.id, str));
        }
    }

    internal enum LogLevel
    {
        FATAL,
        ERROR,
        WARN,
        INFO,
        DEBUG
    }
}
