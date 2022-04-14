using Assets.Scripts.PeroTools.Commons;
using FormulaBase;
using MiscToolsForMD.SDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MiscToolsForMD.MOD
{
    public class PublicDefines
    {
    }

    internal class InternalDefines
    {
    }

    internal enum ControlType
    {
        Air,
        Fever,
        Ground
    }

    internal class KeyInfo
    {
        public ControlType type;
        public uint count;
        public KeyCode code;
        public GUIStyle style;
        public Color displayColor;

        public override string ToString()
        {
            return "type:" + type + ";code:" + code;
        }

        public void AddCount(uint countToAdd = 1)
        {
            if (Singleton<StageBattleComponent>.instance.isInGame)
            {
                count += countToAdd;
            }
        }

        public void SetColor(Color color)
        {
            if (Singleton<StageBattleComponent>.instance.isInGame)
            {
                style.normal.textColor = color;
            }
            else
            {
                ResetColor();
            }
        }

        public void ResetColor()
        {
            style.normal.textColor = displayColor;
        }
    }

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

    public class Config
    {
        public bool debug = false;
        public int size = 24;
        public LyricConfig lyric = new LyricConfig();
        public IndicatorConfig indicator = new IndicatorConfig();
        public HardCoreConfig hardcore = new HardCoreConfig();
        public SoftCoreConfig softcore = new SoftCoreConfig();
    }

    public class LyricConfig
    {
        public bool enabled = true;
        public int size = 36;
        public int x = -1;
        public int y = -1;
        public int width = 500;
        public int height = 100;
    }

    public class IndicatorConfig
    {
        public APIndicatorConfig ap = new APIndicatorConfig();
        public KeyIndicatorConfig key = new KeyIndicatorConfig();
        public int x = -1;
        public int y = -1;
        public int width = 500;
        public int height = 100;
    }

    public class APIndicatorConfig
    {
        public bool enabled = true;
        public bool manual = false;
        public int size = 36;
        public string ap = "#FFD700";
        public string great = "#4169E1";
        public string miss = "#FFFFFF";
    }

    public class KeyIndicatorConfig
    {
        public bool enabled = true;
        public int size = 24;
        public string display = "#000000";
        public string pressing = "#FFFFFF";
    }

    public class HardCoreConfig
    {
        public bool enabled = false;
    }

    public class SoftCoreConfig
    {
        public bool enabled = false;
    }
}
