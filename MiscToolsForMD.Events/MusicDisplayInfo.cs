namespace MiscToolsForMD.SDK
{
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

        public string musicName { get; }
        public string authorName { get; }
        public string albumJsonName { get; }
        public int stageLevel { get; }
        public Difficulty difficulty { get; }
    }
}
