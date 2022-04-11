using Assets.Scripts.PeroTools.Commons;
using FormulaBase;
using GameLogic;
using System.Collections.Generic;
using System.IO;
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

        public static readonly string configPath = Path.Combine("UserData", "MiscToolsForMD.json");
        public static readonly string langPath = Path.Combine("UserData", "Lang", "MiscToolsForMD");
        public static readonly Color apColor = new(255 / 256f, 215 / 256f, 0 / 256f);
        public static readonly Color greatColor = new(65 / 256f, 105 / 256f, 225 / 256f);
        public static readonly Color missColor = Color.white;
        public static readonly Color errColor = Color.red;
        public static readonly Color displayColor = Color.black;
        public static readonly Color pressingColor = Color.white;
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

        public override string ToString()
        {
            return "type:" + type + ";code:" + code;
        }
    }

    internal class Cache
    {
        private readonly List<int> recordedIds = new();
        private readonly List<MusicData> musicList = new();
        private int lastId = 0;
        private Il2CppSystem.Collections.Generic.List<MusicData> musicDatas;

        public void CleanCache()
        {
            recordedIds.Clear();
            musicList.Clear();
            lastId = 0;
            musicDatas = Singleton<StageBattleComponent>.instance.GetMusicData();
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
            List<MusicData> result;
            if (id < 0 || id > musicDatas.Count)
            {
                id = musicDatas.Count - 1;
            }
            if (id >= lastId + 1)
            {
                for (int i = lastId + 1; i <= id; i++)
                {
                    musicList.Add(musicDatas[i]);
                }
                result = musicList;
            }
            else
            {
                result = musicList.FindAll(musicData => musicList.IndexOf(musicData) <= id);
            }
            lastId = id;
            return result;
        }
    }

    public class Config
    {
        public bool debug = false;
        public LyricConfig lyric = new();
        public IndicatorConfig indicator = new();
        public HardCoreConfig hardcore = new();
        public SoftCoreConfig softcore = new();
    }

    public class LyricConfig
    {
        public bool enabled = true;
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
    }

    public class KeyIndicatorConfig
    {
        public bool enabled = true;
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
