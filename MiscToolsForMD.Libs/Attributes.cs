using System.Reflection;

namespace MiscToolsForMD.SDK
{
    public static class AttributeChecker
    {
        public static void Check(object o)
        {
            foreach (MethodInfo methodInfo in o.GetType().GetMethods())
            {
                if ((methodInfo.DeclaringType != null) && (methodInfo.DeclaringType.FullName != null))
                {
                    string methodFullName = string.Format("{0}.{1}", methodInfo.DeclaringType.FullName, methodInfo.Name);
                    methodInfo.GetCustomAttribute<PrintSupportedAttribute>()?.PrintSelf(methodFullName);
                }
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
            SDKLogger.Info(fullMsg);
        }
    }

    public abstract class PrintSupportedAttribute : Attribute
    {
        public abstract void PrintSelf(string methodInfo);
    }
}
