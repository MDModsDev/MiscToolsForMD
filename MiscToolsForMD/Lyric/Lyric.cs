using System.Collections.Generic;

namespace MiscToolsForMD.Lyric
{
    /// <summary>
    /// Lyric object used to display lyric in game
    /// </summary>
    public class Lyric
    {
        public string content;
        public float time;

        public static Lyric GetLyricByTime(List<Lyric> lst, float time)
        {
            Lyric lyricFound = lst.Find(lyric => lyric.time == time);
            if (lyricFound == null)
            {
                lyricFound = new Lyric() { time = time, content = "" };
            }
            return lyricFound;
        }
    }

    public interface ILyricSource
    {
        string name { get; }
        string author { get; }
        string id { get; }
        uint priority { get; }

        List<Lyric> GetLyrics(string title, string artist);
    }
}
