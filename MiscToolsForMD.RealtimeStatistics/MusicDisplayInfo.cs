namespace MiscToolsForMD.Sdk;

/// <summary>
/// Infomation of current music.
/// </summary>
public readonly struct MusicDisplayInfo
{
    internal MusicDisplayInfo(string musicName, string authorName, string albumJsonName, int stageLevel, Difficulty difficulty)
    {
        this.musicName = musicName;
        this.authorName = authorName;
        this.albumJsonName = albumJsonName;
        this.stageLevel = stageLevel;
        this.difficulty = difficulty;
    }

    /// <summary>
    /// The name of the music.
    /// </summary>
    public string musicName { get; }

    /// <summary>
    /// The author of the music.
    /// </summary>
    public string authorName { get; }

    /// <summary>
    /// The album json name of the music.
    /// </summary>
    public string albumJsonName { get; }

    /// <summary>
    /// The level of the music.
    /// </summary>
    public int stageLevel { get; }

    /// <summary>
    /// The difficulty of the name.
    /// </summary>
    public Difficulty difficulty { get; }
}
