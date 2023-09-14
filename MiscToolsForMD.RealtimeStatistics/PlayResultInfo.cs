namespace MiscToolsForMD.Sdk;

/// <summary>
/// The result of player operation
/// </summary>
public readonly struct PlayResultInfo
{
    internal PlayResultInfo(ref byte resultByte, Result result, NoteName noteName)
    {
        this.result = result;
        this.noteName = noteName;
        this.resultByte = resultByte;
    }

    /// <summary>
    /// The reference of the result. <br/>
    /// Used for modifying play result.
    /// </summary>
    public ref byte resultByte => ref resultByte;

    /// <summary>
    /// Result in enum.
    /// </summary>
    public Result result { get; }

    /// <summary>
    /// The name of the note
    /// </summary>
    public NoteName noteName { get; }
}
