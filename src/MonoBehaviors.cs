using Assets.Scripts.Database;
using Assets.Scripts.GameCore.HostComponent;
using Assets.Scripts.PeroTools.Commons;
using Assets.Scripts.PeroTools.Managers;
using FormulaBase;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MiscToolsForMD
{
    public class Indicator : MonoBehaviour
    {
        private readonly Dictionary<string, string> keyDisplayNames = Defines.keyDisplayNames;
        private Rect windowRect = new(MiscToolsForMDMod.config.indicator.x, MiscToolsForMDMod.config.indicator.y, MiscToolsForMDMod.config.indicator.width, MiscToolsForMDMod.config.indicator.height);
        private string accuracyText, lyricContent;
        private List<string> workingKeys;
        private Dictionary<string, uint> counters;
        private Rect lyricWindowRect = new(MiscToolsForMDMod.config.lyric.x, MiscToolsForMDMod.config.lyric.y, MiscToolsForMDMod.config.lyric.width, MiscToolsForMDMod.config.lyric.height);

        private readonly GUIStyle accuracyStyle = new()
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 48
        };

        private readonly GUIStyle keyStyle = new()
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 24
        };

        public int actualWeight = 0;
        public int targetWeight = 0;
        public int actualWeightInGame = 0;
        public int targetWeightInGame = 0;
        public bool isMiss = false;
        internal Cache cache = new();
        private List<Lyric> lyrics;

        public Indicator(IntPtr intPtr) : base(intPtr)
        {
        }

        public void OnGUI()
        {
            if (MiscToolsForMDMod.config.indicator.ap.enabled || MiscToolsForMDMod.config.indicator.key.enabled)
            {
                windowRect = GUILayout.Window(0, windowRect, (GUI.WindowFunction)IndicatorWindow, "MiscToolsUI", null);
            }
            if (MiscToolsForMDMod.config.lyric.enabled)
            {
                lyricWindowRect = GUILayout.Window(1, lyricWindowRect, (GUI.WindowFunction)LyricWindow, "Lyric", null);
            }
        }

        public void Start()
        {
            bool needUpdateConfig = false;
            if (MiscToolsForMDMod.config.indicator.ap.enabled || MiscToolsForMDMod.config.indicator.key.enabled)
            {
                if (MiscToolsForMDMod.config.indicator.x < 0)
                {
                    MiscToolsForMDMod.config.indicator.x = (Screen.width - MiscToolsForMDMod.config.indicator.width) / 2;
                    needUpdateConfig = true;
                }
                if (MiscToolsForMDMod.config.indicator.y < 0)
                {
                    MiscToolsForMDMod.config.indicator.y = 20;
                    needUpdateConfig = true;
                }
                accuracyText = "Accuracy: " + 1.ToString("P");
                accuracyStyle.normal.textColor = Defines.apColor;
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
                windowRect = new Rect(MiscToolsForMDMod.config.indicator.x, MiscToolsForMDMod.config.indicator.y, MiscToolsForMDMod.config.indicator.width, MiscToolsForMDMod.config.indicator.height);
                actualWeight = 0;
                targetWeight = 0;
                actualWeightInGame = 0;
                targetWeightInGame = 0;
                isMiss = false;
                cache = new();
                cache.CleanCache();
            }
            if (MiscToolsForMDMod.config.lyric.enabled)
            {
                if (MiscToolsForMDMod.config.lyric.x < 0)
                {
                    MiscToolsForMDMod.config.lyric.x = (Screen.width - MiscToolsForMDMod.config.lyric.width) / 2;
                    needUpdateConfig = true;
                }
                if (MiscToolsForMDMod.config.lyric.y < 0)
                {
                    MiscToolsForMDMod.config.lyric.y = Screen.height - MiscToolsForMDMod.config.lyric.height - 100;
                    needUpdateConfig = true;
                }
                if (needUpdateConfig)
                {
                    MiscToolsForMDMod.instance.SaveConfig();
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
                    MiscToolsForMDMod.config.lyric.enabled = false;
                    MiscToolsForMDMod.instance.LoggerInstance.Error("No available lyric. We will disable lyric displaying.");
                }
                lyricWindowRect = new Rect(MiscToolsForMDMod.config.lyric.x, MiscToolsForMDMod.config.lyric.y, MiscToolsForMDMod.config.lyric.width, MiscToolsForMDMod.config.lyric.height);
                lyricContent = "";
            }
        }

        public void Update()
        {
            if (MiscToolsForMDMod.config.indicator.key.enabled)
            {
                foreach (string key in workingKeys)
                {
                    if (Input.GetKeyDown(GetKeyCodeByName(key)))
                    {
                        AddKeyCount(key);
                    }
                }
            }
            if (MiscToolsForMDMod.config.lyric.enabled)
            {
                float time = Singleton<StageBattleComponent>.instance.timeFromMusicStart;
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
            if (MiscToolsForMDMod.config.indicator.ap.enabled)
            {
                GUILayout.Label(accuracyText, accuracyStyle, null);
            }
            GUILayout.Space(Defines.indicatorSpacePixelSize);
            if (MiscToolsForMDMod.config.indicator.key.enabled)
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
                        GUILayout.Label(keyDisplayName + "\n\n" + counters[key], keyStyle, null);
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        public void LyricWindow(int windowId)
        {
            GUIStyle lyricStyle = new()
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
                float trueAccInGame = actualWeightInGame * 1.0f / targetWeightInGame;
                MiscToolsForMDMod.instance.Log("trueAcc:" + trueAcc + ";trueAccInGame:" + trueAccInGame);
                float acc = Mathf.RoundToInt(trueAcc / unit) * unit;
                // See Assets.Scripts.GameCore.HostComponent.TaskStageTarget.GetAccuracy
                if (trueAcc < acc && (acc == 0.6f || acc == 0.7f || acc == 0.8f || acc == 0.9f || acc == 1.0f))
                {
                    acc -= unit;
                }
                accuracyText = "Accuracy: " + acc.ToString("P");
                if (acc < 1f && acc >= 0f)
                {
                    if (isMiss || (Singleton<TaskStageTarget>.instance.GetMiss() > 0))
                    {
                        accuracyStyle.normal.textColor = Defines.missColor;
                    }
                    else
                    {
                        accuracyStyle.normal.textColor = Defines.greatColor;
                    }
                }
                else if (acc == 1f)
                {
                    accuracyStyle.normal.textColor = Defines.apColor;
                }
                else
                {
                    accuracyStyle.normal.textColor = Defines.errColor;
                }
            }
        }

        private void AddKeyCount(string actKey, uint num = 1)
        {
            if (Singleton<StageBattleComponent>.instance.isInGame)
            {
                foreach (string workingKey in workingKeys)
                {
                    if (workingKey == actKey)
                    {
                        counters[workingKey] += num;
                    }
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
