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

        public override string ToString()
        {
            return string.Format("{0}-{1}", musicName, authorName);
        }
    }
}
