namespace MiscToolsForMD.Sdk;

/// <summary>
/// An attribute which can print something.
/// </summary>
public abstract class PrintSupportedAttribute : ExtendedAttribute
{
    /// <summary>
    /// Print something
    /// </summary>
    /// <param name="methodFullName">The method name which method has the attribute</param>
    public abstract void PrintSelf(string methodFullName);
}
