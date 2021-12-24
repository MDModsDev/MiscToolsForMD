﻿using System;
using System.Diagnostics;
using System.IO;

namespace ModHelper
{
    public static class ModLogger
    {
        private static StreamWriter fs = File.CreateText("ModLogger.log");
        private static object locker = new object();

        private static bool isShowConsole = false;
        static ModLogger()
        {
            if (Environment.GetEnvironmentVariable("SHOW_CONSOLE_WINDOW") == "TRUE")
            {
                isShowConsole = true;
            }
        }
        public static void Debug(object obj)
        {
            var frame = new StackTrace().GetFrame(1);
            var className = frame.GetMethod().ReflectedType.Name;
            var methodName = frame.GetMethod().Name;
            AddLog(className, methodName, obj);
        }
        public static void AddLog(string className, string methodName,object obj)
        {
            var text = $"[{className}:{methodName}]: {obj}";

            lock (locker)
            {
                if(isShowConsole)
                    Console.Error.WriteLine(text);
                fs.WriteLine(text);
                fs.Flush();
            }
        }
    }
}
