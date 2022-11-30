namespace MiscToolsForMD.SDK
{
    public class MusicDisplayInfo
    {
        /// <summary>
        /// Title of the song.
        /// </summary>
        public string musicName;

        /// <summary>
        /// Author of the song
        /// </summary>
        public string authorName;
        /// <summary>
        /// Level of the song
        /// </summary>
        public int musicLevel;
        /// <summary>
        /// Difficulty of the song. <br/>See
        /// <seealso cref="Difficulty"/>
        /// </summary>
        public Difficulty difficulty;

        public override string ToString()
        {
            return string.Format("{0}-{1}", musicName, authorName);
        }
    }
}
