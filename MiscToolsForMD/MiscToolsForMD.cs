using MelonLoader;
using MiscToolsForMD.SDK;

namespace MiscToolsForMD
{
    public class MiscToolsForMD : MelonMod
    {
        public override void OnInitializeMelon()
        {
            SDKLogger.level = LogLevel.DEBUG;
            LoggerInstance.Msg("MiscToolsForMD is initialized!");
        }
    }
}
