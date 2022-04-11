using Assets.Scripts.Database;
using Assets.Scripts.GameCore.HostComponent;
using Assets.Scripts.PeroTools.Commons;
using Assets.Scripts.PeroTools.Managers;
using FormulaBase;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MiscToolsForMD
{
    public class Indicator : MonoBehaviour
    {
        private readonly Dictionary<string, string> keyDisplayNames = Defines.keyDisplayNames;
        private readonly List<KeyInfo> keyInfos = new();
        private Rect windowRect = new(MiscToolsForMDMod.config.indicator.x, MiscToolsForMDMod.config.indicator.y, MiscToolsForMDMod.config.indicator.width, MiscToolsForMDMod.config.indicator.height);
        private Rect lyricWindowRect = new(MiscToolsForMDMod.config.lyric.x, MiscToolsForMDMod.config.lyric.y, MiscToolsForMDMod.config.lyric.width, MiscToolsForMDMod.config.lyric.height);
        private string accuracyText = "", lyricContent = "";
        private List<Lyric> lyrics;
        private GUIStyle accuracyStyle, labelStyle, lyricStyle;
        internal int actualWeight = 0;
        internal int targetWeight = 0;
        internal int actualWeightInGame = 0;
        internal int targetWeightInGame = 0;
        internal bool isMiss = false;
        internal Cache cache = new();
        internal Lang lang = Lang.GetLang();

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
            cache.CleanCache();
            if (MiscToolsForMDMod.config.indicator.ap.enabled || MiscToolsForMDMod.config.indicator.key.enabled)
            {
                if (MiscToolsForMDMod.config.indicator.x < 0)
                {
                    MiscToolsForMDMod.config.indicator.x = (Screen.width - MiscToolsForMDMod.config.indicator.width) / 2;
                }
                if (MiscToolsForMDMod.config.indicator.y < 0)
                {
                    MiscToolsForMDMod.config.indicator.y = 20;
                }
                accuracyStyle = new()
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 48
                };
                lyricStyle = new()
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 48
                };
                labelStyle = new()
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 24
                };
                accuracyStyle.normal.textColor = Defines.apColor;
                accuracyText = "Accuracy: " + 1.ToString("P");
                List<string> workingKeys = GetControlKeys();
                List<KeyCode> keyCodes = Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>().ToList();
                if (workingKeys.Count >= 3 && workingKeys.Count <= 9)
                {
                    int controlKeysNum = workingKeys.Count / 2;
                    keyInfos.Clear();
                    for (int i = 0; i < workingKeys.Count; i++)
                    {
                        KeyInfo keyInfo = new()
                        {
                            code = keyCodes.Find(keyCode => keyCode.ToString() == workingKeys[i]),
                            count = 0,
                            style = new()
                            {
                                alignment = TextAnchor.MiddleCenter,
                                fontSize = 24
                            }
                        };
                        if (i < controlKeysNum)
                        {
                            keyInfo.type = ControlType.Air;
                        }
                        else if (i >= (workingKeys.Count - controlKeysNum) && i < (workingKeys.Count))
                        {
                            keyInfo.type = ControlType.Ground;
                        }
                        else
                        {
                            keyInfo.type = ControlType.Fever;
                        }
                        keyInfo.style.normal.textColor = Defines.displayColor;
                        MiscToolsForMDMod.instance.Log("KeyInfo:" + keyInfo);
                        keyInfos.Add(keyInfo);
                        if (keyInfos.FindAll(keyInfo => keyInfo.type == ControlType.Fever).Count > 1)
                        {
                            MiscToolsForMDMod.instance.LoggerInstance.Warning("There seems to many Fever keys.");
                        }
                    }
                }
                else
                {
                    MiscToolsForMDMod.instance.LoggerInstance.Error("Unexcepted Keys List.");
                }
            }

            if (MiscToolsForMDMod.config.lyric.enabled)
            {
                if (MiscToolsForMDMod.config.lyric.x < 0)
                {
                    MiscToolsForMDMod.config.lyric.x = (Screen.width - MiscToolsForMDMod.config.lyric.width) / 2;
                }

                if (MiscToolsForMDMod.config.lyric.y < 0)
                {
                    MiscToolsForMDMod.config.lyric.y = Screen.height - MiscToolsForMDMod.config.lyric.height - 100;
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
            }
        }

        public void Update()
        {
            if (MiscToolsForMDMod.config.indicator.key.enabled)
            {
                for (int i = 0; i < keyInfos.Count; i++)
                {
                    KeyInfo keyInfo = keyInfos[i];
                    if (Input.GetKeyDown(keyInfo.code))
                    {
                        AddKeyCount(keyInfo);
                        SetKeyColor(keyInfo, Defines.pressingColor);
                    }
                    if (Input.GetKeyUp(keyInfo.code))
                    {
                        SetKeyColor(keyInfo, Defines.displayColor);
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
                accuracyText = string.Format(lang.localizedAccuracy, acc);
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

        private void IndicatorWindow(int windowId)
        {
            GUILayout.BeginVertical(null);
            if (MiscToolsForMDMod.config.indicator.ap.enabled)
            {
                GUILayout.Label(accuracyText, accuracyStyle, null);
                GUILayout.Space(20f);
            }
            GUILayout.FlexibleSpace();
            if (MiscToolsForMDMod.config.indicator.key.enabled)
            {
                GUILayout.BeginHorizontal(null);
                foreach (ControlType type in Enum.GetValues(typeof(ControlType)))
                {
                    List<KeyInfo> keyInfosByType = keyInfos.FindAll(keyInfo => keyInfo.type == type);
                    if (keyInfosByType.Count > 0)
                    {
                        GUILayout.BeginVertical(null);
                        GUILayout.Label(lang.localizedControlTypes[type], labelStyle, null);
                        GUILayout.Space(10f);
                        GUILayout.BeginHorizontal(null);
                        foreach (KeyInfo keyInfoByType in keyInfosByType)
                        {
                            string keyDisplayName = keyInfoByType.code.ToString();
                            if (keyDisplayNames.ContainsKey(keyDisplayName))
                            {
                                keyDisplayName = keyDisplayNames[keyDisplayName];
                            }
                            GUILayout.BeginVertical(null);
                            GUILayout.Label(keyDisplayName, keyInfoByType.style, null);
                            GUILayout.Space(10f);
                            GUILayout.Label(keyInfoByType.count.ToString(), keyInfoByType.style, null);
                            GUILayout.EndVertical();
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        private void LyricWindow(int windowId)
        {
            GUILayout.BeginVertical(null);
            GUILayout.Label(lyricContent, lyricStyle, null);
            GUILayout.EndVertical();
        }

        private void AddKeyCount(KeyInfo keyInfo, uint num = 1)
        {
            if (Singleton<StageBattleComponent>.instance.isInGame)
            {
                keyInfo.count += num;
            }
        }

        private void SetKeyColor(KeyInfo keyInfo, Color color)
        {
            if (Singleton<StageBattleComponent>.instance.isInGame)
            {
                keyInfo.style.normal.textColor = color;
            }
            else
            {
                keyInfo.style.normal.textColor = Defines.displayColor;
            }
        }

        private List<string> GetControlKeys()
        {
            List<string> keys = new();
            string text;
            // See Assets.Scripts.GameCore.Controller.StandloneController.GetDefaultKeyList
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
            if (!DataHelper.isAutoFever && keyConfig.FeverKey != "None")
            {
                keys.Insert(keys.Count / 2, keyConfig.FeverKey);
            }
            return keys;
        }
    }
}
