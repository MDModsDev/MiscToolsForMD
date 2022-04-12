using Assets.Scripts.PeroTools.Commons;
using FormulaBase;
using GameLogic;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace MiscToolsForMD
{
    internal class Defines
    {
        public static readonly Dictionary<string, string> keyDisplayNames = new()
        {
            { "Backspace", "←" },
            { "Delete", "Del" },
            { "Tab", "Tab" },
            { "Return", "↲" },
            { "Escape", "Esc" },
            { "Keypad0", "0" },
            { "Keypad1", "1" },
            { "Keypad2", "2" },
            { "Keypad3", "3" },
            { "Keypad4", "4" },
            { "Keypad5", "5" },
            { "Keypad6", "6" },
            { "Keypad7", "7" },
            { "Keypad8", "8" },
            { "Keypad9", "9" },
            { "KeypadPeriod", "." },
            { "KeypadDivide", "/" },
            { "KeypadMultiply", "*" },
            { "KeypadMinus", "-" },
            { "KeypadPlus", "+" },
            { "KeypadEnter", "↲" },
            { "KeypadEquals", "=" },
            { "UpArrow", "↑" },
            { "DownArrow", "↓" },
            { "RightArrow", "→" },
            { "LeftArrow", "←" },
            { "Insert", "Ins" },
            { "Home", "Home" },
            { "End", "End" },
            { "PageUp", "PgUp" },
            { "PageDown", "PgDn" },
            { "Alpha0", "0" },
            { "Alpha1", "1" },
            { "Alpha2", "2" },
            { "Alpha3", "3" },
            { "Alpha4", "4" },
            { "Alpha5", "5" },
            { "Alpha6", "6" },
            { "Alpha7", "7" },
            { "Alpha8", "8" },
            { "Alpha9", "9" },
            { "Exclaim", "!" },
            { "DoubbleQuote", "\"" },
            { "Hash", "#" },
            { "Dollar", "$" },
            { "Percent", "%" },
            { "Ampersand", "&" },
            { "Quote", "'" },
            { "LeftParen", "(" },
            { "RightParen", ")" },
            { "Asterisk", "*" },
            { "Plus", "+" },
            { "Comma", "," },
            { "Minus", "-" },
            { "Period", "." },
            { "Slash", "/" },
            { "Colon", ":" },
            { "Semicolon", ";" },
            { "Less", "<" },
            { "Equals", "=" },
            { "Greater", ">" },
            { "Question", "?" },
            { "At", "@" },
            { "LeftBracket", "[" },
            { "RightBracket", "]" },
            { "Backslash", "\\" },
            { "Caret", "^" },
            { "Underscore", "_" },
            { "BackQuote", "`" },
            { "LeftCurlyBracket", "{" },
            { "RightCurlyBracket", "}" },
            { "Pipe", "|" },
            { "Tilde", "~" }
        };

        public static readonly string basePath = Path.Combine("UserData", "MiscToolsForMD");
        public static readonly string configPath = Path.Combine(basePath, "MiscToolsForMD.json");
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

    internal class Cache
    {
        private readonly List<int> recordedIds = new();
        private readonly List<MusicData> musicDatas = new();

        public void CleanCache()
        {
            recordedIds.Clear();
            Il2CppSystem.Collections.Generic.List<MusicData> musicDatasInIl2Cpp = Singleton<StageBattleComponent>.instance.GetMusicData();
            for (int i = 0; i < musicDatasInIl2Cpp.Count; i++)
            {
                musicDatas.Add(musicDatasInIl2Cpp[i]);
            }
        }

        public void AddRecordedId(int id)
        {
            if (!IsIdRecorded(id))
            {
                recordedIds.Add(id);
            }
        }

        public bool IsIdRecorded(int id)
        {
            return recordedIds.Contains(id);
        }

        public List<MusicData> GetAllMusicDatasBeforeId(int id)
        {
            int lastId = 0;
            if (recordedIds.Count != 0)
            {
                lastId = recordedIds.Max();
            }
            if (id > lastId)
            {
                return musicDatas.FindAll(musicData => musicData.objId <= id);
            }
            return musicDatas.FindAll(musicData => musicData.objId <= lastId);
        }

        public void ExportMusicDatas(string path)
        {
            string musicDatasJsonStr = JsonConvert.SerializeObject(musicDatas, Formatting.Indented);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, musicDatasJsonStr);
        }
    }

    public class MusicDisplayInfo
    {
        public string musicName;
        public string authorName;
    }

    public class Config
    {
        public bool debug = false;
        public int size = 24;
        public LyricConfig lyric = new();
        public IndicatorConfig indicator = new();
        public HardCoreConfig hardcore = new();
        public SoftCoreConfig softcore = new();
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
        public APIndicatorConfig ap = new();
        public KeyIndicatorConfig key = new();
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
        public string error = "#FF0000";
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

    public class KeyConfigObj
    {
        public KeyListObj KeyList;
        public string IsChanged;
        public string KeyBoardProposal;
        public string HandleProposal;
        public string IsVibration;
        public string FeverKey;
    }

    public class KeyObj
    {
        public string Key;
        public string Type;
    }

    public class KeyListObj
    {
        public List<KeyObj> Custom;
    }
}
