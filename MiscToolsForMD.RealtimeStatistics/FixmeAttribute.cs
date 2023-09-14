namespace MiscToolsForMD.Sdk;

/// <summary>
/// The attribute which helps developers fix bugs.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class FixmeAttribute : PrintSupportedAttribute
{
    private readonly string msg;

    /// <summary>
    /// The constructor of <see cref="FixmeAttribute"/>
    /// </summary>
    /// <param name="msg">The message to the developers.</param>
    public FixmeAttribute(string msg)
    {
        this.msg = msg;
    }

    /// <inheritdoc/>
    public override void PrintSelf(string methodFullName)
    {
        LoggerShim.loggerInstance?.Msg(string.Format("{0}: Fixme: {1}", methodFullName, msg));
    }
}
