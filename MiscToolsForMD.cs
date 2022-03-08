using Assets.Scripts.Database;
using Assets.Scripts.GameCore.GamePlay;
using Assets.Scripts.GameCore.HostComponent;
using Assets.Scripts.PeroTools.Commons;
using Assets.Scripts.PeroTools.Managers;
using HarmonyLib;
using MelonLoader;
using Newtonsoft.Json;
using PeroPeroGames.GlobalDefines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.UI;

namespace MiscToolsForMD
{
    public class MiscToolsForMDMod : MelonMod
    {
        public List<ILyricSource> lyricSources = new List<ILyricSource>();
        public static Config config;
        public static MiscToolsForMDMod instance;
        public static Indicator indicator;
        private static bool isBattleScene;
        private static bool Set;
        public override void OnApplicationLateStart()
        {
            if (System.IO.File.Exists("UserData" + System.IO.Path.DirectorySeparatorChar + "MiscToolsForMD.json"))
            {
                config = JsonConvert.DeserializeObject<Config>(System.IO.File.ReadAllText("UserData" + System.IO.Path.DirectorySeparatorChar + "MiscToolsForMD.json"));
            }
            else
            {
                config = new Config();
                SaveConfig();
            }
            config.debug = config.debug || MelonDebug.IsEnabled();
            LoggerInstance.Msg("Debug mode:" + config.debug);
            if (config.ap_indicator || config.key_indicator || config.lyric)
            {
                MethodInfo start = typeof(GameOptimization).GetMethod("Init");
                MethodInfo startPatch = typeof(MiscToolsForMDMod).GetMethod(nameof(InitUI), BindingFlags.Static | BindingFlags.NonPublic);
                HarmonyInstance.Patch(start, null, new HarmonyMethod(startPatch));
            }
            else
            {
                LoggerInstance.Msg("Nothing was applied.");
            }
            if (config.ap_indicator)
            {
                MethodInfo addCount = typeof(TaskStageTarget).GetMethod("AddCount");
                MethodInfo addCountPatch = typeof(MiscToolsForMDMod).GetMethod(nameof(AddCount), BindingFlags.Static | BindingFlags.NonPublic);
                HarmonyInstance.Patch(addCount, null, new HarmonyMethod(addCountPatch));
            }
            if (config.lyric)
            {
                lyricSources.Add(new LocalSource());
                // TODO: Load other lyric source
                lyricSources.OrderBy(lyricSource => lyricSource.Priority);
            }
            instance = this;
            LoggerInstance.Msg("MiscToolsForMD Loads Completed.");
        }
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            base.OnSceneWasLoaded(buildIndex, sceneName);
            if (sceneName == "GameMain")
            {
                isBattleScene = true;
            }
            else
            {
                isBattleScene = false;
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (isBattleScene && Set)
            {
                SetGameobject();
            }
        }

        private static void SetGameobject()
        {
            Set = false;
            // create gameobject called FC and AP
            GameObject FC = new GameObject("FC Indicator");
            GameObject AP = new GameObject("AP Indicator");
            // create text component
            Text FCtext = FC.AddComponent<Text>();
            Text APtext = AP.AddComponent<Text>(); ;
            // set text
            FCtext.text = "FC";
            APtext.text = "AP";
            // set color
            FCtext.color = Color.white;
            APtext.color = Color.white;
            // set position
            FC.transform.position = new Vector3(0, 0, 0);
            AP.transform.position = new Vector3(0, 0, 0);
            // set font size
            FCtext.fontSize = 20;
            APtext.fontSize = 20;
            // set active
            FC.SetActive(true);
            AP.SetActive(true);
        }

        private static void AddCount(uint result,int value)
        {
            if (value == 1)
            {
                indicator.targetWeight += 2;
                if (result == (uint)TaskResult.Great)
                {
                    indicator.actualWeight += 1;
                }
                else if (result == (uint)TaskResult.Prefect)
                {
                    indicator.actualWeight += 2;
                }
            }
            else if (value == 2)
            {
                indicator.targetWeight += 4;
                if (result == (uint)TaskResult.Great)
                {
                    indicator.actualWeight += 2;
                }
                else if (result == (uint)TaskResult.Prefect)
                {
                    indicator.actualWeight += 4;
                }
            }
            indicator.UpdateAccuracy();
            instance.Log("result:" + result + ";value:" + value + ";targetWeight:" + indicator.targetWeight + ";actualWeight:" + indicator.actualWeight);
        }

        private static void InitUI()
        {
            ClassInjector.RegisterTypeInIl2Cpp<Indicator>();
            GameObject ui = GameObject.Find("MiscToolsUI");
            if (ui == null)
            {
                ui = new GameObject("MiscToolsUI");
                instance.Log("Creating new GameObject");
            }
            else
            {
                instance.Log("Using existing GameObject");
            }
            indicator = ui.AddComponent<Indicator>();
            instance.Log("Created UI");
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

        public void Log(object log, object normalLog = null)
        {
            if (config.debug)
            {
                LoggerInstance.Msg(log);
            }
            else if (normalLog != null)
            {
                LoggerInstance.Msg(normalLog);
            }
        }

        public void SaveConfig()
        {
            System.IO.File.WriteAllText("UserData" + System.IO.Path.DirectorySeparatorChar + "MiscToolsForMD.json", JsonConvert.SerializeObject(config, Formatting.Indented));
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
        private Rect windowRect = new Rect(MiscToolsForMDMod.config.x, MiscToolsForMDMod.config.y, MiscToolsForMDMod.config.width, MiscToolsForMDMod.config.height);
        private string accuracyText, lyricContent;
        private List<string> workingKeys;
        private Dictionary<string, uint> counters;
        private Rect lyricWindowRect = new Rect(MiscToolsForMDMod.config.lyric_x, MiscToolsForMDMod.config.lyric_y, MiscToolsForMDMod.config.lyric_width, MiscToolsForMDMod.config.lyric_height);
        
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

        public int actualWeight = 0;
        public int targetWeight = 0;
        private List<Lyric> lyrics;
        public Indicator(IntPtr intPtr) : base(intPtr) { }

        public void OnGUI()
        {
            if (MiscToolsForMDMod.config.ap_indicator || MiscToolsForMDMod.config.key_indicator)
            {
                windowRect = GUILayout.Window(0, windowRect, (GUI.WindowFunction)IndicatorWindow, "MiscToolsUI",null);
            }
            if (MiscToolsForMDMod.config.lyric)
            {
                lyricWindowRect = GUILayout.Window(1, lyricWindowRect, (GUI.WindowFunction)LyricWindow, "Lyric",null);
            }
        }

        public void Start()
        {
            bool needUpdateConfig = false;
            if (MiscToolsForMDMod.config.ap_indicator || MiscToolsForMDMod.config.key_indicator)
            {
                if (MiscToolsForMDMod.config.x == -1)
                {
                    MiscToolsForMDMod.config.x = (Screen.width - MiscToolsForMDMod.config.width) / 2;
                    needUpdateConfig = true;
                }
                if (MiscToolsForMDMod.config.y == -1)
                {
                    MiscToolsForMDMod.config.y = 20;
                    needUpdateConfig |= true;
                }
                accuracyText = "Accuracy: " + 1.ToString("P");
                workingKeys = MiscToolsForMDMod.GetControlKeys();
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
                    MiscToolsForMDMod.instance.LoggerInstance.Error("Unexcepted Keys List.");
                }
                windowRect = new Rect(MiscToolsForMDMod.config.x, MiscToolsForMDMod.config.y, MiscToolsForMDMod.config.width, MiscToolsForMDMod.config.height);
                actualWeight = 0;
                targetWeight = 0;
            }
            if (MiscToolsForMDMod.config.lyric)
            {
                if (MiscToolsForMDMod.config.lyric_x == -1)
                {
                    MiscToolsForMDMod.config.lyric_x = (Screen.width - MiscToolsForMDMod.config.lyric_width) / 2;
                    needUpdateConfig = true;
                }
                if (MiscToolsForMDMod.config.lyric_y == -1)
                {
                    MiscToolsForMDMod.config.lyric_y = Screen.height - MiscToolsForMDMod.config.lyric_height - 100;
                    needUpdateConfig = true;
                }
                // See SetSelectedMusicNameTxt
                string musicName, musicAuthor;
                if (DataHelper.selectedAlbumUid != "collection" || DataHelper.selectedMusicIndex < 0)
                {
                    musicName = Singleton<ConfigManager>.instance.GetConfigStringValue(DataHelper.selectedAlbumName, "uid", "name", DataHelper.selectedMusicUidFromInfoList);
                    musicAuthor = Singleton<ConfigManager>.instance.GetConfigStringValue(DataHelper.selectedAlbumName, "uid", "author", DataHelper.selectedMusicUidFromInfoList);
                }
                else if (DataHelper.collections.Count == 0 || DataHelper.collections.Count < DataHelper.selectedMusicIndex)
                {
                    musicName = "?????";
                    musicAuthor = "???";
                }
                else
                {
                    musicName = Singleton<ConfigManager>.instance.GetConfigStringValue(DataHelper.selectedAlbumName, "uid", "name", DataHelper.collections[DataHelper.selectedMusicIndex]);
                    musicAuthor = Singleton<ConfigManager>.instance.GetConfigStringValue(DataHelper.selectedAlbumName, "uid", "author", DataHelper.collections[DataHelper.selectedMusicIndex]);
                }

                MiscToolsForMDMod.instance.Log("Song name: " + musicName + "; author: " + musicAuthor);
                bool successGetLyric = false;
                foreach (ILyricSource source in MiscToolsForMDMod.instance.lyricSources)
                {
                    try
                    {
                        lyrics = source.GetLyrics(musicName, musicAuthor);
                        successGetLyric = true;
                        break;
                    }
                    catch (Exception ex)
                    {
                        MiscToolsForMDMod.instance.LoggerInstance.Error(ex.ToString(), "Failed to get lyric through source " + source.Name);
                    }
                }
                if (!successGetLyric || lyrics.Count == 0)
                {
                    MiscToolsForMDMod.instance.LoggerInstance.Error("No available lyric.");
                }
                lyricWindowRect = new Rect(MiscToolsForMDMod.config.lyric_x, MiscToolsForMDMod.config.lyric_y, MiscToolsForMDMod.config.lyric_width, MiscToolsForMDMod.config.lyric_height);
                lyricContent = "";
            }
            if (needUpdateConfig)
            {
                MiscToolsForMDMod.instance.SaveConfig();
            }
        }

        public void Update()
        {
            if (MiscToolsForMDMod.config.key_indicator)
            {
                foreach (string key in workingKeys)
                {
                    if (Input.GetKeyDown(GetKeyCodeByName(key)))
                    {
                        AddKeyCount(key);
                    }
                }
            }
            if (MiscToolsForMDMod.config.lyric)
            {
                float time = Singleton<FormulaBase.StageBattleComponent>.instance.timeFromMusicStart;
                lyricContent = Lyric.GetLyricByTime(lyrics, time).content;
            }
        }

        public void OnDestroy()
        {
            MiscToolsForMDMod.indicator = null;
        }

        public void IndicatorWindow(int windowID)
        {
            GUILayout.BeginVertical(null);
            if (MiscToolsForMDMod.config.ap_indicator)
            {
                GUIStyle accuracyStyle = new GUIStyle
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 48
                };
                GUILayout.Label(accuracyText, accuracyStyle, null);
            }
            if (MiscToolsForMDMod.config.key_indicator)
            {
                GUILayout.BeginHorizontal(null);
                foreach (string key in workingKeys)
                {
                    if (key != null)
                    {
                        string keyDisplayName;
                        if (keyDisplayNames.ContainsKey(key))
                        {
                            keyDisplayName = keyDisplayNames[key];
                        }
                        else
                        {
                            keyDisplayName = key;
                        }
                        GUIStyle keyStyle = new GUIStyle
                        {
                            alignment = TextAnchor.MiddleCenter,
                            fontSize = 24
                        };
                        GUILayout.Label(keyDisplayName + "\n\n" + counters[key], keyStyle, null);
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        public void LyricWindow(int windowId)
        {
            GUIStyle lyricStyle = new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 48
            };
            GUILayout.BeginVertical(null);
            GUILayout.Label(lyricContent, lyricStyle, null);
            GUILayout.EndVertical();
        }

        public void UpdateAccuracy()
        {
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
                accuracyText = "Accuracy: " + acc.ToString("P");
            }
        }
        private void AddKeyCount(string actKey, uint num = 1)
        {
            foreach (string workingKey in workingKeys)
            {
                if (workingKey == actKey)
                {
                    counters[workingKey] += num;
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
