using MelonLoader;

namespace MiscToolsForMD.SyncLyrics;

/// <summary>
/// The MelonMod, the entrance of the mod.
/// </summary>
public class SyncLyricsMod : MelonMod
{
    /// <inheritdoc/>
    public override void OnLateInitializeMelon()
    {
        LoggerInstance.Msg("MiscToolsForMD.SyncLyrics is initialized!");
    }
}
