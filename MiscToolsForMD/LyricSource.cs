using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MiscToolsForMD
{
    public interface LyricSource
    {
        string Name { get; }
        string Author { get; }
        string Id { get; }
        int Priority { get; }

        List<Lyric> GetLyrics(string title, string artist);
    }

    public class Lyric
    {
        public string content;
        public double tick;

        public static Lyric GetLyricByTick(List<Lyric> lst, double tick)
        {
            foreach (Lyric lyric in lst)
            {
                if (lyric.tick == tick)
                {
                    return lyric;
                }
            }
            return new Lyric() { tick = tick, content = "No Lyric" };
        }
    }

    public class LocalSource : LyricSource
    {
        public string Name => "local";
        public string Author => "zhanghua000";
        public string Id => Author.ToLower() + "." + Name.ToLower();
        public int Priority => 0;

        public List<Lyric> GetLyrics(string title, string artist)
        {
            double offset = 0.0;
            if (!Directory.Exists("Mods" + Path.DirectorySeparatorChar + "Lyrics"))
            {
                Directory.CreateDirectory("Mods" + Path.DirectorySeparatorChar + "Lyrics");
            }
            List<Lyric> result = new List<Lyric>();
            string fileName = title + "-" + artist + ".lrc";
            string filePath = "Mods" + Path.DirectorySeparatorChar + "Lyrics" + Path.DirectorySeparatorChar + fileName;
            if (File.Exists(filePath))
            {
                string[] fileLines = File.ReadAllLines(filePath);
                if (fileLines.Length > 0)
                {
                    foreach (string line in fileLines)
                    {
                        if (line.StartsWith("[ti:") || line.StartsWith("[ar:") || line.StartsWith("[al:") || line.StartsWith("[by:"))
                        {
                            continue;
                        }
                        else if (line.StartsWith("[offset:"))
                        {
                            offset = double.Parse(line.Substring(line.IndexOf(":") + 1).TrimEnd(']')) / 1000; ;
                        }
                        else
                        {
                            try
                            {
                                Regex regexWord = new Regex(@".*\](.*)");
                                Match matchesWord = regexWord.Match(line);
                                string content = matchesWord.Groups[1].Value;
                                Regex regexTime = new Regex(@"\[([0-9.:]*)\]", RegexOptions.Compiled);
                                MatchCollection matchesTime = regexTime.Matches(line);
                                foreach (Match matchTime in matchesTime)
                                {
                                    double tick = TimeSpan.Parse("00:" + matchTime.Groups[1].Value).TotalSeconds + offset;
                                    result.Add(new Lyric() { content = content, tick = tick });
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                    result.OrderBy(lyric => lyric.tick);
                }
            }
            return result;
        }
    }
}
