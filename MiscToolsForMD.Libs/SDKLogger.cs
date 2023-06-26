namespace MiscToolsForMD.SDK
{
    public enum LogLevel
    {
        ERROR,
        WARN,
        INFO,
        DEBUG
    }

    public static class SDKLogger
    {
        public static LogLevel level = LogLevel.INFO;
        private static readonly string logPath = Path.Combine(PublicDefines.basePath, PublicDefines.id + ".log");
        private static readonly string logFileFormat = "[{0}] [{1}] [{2}] {3}" + Environment.NewLine;
        private const string logStreamFormat = "[{0}] {1}";
        private const string timeFormat = "HH:mm:ss.fff";

        public static void Debug(string message)
        {
            if (level >= LogLevel.DEBUG)
            {
                const string levelString = "DEBUG";
                string formattedTime = DateTime.Now.ToString(timeFormat);
                PrintHeader(formattedTime);
                Console.WriteLine(string.Format(logStreamFormat, levelString, message));

                File.AppendAllText(logPath, string.Format(logFileFormat, formattedTime, PublicDefines.id, levelString, message));
            }
        }

        public static void Info(string message)
        {
            if (level >= LogLevel.INFO)
            {
                const string levelString = "INFO";
                string formattedTime = DateTime.Now.ToString(timeFormat);
                PrintHeader(formattedTime);
                Console.WriteLine(string.Format(logStreamFormat, levelString, message));

                File.AppendAllText(logPath, string.Format(logFileFormat, formattedTime, PublicDefines.id, levelString, message));
            }
        }

        public static void Warn(string message)
        {
            if (level >= LogLevel.WARN)
            {
                const string levelString = "WARNING";
                string formattedTime = DateTime.Now.ToString(timeFormat);
                PrintHeader(formattedTime);
                Console.WriteLine(string.Format(logStreamFormat, levelString, message));

                File.AppendAllText(logPath, string.Format(logFileFormat, formattedTime, PublicDefines.id, levelString, message));
            }
        }

        public static void Error(string message)
        {
            if (level >= LogLevel.ERROR)
            {
                const string levelString = "ERROR";
                string formattedTime = DateTime.Now.ToString(timeFormat);
                PrintHeader(formattedTime);
                Console.WriteLine(string.Format(logStreamFormat, levelString, message));

                File.AppendAllText(logPath, string.Format(logFileFormat, formattedTime, PublicDefines.id, levelString, message));
            }
        }

        private static void PrintHeader(string formattedTime)
        {
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
}
