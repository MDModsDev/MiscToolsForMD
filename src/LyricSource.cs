using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MiscToolsForMD
{
    public interface ILyricSource
    {
        string Name { get; }
        string Author { get; }
        string Id { get; }
        uint Priority { get; }

        List<Lyric> GetLyrics(string title, string artist);
    }

    public class Lyric
    {
        public string content;
        public float time;

        public static Lyric GetLyricByTime(List<Lyric> lst, float time)
        {
            foreach (Lyric lyric in lst)
            {
                if (lyric.time == time)
                {
                    return lyric;
                }
            }
            return new Lyric() { time = time, content = "No Lyric" };
        }
    }

    public class LocalSource : ILyricSource
    {
        public string Name => "local";
        public string Author => "zhanghua000";
        public string Id => Author.ToLower() + "." + Name.ToLower();
        public uint Priority => 0;

        public List<Lyric> GetLyrics(string title, string artist)
        {
            float offset = 0.0f;
            if (!Directory.Exists("UserData" + Path.DirectorySeparatorChar + "Lyrics"))
            {
                Directory.CreateDirectory("UserData" + Path.DirectorySeparatorChar + "Lyrics");
            }
            List<Lyric> result = new();
            string fileName = title + "-" + artist + ".lrc";
            string filePath = "UserData" + Path.DirectorySeparatorChar + "Lyrics" + Path.DirectorySeparatorChar + fileName;
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
                            offset = float.Parse(line.Substring(line.IndexOf(":") + 1).TrimEnd(']')) / 1000; ;
                        }
                        else
                        {
                            try
                            {
                                Regex regexWord = new(@".*\](.*)");
                                Match matchesWord = regexWord.Match(line);
                                string content = matchesWord.Groups[1].Value;
                                Regex regexTime = new(@"\[([0-9.:]*)\]", RegexOptions.Compiled);
                                MatchCollection matchesTime = regexTime.Matches(line);
                                foreach (Match matchTime in matchesTime)
                                {
                                    float time = (float)TimeSpan.Parse("00:" + matchTime.Groups[1].Value).TotalSeconds + offset;
                                    result.Add(new Lyric() { content = content, time = time });
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                    result.OrderBy(lyric => lyric.time);
                }
            }
            return result;
        }
    }
}
