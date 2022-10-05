using MiscToolsForMD.SDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MiscToolsForMD
{
    internal class LocalSource : ILyricSource
    {
        public string Name => "local";
        public string Author => "zhanghua000";
        public string Id => Author.ToLower() + "." + Name.ToLower();
        public uint Priority => 0;

        public List<Lyric> GetLyrics(string title, string artist)
        {
            float offset = 0.0f;
            string lyricPath = Path.Combine(SDK.PublicDefines.basePath, "Lyrics");
            Directory.CreateDirectory(lyricPath);
            List<Lyric> result = new List<Lyric>();
            string fileName = string.Format("{0}-{1}.lrc", title, artist);
            string filePath = Path.Combine(lyricPath, fileName);
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
                                Regex regexWord = new Regex(@".*\](.*)");
                                Match matchesWord = regexWord.Match(line);
                                string content = matchesWord.Groups[1].Value;
                                Regex regexTime = new Regex(@"\[([0-9.:]*)\]", RegexOptions.Compiled);
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
