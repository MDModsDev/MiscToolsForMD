using System.Diagnostics;
using System.Reflection;

namespace MiscToolsForMD.Sdk;

/// <summary>
/// Utils used for diagnostic reasons.
/// </summary>
public static class DiagnosticUtils
{
    /// <summary>
    /// Get FQDN of the caller
    /// </summary>
    /// <returns>The FQDN, <c>&lt;UnamedType&gt;.&lt;UnamedMethod&gt;</c> if not found.</returns>
    public static string GetCallerFullName()
    {
        StackTrace stackTrace = new(true);
        MethodBase? methodBase = stackTrace.GetFrame(1)?.GetMethod();
        string? methodName = methodBase?.Name;
        string? typeName = methodBase?.DeclaringType?.FullName;
        if (typeName != null && methodName != null)
        {
            return string.Join(".", new string[2] { typeName, methodName });
        }
        return "<UnamedType>.<UnamedMethod>";
    }
}
