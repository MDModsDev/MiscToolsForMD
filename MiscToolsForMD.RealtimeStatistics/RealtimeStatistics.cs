using MelonLoader;
using MiscToolsForMD.Sdk;
using System.Reflection;

namespace MiscToolsForMD.RealtimeStatistics;

/// <summary>
/// The MelonMod, this mod's entrance
/// </summary>
public class RealtimeStatisticsMod : MelonMod
{
    /// <inheritdoc/>
    public override void OnLateInitializeMelon()
    {
        LoggerShim.loggerInstance = LoggerInstance;
        List<MethodInfo> methods = new();
        Array.ForEach(Assembly.GetExecutingAssembly().GetTypes(), t => methods.AddRange(t.GetMethods()));
        methods.ForEach(
            delegate (MethodInfo m)
            {
                if (m.IsDefined(typeof(PrintSupportedAttribute)))
                {
                    string? typeFullName = m.DeclaringType?.FullName;
                    if (typeFullName != null)
                    {
                        m.GetCustomAttribute<PrintSupportedAttribute>()?.PrintSelf(
                            string.Format(".", new string[2] { typeFullName, m.Name }
                        ));
                    }
                }
            }
        );
        LoggerInstance.Msg("MiscToolsForMD.RealtimeStatistics is initialized!");
    }
}
