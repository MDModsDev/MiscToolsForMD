using Il2CppFormulaBase;
using System.Text;
using System.Text.Json;
namespace MiscToolsForMD.Sdk;

public static class Events
{
    /// <summary>
    /// Event that updates current chart's info.<br/>
    /// Will be triggered after <see cref="StageBattleComponent.InitGame"/> is called.
    /// </summary>
    public delegate void MusicInfoUpdatedEventHandler(MusicDisplayInfo musicDisplayInfo);
    /// <summary>
    /// Event handler for <see cref="MusicInfoUpdatedEventHandler"/>
    /// </summary>
    private static event MusicInfoUpdatedEventHandler MusicInfoUpdated = new(
        delegate (MusicDisplayInfo displayInfo)
        {
            SDKLogger.Debug(
                string.Format(
                    "[{0}]: Calling callbacks with argument {1}",
                    DiagnosticUtils.GetCallerFullName(),
                    Encoding.UTF8.GetString(JsonSerializer.SerializeToUtf8Bytes(displayInfo, jsonSerializeOptions))
                )
            );
        }
    );
    /// <summary>
    /// Event with play result has not been modified by game.<br/>
    /// Will be triggered before <see cref="GameTouchPlay.TouchResult(int, byte, uint, TimeNodeOrder, bool)"/> is called.
    /// </summary>
    public delegate void BeforeResultGeneratedEventHandler(ref byte result, PlayResultInfo resultInfo);

    /// <summary>
    /// Event handler for <see cref="BeforeResultGeneratedEventHandler"/>
    /// </summary>
    private static event BeforeResultGeneratedEventHandler BeforeResultGenerated = new(
        delegate (ref byte result, PlayResultInfo resultInfo)
        {
            SDKLogger.Debug(
                string.Format(
                    "[{0}]: Calling callbacks with argument {1}",
                    DiagnosticUtils.GetCallerFullName(),
                    Encoding.UTF8.GetString(JsonSerializer.SerializeToUtf8Bytes(resultInfo, jsonSerializeOptions))
                )
            );
        }
    );
    /// <summary>
    /// Event with play result has been modified by game.<br/>
    /// Will be triggered after <see cref="GameTouchPlay.TouchResult(int, byte, uint, TimeNodeOrder, bool)"/> is called.
    /// </summary>
    public delegate void AfterResultGeneratedEventHandler(ref byte result, PlayResultInfo resultInfo);

    /// <summary>
    /// Event handler for <see cref="AfterResultGeneratedEventHandler"/>
    /// </summary>
    private static event AfterResultGeneratedEventHandler AfterResultGenerated = new(
        delegate (ref byte result, PlayResultInfo resultInfo)
        {
            SDKLogger.Debug(
                string.Format(
                    "[{0}]: Calling callbacks with argument {1}",
                    DiagnosticUtils.GetCallerFullName(),
                    Encoding.UTF8.GetString(JsonSerializer.SerializeToUtf8Bytes(resultInfo, jsonSerializeOptions))
                )
            );
        }
    );
    /// <summary>
    /// JSON serialize option for printing objects
    /// </summary>
    private static readonly JsonSerializerOptions jsonSerializeOptions = new()
    {
        WriteIndented = true,
    };
    /// <summary>
    /// Add a callback for <see cref="MusicInfoUpdated"/> event
    /// </summary>
    /// <param name="eventCallback">The callback delegate</param>
    public static void AddCallback(MusicInfoUpdatedEventHandler eventCallback) =>
        MusicInfoUpdated += delegate (MusicDisplayInfo displayInfo)
        {
            try { eventCallback(displayInfo); }
            catch (Exception) { }
        };

    /// <summary>
    /// Add a callback for <see cref="BeforeResultGenerated"/> event
    /// </summary>
    /// <param name="eventCallback">The callback delegate</param>
    public static void AddCallback(BeforeResultGeneratedEventHandler eventCallback) =>
        BeforeResultGenerated += delegate (ref byte result, PlayResultInfo resultInfo)
        {
            try { eventCallback(ref result, resultInfo); }
            catch (Exception) { }
        };

    /// <summary>
    /// Add a callback for <see cref="AfterResultGenerated"/> event
    /// </summary>
    /// <param name="eventCallback">The callback delegate</param>
    public static void AddCallback(AfterResultGeneratedEventHandler eventCallback) =>
        AfterResultGenerated += delegate (ref byte result, PlayResultInfo resultInfo)
        {
            try { eventCallback(ref result, resultInfo); }
            catch (Exception) { }
        };
}
