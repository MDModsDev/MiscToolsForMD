using System.Reflection;

namespace MiscToolsForMD.Sdk;

/// <summary>
/// The helper class used for checking attributes.
/// </summary>
public static class AttributeChecker
{
    /// <summary>
    /// Check object if has <see cref="PrintSupportedAttribute"/>
    /// </summary>
    /// <param name="o">The object to check.</param>
    public static void CheckPrintSupportedAttribute(object o)
    {
        foreach (MethodInfo methodInfo in o.GetType().GetMethods())
        {
            if ((methodInfo.DeclaringType != null) && (methodInfo.DeclaringType.FullName != null))
            {
                string methodFullName = string.Format("{0}.{1}", methodInfo.DeclaringType.FullName, methodInfo.Name);
                PrintSupportedAttribute? attribute = methodInfo.GetCustomAttribute<PrintSupportedAttribute>();
                if (attribute != null && !attribute.isChecked)
                {
                    attribute.PrintSelf(methodFullName);
                    attribute.isChecked = true;
                }
            }
        }
    }
}
