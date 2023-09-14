using MelonLoader;

namespace MiscToolsForMD.Sdk;

/// <summary>
/// The helper class for getting LoggerInstance of a MelonMod
/// </summary>
public static class LoggerShim
{
    /// <summary>
    /// The LoggerInstance, <c>null</c> if it is not set.
    /// </summary>
    public static MelonLogger.Instance? loggerInstance
    {
        get
        {
            return loggerInstance;
        }
        set
        {
            loggerInstance = value;
        }
    }
}
