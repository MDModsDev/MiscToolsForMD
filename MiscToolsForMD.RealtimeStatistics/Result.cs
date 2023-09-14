namespace MiscToolsForMD.Sdk;

/// <summary>
/// The result after player operated on a note.
/// </summary>
public enum Result
{
    /// <summary>
    /// Unknown result, for placeholder.
    /// </summary>
    UNKNOWN,

    /// <summary>
    /// Miss result, when player missed the note.
    /// </summary>
    MISS,

    /// <summary>
    /// Great result, when player get great result from game.
    /// </summary>
    GREAT,

    /// <summary>
    /// Perfect result, when player get perfect result from game.
    /// </summary>
    PERFECT
}
