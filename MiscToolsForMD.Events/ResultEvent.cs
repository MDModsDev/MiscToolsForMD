using Il2CppPeroPeroGames.GlobalDefines;

namespace MiscToolsForMD.SDK
{
    public enum Result
    {
        UNKNOWN,
        MISS,
        GREAT,
        PERFECT
    }

    public readonly struct PlayResultInfo
    {
        internal PlayResultInfo(Result result, NoteType noteType)
        {
            this.result = result;
            this.noteType = noteType;
        }

        public Result result { get; }
        public NoteType noteType { get; }
    }

    public class ResultEvent : Event<PlayResultInfo>
    { }

    public class BeforeResultGeneratedEvent : ResultEvent
    { }

    public class AfterResultGeneratedEvent : ResultEvent
    { }
}
