using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MiscToolsForMD.SDK
{
    public class PublicDefines
    {
        public static readonly Dictionary<string, string> keyDisplayNames = new Dictionary<string, string>()
        {
            { "Backspace", "←" },
            { "Delete", "Del" },
            { "Tab", "Tab" },
            { "Return", "↲" },
            { "Escape", "Esc" },
            { "Keypad0", "0" },
            { "Keypad1", "1" },
            { "Keypad2", "2" },
            { "Keypad3", "3" },
            { "Keypad4", "4" },
            { "Keypad5", "5" },
            { "Keypad6", "6" },
            { "Keypad7", "7" },
            { "Keypad8", "8" },
            { "Keypad9", "9" },
            { "KeypadPeriod", "." },
            { "KeypadDivide", "/" },
            { "KeypadMultiply", "*" },
            { "KeypadMinus", "-" },
            { "KeypadPlus", "+" },
            { "KeypadEnter", "↲" },
            { "KeypadEquals", "=" },
            { "UpArrow", "↑" },
            { "DownArrow", "↓" },
            { "RightArrow", "→" },
            { "LeftArrow", "←" },
            { "Insert", "Ins" },
            { "Home", "Home" },
            { "End", "End" },
            { "PageUp", "PgUp" },
            { "PageDown", "PgDn" },
            { "Alpha0", "0" },
            { "Alpha1", "1" },
            { "Alpha2", "2" },
            { "Alpha3", "3" },
            { "Alpha4", "4" },
            { "Alpha5", "5" },
            { "Alpha6", "6" },
            { "Alpha7", "7" },
            { "Alpha8", "8" },
            { "Alpha9", "9" },
            { "Exclaim", "!" },
            { "DoubbleQuote", "\"" },
            { "Hash", "#" },
            { "Dollar", "$" },
            { "Percent", "%" },
            { "Ampersand", "&" },
            { "Quote", "'" },
            { "LeftParen", "(" },
            { "RightParen", ")" },
            { "Asterisk", "*" },
            { "Plus", "+" },
            { "Comma", "," },
            { "Minus", "-" },
            { "Period", "." },
            { "Slash", "/" },
            { "Colon", ":" },
            { "Semicolon", ";" },
            { "Less", "<" },
            { "Equals", "=" },
            { "Greater", ">" },
            { "Question", "?" },
            { "At", "@" },
            { "LeftBracket", "[" },
            { "RightBracket", "]" },
            { "Backslash", "\\" },
            { "Caret", "^" },
            { "Underscore", "_" },
            { "BackQuote", "`" },
            { "LeftCurlyBracket", "{" },
            { "RightCurlyBracket", "}" },
            { "Pipe", "|" },
            { "Tilde", "~" }
        };

        public static readonly string gamePath = Directory.GetCurrentDirectory();
        public static readonly string dataPath = Path.Combine(gamePath, "UserData");
        public static readonly string basePath = Path.Combine(dataPath, "MiscToolsForMD");
        public static readonly string statisticProviderId = "currentStatistics";
        public static readonly string attrCheckerId = "attrChecker";
        public static readonly string id = "MiscToolsForMD.SDK";
    }

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

    public class KeyConfigObj
    {
        public KeyListObj KeyList;
        public string IsChanged;
        public string KeyBoardProposal;
        public string HandleProposal;
        public string IsVibration;
        public string FeverKey;
    }

    public class KeyObj
    {
        public string Key;
        public string Type;
    }

    public class KeyListObj
    {
        public List<KeyObj> Custom;
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
            MelonMod firstCaller = InstancesManager.GetFirstMod();
            if (firstCaller != null)
            {
                firstCaller.LoggerInstance.Msg(fullMsg);
            }
            else
            {
                WriteLogHeader();
                Console.WriteLine(fullMsg);
            }
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

    internal class InternalDefines
    {
        public static readonly string configPath = Path.Combine(PublicDefines.basePath, "MiscToolsForMD.SDK.json");
    }
}
