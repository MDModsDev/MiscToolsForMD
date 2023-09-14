namespace MiscToolsForMD.Sdk;

public readonly struct PlayResultInfo
{
    internal PlayResultInfo(Result result, NoteName noteName)
    {
        this.result = result;
        this.noteName = noteName;
    }

    public Result result { get; }
    public NoteName noteName { get; }
}
