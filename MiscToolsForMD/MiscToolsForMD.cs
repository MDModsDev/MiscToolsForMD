using Assets.Scripts.GameCore.GamePlay;
using Assets.Scripts.GameCore.HostComponent;
using Assets.Scripts.PeroTools.Commons;
using Assets.Scripts.PeroTools.Managers;
using Assets.Scripts.Database;
using HarmonyLib;
using ModHelper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PeroPeroGames.GlobalDefines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MiscToolsForMD
{
    public class MiscToolsForMD : IMod
    {
        public string Name => "MiscToolsForMD";
        public string Description => "Misc Tools for Muse Dash(Realtime accuracy indicator, key indicator...)";
        public string Author => "zhanghua000";
        public string HomePage => "https://github.com/zhanghua000/MiscToolsForMD";
        public List<ILyricSource> lyricSources = new List<ILyricSource>();
        public static Config config;
        public static MiscToolsForMD instance;
        public static Indicator indicator;

        public void DoPatching()
        {
            if (System.IO.File.Exists("Mods" + System.IO.Path.DirectorySeparatorChar + Name + ".json"))
            {
                config = JsonConvert.DeserializeObject<Config>(System.IO.File.ReadAllText("Mods" + System.IO.Path.DirectorySeparatorChar + Name + ".json"));
            }
            else
            {
                config = new Config();
                SaveConfig();
            }
            ModLogger.Debug("Debug mode:" + config.debug);
            Harmony.DEBUG = config.debug;
            Harmony harmony = new Harmony(string.Format("com.github.{0}.{1}", Author, Name.ToLower()));
            ModLogger.Debug(string.Format("Harmony instance initialized with ID {0}.", harmony.Id));
            if (Harmony.HasAnyPatches(harmony.Id))
            {
                ModLogger.Debug("Harmony instance has patches, this may result unexpected behaviors.");
            }
            if (config.ap_indicator || config.key_indicator || config.lyric)
            {
                MethodInfo start = typeof(GameOptimization).GetMethod("Init");
                MethodInfo startPatch = typeof(MiscToolsForMD).GetMethod(nameof(InitUI), BindingFlags.Static | BindingFlags.NonPublic);
                TryPatch(harmony, start, null, new HarmonyMethod(startPatch));
            }
            else
            {
                ModLogger.Debug("Nothing was applied.");
            }
            if (config.ap_indicator)
            {
                MethodInfo noteResult = typeof(BattleEnemyManager).GetMethod("SetPlayResult");
                MethodInfo apPatch = typeof(MiscToolsForMD).GetMethod(nameof(SetPlayResult), BindingFlags.Static | BindingFlags.NonPublic);
                TryPatch(harmony, noteResult, null, new HarmonyMethod(apPatch));
            }
            if (config.lyric)
            {
                lyricSources.Add(new LocalSource());
                // TODO: Load other lyric source
                lyricSources.OrderBy(lyricSource => lyricSource.Priority);
            }
            instance = this;
            ModLogger.Debug("MiscToolsForMD Loads Completed.");
        }

        private static void SetPlayResult(int idx, byte result, bool isMulStart)
        {
            Log("Note " + idx + " with result " + result + " and mul start: " + isMulStart + " was captured.");
            if (indicator == null)
            {
                Log("No UI instance");
            }
            else
            {
                try
                {
                    indicator.SetAccuracy(idx, result, isMulStart);
                }
                catch (Exception e)
                {
                    Log(e.ToString(), "Failed to calculate accuracy, open debug mode for more info.");
                }
            }
        }

        private static void InitUI()
        {
            GameObject ui = GameObject.Find("MiscToolsUI");
            if (ui == null)
            {
                ui = new GameObject("MiscToolsUI");
                Log("Creating new GameObject");
            }
            else
            {
                Log("Using existing GameObject");
            }
            indicator = ui.AddComponent(typeof(Indicator)) as Indicator;
            Log("Created UI");
        }

        public static List<string> GetControlKeys()
        {
            List<string> keys = new List<string>();
            string text;
            if (PlayerPrefs.HasKey("Controller"))
            {
                text = Singleton<ConfigManager>.instance.GetString("Controller");
            }
            else
            {
                text = "{\"Keylist\":{ \"Custom\":[{\"Key\":\"None\",\"Type\":\"BattleAir\"},{\"Key\":\"None\",\"Type\":\"BattleAir\"},{\"Key\":\"None\",\"Type\":\"BattleAir\"},{\"Key\":\"None\",\"Type\":\"BattleAir\"},{\"Key\":\"None\",\"Type\":\"BattleGround\"},{\"Key\":\"None\",\"Type\":\"BattleGround\"},{\"Key\":\"None\",\"Type\":\"BattleGround\"},{\"Key\":\"None\",\"Type\":\"BattleGround\"}]},\"IsChanged\":\"false\",\"KeyBoardProposal\":\"Default\",\"HandleProposal\":\"Default\",\"IsVibration\":\"true\",\"FeverKey\":\"Space\"}";
            }
            KeyConfigObj keyConfig = JsonConvert.DeserializeObject<KeyConfigObj>(text);
            foreach (KeyObj key in keyConfig.KeyList.Custom)
            {
                if (key.Key != "None")
                {
                    keys.Add(key.Key);
                }
            }
            keys.Insert(keys.Count / 2, keyConfig.FeverKey);
            return keys;
        }

        private void TryPatch(Harmony harmony, MethodInfo orig, HarmonyMethod prefix = null, HarmonyMethod postfix = null, HarmonyMethod transpiler = null, HarmonyMethod finalizer = null)
        {
            List<HarmonyMethod> methods = new List<HarmonyMethod>() { prefix, postfix, transpiler, finalizer };
            if (methods.All(method => method is null) || methods.Count == 0)
            {
                Log("Failed to patch " + orig.ReflectedType.FullName + "." + orig.Name + " because all methods are empty.");
                Log("Details: target: " + (orig is null) + "; prefix: " + (prefix is null) + "; postfix: " + (postfix is null) + "; transpiler: " + (transpiler is null) + "; finalizer: " + (finalizer is null));
            }
            else
            {
                harmony.Patch(orig, prefix, postfix, transpiler, finalizer);
                Log("Patched method " + orig.ReflectedType.FullName + "." + orig.Name);
            }
        }

        public static void Log(object log, object normal_log = null)
        {
            if (config.debug)
            {
                ModLogger.Debug(log);
            }
            else if (normal_log != null)
            {
                ModLogger.Debug(normal_log);
            }
        }

        public void SaveConfig()
        {
            System.IO.File.WriteAllText("Mods" + System.IO.Path.DirectorySeparatorChar + Name + ".json", JsonConvert.SerializeObject(config, Formatting.Indented));
        }
    }

    public class Config
    {
        public bool ap_indicator = true;
        public bool key_indicator = true;
        public bool lyric = true;
        public bool debug = false;
        public int width = 500;
        public int height = 100;
        public int x = -1;
        public int y = -1;
        public int lyric_x = -1;
        public int lyric_y = -1;
        public int lyric_width = 500;
        public int lyric_height = 100;
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

    public class Indicator : MonoBehaviour
    {
        private Rect windowRect = new Rect(MiscToolsForMD.config.x, MiscToolsForMD.config.y, MiscToolsForMD.config.width, MiscToolsForMD.config.height);
        private string accuracyText, lyricContent;
        private List<string> workingKeys;
        private Dictionary<string, uint> counters;
        private Rect lyricWindowRect = new Rect(MiscToolsForMD.config.lyric_x, MiscToolsForMD.config.lyric_y, MiscToolsForMD.config.lyric_width, MiscToolsForMD.config.lyric_height);
        private readonly Dictionary<string, string> keyDisplayNames = new Dictionary<string, string>()
        {
            {"Backspace", "←"}, {"Delete", "Del"}, {"Tab", "Tab"}, {"Return", "↲"}, {"Escape", "Esc"}, {"Keypad0", "0"}, {"Keypad1", "1"}, {"Keypad2", "2"},
            {"Keypad3", "3"}, {"Keypad4", "4"}, {"Keypad5", "5"}, {"Keypad6", "6"}, {"Keypad7", "7"}, {"Keypad8", "8"}, {"Keypad9", "9"}, {"KeypadPeriod", "."},
            {"KeypadDivide", "/"}, {"KeypadMultiply", "*"}, {"KeypadMinus", "-"}, {"KeypadPlus", "+"}, {"KeypadEnter", "↲"}, {"KeypadEquals", "="}, {"UpArrow", "↑"},
            {"DownArrow", "↓"}, {"RightArrow", "→" }, {"LeftArrow", "←"}, {"Insert", "Ins"}, {"Home", "Home"}, {"End", "End"}, {"PageUp", "PgUp"}, {"PageDown", "PgDn"},
            {"Alpha0", "0"}, {"Alpha1", "1"}, {"Alpha2", "2"}, {"Alpha3", "3"},{"Alpha4", "4"}, {"Alpha5", "5"}, {"Alpha6", "6"}, {"Alpha7", "7"}, {"Alpha8", "8"},
            {"Alpha9", "9"}, {"Exclaim", "!"}, {"DoubbleQuote", "\""}, {"Hash", "#"}, {"Dollar", "$"}, {"Percent", "%"}, {"Ampersand", "&"}, {"Quote", "'"},
            {"LeftParen", "("}, {"RightParen", ")"}, {"Asterisk", "*"},{"Plus", "+"}, {"Comma", ","}, {"Minus", "-"}, {"Period", "."}, {"Slash", "/"},{"Colon", ":"},
            {"Semicolon", ";"}, {"Less", "<"}, {"Equals", "="}, {"Greater", ">"}, {"Question", "?"}, {"At", "@"}, {"LeftBracket", "["}, {"RightBracket", "]"},
            {"Backslash", "\\"}, {"Caret", "^"}, {"Underscore", "_"}, {"BackQuote", "`"}, {"LeftCurlyBracket", "{"}, {"RightCurlyBracket", "}"}, {"Pipe", "|"},
            {"Tilde", "~"}
        };
        private int actualWeight = 0;
        private int targetWeight = 0;
        private List<Lyric> lyrics;

        public void OnGUI()
        {
            if (MiscToolsForMD.config.ap_indicator || MiscToolsForMD.config.key_indicator)
            {
                windowRect = GUILayout.Window(0, windowRect, IndicatorWindow, "MiscToolsUI");
            }
            if (MiscToolsForMD.config.lyric)
            {
                lyricWindowRect = GUILayout.Window(1, lyricWindowRect, LyricWindow, "Lyric");
            }
        }

        public void Start()
        {
            bool needUpdateConfig = false;
            if (MiscToolsForMD.config.ap_indicator || MiscToolsForMD.config.key_indicator)
            {
                if (MiscToolsForMD.config.x == -1)
                {
                    MiscToolsForMD.config.x = (Screen.width - MiscToolsForMD.config.width) / 2;
                    needUpdateConfig = true;
                }
                if (MiscToolsForMD.config.y == -1)
                {
                    MiscToolsForMD.config.y = 20;
                    needUpdateConfig |= true;
                }
                accuracyText = "Accuracy: " + 1.ToString("P");
                workingKeys = MiscToolsForMD.GetControlKeys();
                counters = new Dictionary<string, uint>();
                if (workingKeys.Count >= 3 && workingKeys.Count <= 9)
                {
                    foreach (string key in workingKeys)
                    {
                        counters.Add(key, 0);
                    }
                }
                else
                {
                    ModLogger.Debug("Unexcepted Keys List.");
                }
                windowRect = new Rect(MiscToolsForMD.config.x, MiscToolsForMD.config.y, MiscToolsForMD.config.width, MiscToolsForMD.config.height);
                actualWeight = 0;
                targetWeight = 0;
            }
            if (MiscToolsForMD.config.lyric)
            {
                if (MiscToolsForMD.config.lyric_x == -1)
                {
                    MiscToolsForMD.config.lyric_x = (Screen.width - MiscToolsForMD.config.lyric_width) / 2;
                    needUpdateConfig = true;
                }
                if (MiscToolsForMD.config.lyric_y == -1)
                {
                    MiscToolsForMD.config.lyric_y = Screen.height - MiscToolsForMD.config.lyric_height - 100;
                    needUpdateConfig = true;
                }
                // See SetSelectedMusicNameTxt
                string musicName, musicAuthor;
                if (DataHelper.selectedAlbumUid != "collection" || DataHelper.selectedMusicIndex < 0)
                {
                    musicName = Singleton<ConfigManager>.instance.GetConfigStringValue(DataHelper.selectedAlbumName, "uid", "name", DataHelper.selectedMusicUidFromInfoList);
                    musicAuthor = Singleton<ConfigManager>.instance.GetConfigStringValue(DataHelper.selectedAlbumName, "uid", "author", DataHelper.selectedMusicUidFromInfoList);
                }
                else if (DataHelper.collections.Count == 0||DataHelper.collections.Count < DataHelper.selectedMusicIndex)
                {
                    musicName = "?????";
                    musicAuthor = "???";
                }
                else
                {
                    musicName = Singleton<ConfigManager>.instance.GetConfigStringValue(DataHelper.selectedAlbumName, "uid", "name", DataHelper.collections[DataHelper.selectedMusicIndex]);
                    musicAuthor = Singleton<ConfigManager>.instance.GetConfigStringValue(DataHelper.selectedAlbumName, "uid", "author", DataHelper.collections[DataHelper.selectedMusicIndex]);
                }

                MiscToolsForMD.Log("Song name: " + musicName + "; author: " + musicAuthor);
                bool successGetLyric = false;
                foreach (ILyricSource source in MiscToolsForMD.instance.lyricSources)
                {
                    try
                    {
                        lyrics = source.GetLyrics(musicName, musicAuthor);
                        successGetLyric = true;
                        break;
                    }
                    catch (Exception ex)
                    {
                        MiscToolsForMD.Log(ex.ToString(), "Failed to get lyric through source " + source.Name);
                    }

                }
                if (!successGetLyric || lyrics.Count == 0)
                {
                    MiscToolsForMD.Log("No available lyric.");
                }
                lyricWindowRect = new Rect(MiscToolsForMD.config.lyric_x, MiscToolsForMD.config.lyric_y, MiscToolsForMD.config.lyric_width, MiscToolsForMD.config.lyric_height);
                lyricContent = "";
            }
            if (needUpdateConfig)
            {
                MiscToolsForMD.instance.SaveConfig();
            }
        }

        public void Update()
        {
            if (MiscToolsForMD.config.key_indicator)
            {
                foreach (string key in workingKeys)
                {
                    if (Input.GetKeyDown(GetKeyCodeByName(key)))
                    {
                        AddKeyCount(key);
                    }
                }
            }
            if (MiscToolsForMD.config.lyric)
            {
                float time = Singleton<FormulaBase.StageBattleComponent>.instance.timeFromMusicStart;
                lyricContent = Lyric.GetLyricByTime(lyrics, time).content;
            }
        }

        public void OnDestroy()
        {
            MiscToolsForMD.indicator = null;
        }

        public void IndicatorWindow(int windowID)
        {
            GUILayout.BeginVertical();
            if (MiscToolsForMD.config.ap_indicator)
            {
                GUIStyle accuracyStyle = new GUIStyle
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 48
                };
                GUILayout.Label(accuracyText, accuracyStyle);
            }
            if (MiscToolsForMD.config.key_indicator)
            {
                GUILayout.BeginHorizontal();
                foreach (string key in workingKeys)
                {
                    if (key != null)
                    {
                        GUIStyle keyStyle = new GUIStyle
                        {
                            alignment = TextAnchor.MiddleCenter,
                            fontSize = 24
                        };
                        string keyDisplayName;
                        if (keyDisplayNames.ContainsKey(key))
                        {
                            keyDisplayName = keyDisplayNames[key];
                        }
                        else
                        {
                            keyDisplayName = key;
                        }
                        GUILayout.Label(keyDisplayName + "\n\n" + counters[key], keyStyle);
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        public void SetAccuracy(int idx, byte result, bool isMulStart)
        {
            // See https://zh.moegirl.org.cn/Muse_Dash#%E5%87%86%E7%A1%AE%E7%8E%87 for more info
            // Also see:
            // Assets.Scripts.GameCore.HostComponent.BattleEnemyManager.SetPlayResult
            GameLogic.MusicData musicData = Singleton<FormulaBase.StageBattleComponent>.instance.GetMusicDataByIdx(idx);
            MiscToolsForMD.Log("Music data info: isLongPressing: " + musicData.isLongPressing + "; doubleIdx: " + musicData.doubleIdx + "; isDouble: " +
                musicData.isDouble + "; isLongPressEnd: " + musicData.isLongPressEnd + "; isLongPressStart: " + musicData.isLongPressStart);
            MiscToolsForMD.Log("Note data info: id: " + musicData.noteData.id + "; type: " + musicData.noteData.type + "; damage: " + musicData.noteData.damage +
                "; pathway: " + musicData.noteData.pathway + "; speed: " + musicData.noteData.speed + "; score: " + musicData.noteData.score + "; missCombo: " +
                musicData.noteData.missCombo + "; addCombo: " + musicData.noteData.addCombo + "; jumpNote: " + musicData.noteData.jumpNote + "; isShowPlayEffect: " +
                musicData.noteData.isShowPlayEffect);
            if (!musicData.isLongPressing && !isMulStart)
            {
                if (musicData.noteData.addCombo)
                {
                    if (musicData.isDouble)
                    {
                        // Double-Press Notes
                        byte doubleResult = Singleton<BattleEnemyManager>.instance.GetPlayResult(musicData.doubleIdx);
                        if (doubleResult != (byte)TaskResult.None)
                        {
                            // If is TaskResult.None, maybe this note is Double-Start or haven't be recorded by Game.
                            // The latter situation will result unavoidable deviation with game result because game will
                            // check result after player finished stage, it will have all the results. Maybe someone
                            // can help me fix this. The BPM is higher and the game is laggier, the problem is severer.
                            targetWeight += 4;
                            if (result == (int)TaskResult.Great && doubleResult == (int)TaskResult.Prefect)
                            {
                                actualWeight += 2;
                            }
                            else if (result == (int)TaskResult.Prefect && doubleResult == (int)TaskResult.Great)
                            {
                                actualWeight += 2;
                            }
                            else if (result == (int)TaskResult.Great && doubleResult == (int)TaskResult.Great)
                            {
                                actualWeight += 2;
                            }
                            else if (result == (int)TaskResult.Prefect && doubleResult == (int)TaskResult.Prefect)
                            {
                                actualWeight += 4;
                            }
                        }
                    }
                    else
                    {
                        // Normal Notes
                        targetWeight += 2;
                        if (result == (int)TaskResult.Prefect)
                        {
                            actualWeight += 2;
                        }
                        else if (result == (int)TaskResult.Great)
                        {
                            actualWeight += 1;
                        }
                        MiscToolsForMD.Log("Normal Note");
                    }
                }
                else
                {
                    if (musicData.noteData.type == (uint)NoteType.Hp)
                    {
                        // Normal hearts
                        targetWeight += 1;
                        if (result == (int)TaskResult.Prefect)
                        {
                            actualWeight += 1;
                        }
                        MiscToolsForMD.Log("Heart Note");
                    }
                    else if (musicData.noteData.type == (uint)NoteType.Music)
                    {
                        // Musics
                        targetWeight += 1;
                        if (result == (int)TaskResult.Prefect)
                        {
                            actualWeight += 1;
                        }
                        MiscToolsForMD.Log("Music Note");
                    }
                    else
                    {
                        // Gears
                        targetWeight += 2;
                        if (result == (int)TaskResult.Prefect)
                        {
                            actualWeight += 2;
                        }
                        MiscToolsForMD.Log("Gear Note");
                    }
                }
                if (targetWeight > 0)
                {
                    float unit = 0.0001f;
                    float trueAcc = actualWeight * 1.0f / targetWeight;
                    float acc = Mathf.RoundToInt(trueAcc / unit) * unit;
                    // See Assets.Scripts.GameCore.HostComponent.TaskStageTarget.GetAccuracy
                    if (trueAcc < acc && (acc == 0.6f || acc == 0.7f || acc == 0.8f || acc == 0.9f || acc == 1.0f))
                    {
                        acc -= unit;
                    }
                    MiscToolsForMD.Log("Result: " + result + "; current accuracy: " + acc + "; current weight: " + actualWeight + "; target weight: " + targetWeight);
                    accuracyText = "Accuracy: " + acc.ToString("P");
                }
            }
        }

        public void LyricWindow(int windowId)
        {
            GUILayout.BeginVertical();
            GUIStyle lyricStyle = new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 48
            };
            GUILayout.Label(lyricContent, lyricStyle);
            GUILayout.EndVertical();
        }

        private void AddKeyCount(string actKey, uint num = 1)
        {
            foreach (string workingKey in workingKeys)
            {
                if (workingKey == actKey)
                {
                    uint count = counters[workingKey];
                    counters[workingKey] += num;
                    MiscToolsForMD.Log("Key " + actKey + " original count:" + count + " added " + num);
                }
            }
        }

        private KeyCode GetKeyCodeByName(string name)
        {
            foreach (KeyCode code in Enum.GetValues(typeof(KeyCode)))
            {
                if (code.ToString() == name)
                {
                    return code;
                }
            }
            return KeyCode.None;
        }
    }
}
