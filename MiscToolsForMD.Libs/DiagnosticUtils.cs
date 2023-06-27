using System.Diagnostics;
using System.Reflection;

namespace MiscToolsForMD.SDK
{
    public static class DiagnosticUtils
    {
        public static string GetCallerFullName()
        {
            StackTrace stackTrace = new(true);
            MethodBase? methodBase = stackTrace.GetFrame(1)?.GetMethod();
            Type? methodType = methodBase?.DeclaringType;
            if ((methodType == null) || (methodBase == null))
            {
                return "Unknown";
            }
            return methodType.FullName + "." + methodBase.Name;
        }
    }
}
