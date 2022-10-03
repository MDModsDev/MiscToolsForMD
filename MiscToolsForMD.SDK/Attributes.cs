using System;
using System.Reflection;

namespace MiscToolsForMD.SDK
{
    public static class AttributeChecker
    {
        public static void Check(object o)
        {
            foreach (MethodInfo methodInfo in o.GetType().GetMethods())
            {
                string methodFullName = string.Format("{0}.{1}", methodInfo.DeclaringType.FullName, methodInfo.Name);
                methodInfo.GetCustomAttribute<PrintSupportedAttribute>()?.PrintSelf(methodFullName);
            }
        }
    }

    [AttributeUsage(AttributeTargets.All, Inherited = false)]
    public class FixmeAttribute : PrintSupportedAttribute
    {
        private readonly string msg;

        public FixmeAttribute(string reason)

        {
            msg = reason;
        }

        public override void PrintSelf(string methodInfo)
        {
            string fullMsg = string.Format("{0}: Fixme: {1}", methodInfo, msg);
            WriteLogHeader();
            Console.WriteLine(fullMsg);
        }

        private static void WriteLogHeader()
        {
            string formattedTime = DateTime.Now.ToString("HH:mm:ss.fff");

            Console.Write("[");
            ConsoleColor consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(formattedTime);
            Console.ForegroundColor = consoleColor;
            Console.Write("] [");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(PublicDefines.id);
            Console.ForegroundColor = consoleColor;
            Console.Write("] ");
        }
    }

    public abstract class PrintSupportedAttribute : Attribute
    {
        public abstract void PrintSelf(string methodInfo);
    }
}
