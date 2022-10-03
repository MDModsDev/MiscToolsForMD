using Assets.Scripts.Database;
using Assets.Scripts.PeroTools.Commons;
using FormulaBase;
using MiscToolsForMD.SDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace MiscToolsForMD.MOD
{
    public class Indicator : MonoBehaviour
    {
        private readonly Dictionary<string, string> keyDisplayNames = PublicDefines.keyDisplayNames;
        private readonly List<KeyInfo> keyInfos = new List<KeyInfo>();
        private readonly GameStatisticsProvider statisticsProvider = MiscTools.realtimeGameStatics;
        private string accuracyText = "", lyricContent = "";
        private List<Lyric> lyrics;
        private Rect windowRect, lyricWindowRect;
        private GUIStyle accuracyStyle, labelStyle, lyricStyle;
        private Color32 apColor, displayColor, pressingColor, missColor, greatColor;
        internal Lang lang = Lang.GetLang();

        public Indicator(IntPtr intPtr) : base(intPtr)
        {
        }

        public Indicator() : base(ClassInjector.DerivedConstructorPointer<Indicator>()) => ClassInjector.DerivedConstructorBody(this);

        public void Start()
        {
            MusicDisplayInfo musicDisplayInfo = statisticsProvider.GetMusicDisplayInfo();
            MiscToolsForMDMod.instance.Log(string.Format("Song name:{0};author:{1}", musicDisplayInfo.musicName, musicDisplayInfo.authorName));
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
                windowRect = new Rect()
                {
                    x = MiscToolsForMDMod.config.indicator.x,
                    y = MiscToolsForMDMod.config.indicator.y,
                    width = MiscToolsForMDMod.config.indicator.width,
                    height = MiscToolsForMDMod.config.indicator.height,
                };
                if (!ColorUtility.DoTryParseHtmlColor(MiscToolsForMDMod.config.indicator.ap.ap, out apColor))
                {
                    apColor = new Color32()
                    {
                        r = (byte)(255 / 256f),
                        g = (byte)(215 / 256f),
                        b = (byte)(0 / 256f)
                    };
                    MiscToolsForMDMod.instance.Log("Failed to read apColor, use default instead");
                }
                if (!ColorUtility.DoTryParseHtmlColor(MiscToolsForMDMod.config.indicator.ap.great, out greatColor))
                {
                    greatColor = new Color32()
                    {
                        r = (byte)(65 / 256f),
                        g = (byte)(105 / 256f),
                        b = (byte)(225 / 256f)
                    };
                    MiscToolsForMDMod.instance.Log("Failed to read greatColor, use default instead");
                }
                if (!ColorUtility.DoTryParseHtmlColor(MiscToolsForMDMod.config.indicator.ap.miss, out missColor))
                {
                    missColor = Color.white;
                    MiscToolsForMDMod.instance.Log("Failed to read missColor, use default instead");
                }
                if (!ColorUtility.DoTryParseHtmlColor(MiscToolsForMDMod.config.indicator.key.display, out displayColor))
                {
                    displayColor = Color.black;
                    MiscToolsForMDMod.instance.Log("Failed to read displayColor, use default instead");
                }
                if (!ColorUtility.DoTryParseHtmlColor(MiscToolsForMDMod.config.indicator.key.pressing, out pressingColor))
                {
                    pressingColor = Color.white;
                    MiscToolsForMDMod.instance.Log("Failed to read pressingColor, use default instead");
                }
                accuracyStyle = new GUIStyle()
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = MiscToolsForMDMod.config.indicator.ap.size
                };
                lyricStyle = new GUIStyle()
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = MiscToolsForMDMod.config.lyric.size
                };
                labelStyle = new GUIStyle()
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = MiscToolsForMDMod.config.size
                };
                accuracyStyle.normal.textColor = apColor;
                accuracyText = string.Format("{0:P}", 1);
                List<string> workingKeys = statisticsProvider.GetControlKeys();
                List<KeyCode> keyCodes = Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>().ToList();
                if (workingKeys.Count >= 3 && workingKeys.Count <= 9)
                {
                    int controlKeysNum = workingKeys.Count / 2;
                    keyInfos.Clear();
                    for (int i = 0; i < workingKeys.Count; i++)
                    {
                        KeyInfo keyInfo = new KeyInfo()
                        {
                            code = keyCodes.Find(keyCode => keyCode.ToString() == workingKeys[i]),
                            count = 0,
                            displayColor = displayColor,
                            style = new GUIStyle()
                            {
                                alignment = TextAnchor.MiddleCenter,
                                fontSize = MiscToolsForMDMod.config.indicator.key.size
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
                        keyInfo.style.normal.textColor = displayColor;
                        MiscToolsForMDMod.instance.Log("KeyInfo:" + keyInfo);
                        keyInfos.Add(keyInfo);
                        if (keyInfos.FindAll(eachKeyInfo => eachKeyInfo.type == ControlType.Fever).Count > 1)
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
                lyricWindowRect = new Rect()
                {
                    x = MiscToolsForMDMod.config.lyric.x,
                    y = MiscToolsForMDMod.config.lyric.y,
                    height = MiscToolsForMDMod.config.lyric.height,
                    width = MiscToolsForMDMod.config.lyric.width,
                };
                bool successGetLyric = false;
                foreach (ILyricSource source in MiscToolsForMDMod.instance.lyricSources)
                {
                    try
                    {
                        lyrics = source.GetLyrics(musicDisplayInfo.musicName, musicDisplayInfo.authorName);
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
            if (MiscToolsForMDMod.config.debug)
            {
                string musicDatasJsonPath = Path.Combine(PublicDefines.basePath, "MusicDatas");
                string musicDatasJsonFile = Path.Combine(musicDatasJsonPath, string.Format("{0}-{1}-{2}.json", musicDisplayInfo.musicName, musicDisplayInfo.authorName, DataHelper.selectedMusicLevel));
                statisticsProvider.ExportMusicDatasTo(musicDatasJsonFile);
                MiscToolsForMDMod.instance.Log("Exported MusicDatas to " + musicDatasJsonFile);
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
                        keyInfo.AddCount();
                        keyInfo.SetColor(pressingColor);
                    }
                    if (Input.GetKeyUp(keyInfo.code))
                    {
                        keyInfo.ResetColor();
                    }
                }
            }
            if (MiscToolsForMDMod.config.lyric.enabled)
            {
                float time = Singleton<StageBattleComponent>.instance.timeFromMusicStart;
                lyricContent = Lyric.GetLyricByTime(lyrics, time).content;
            }
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

        public void OnDestroy()
        {
            MiscToolsForMDMod.indicator = null;
        }

        internal void UpdateAccuracy()
        {
            float unit = 0.0001f;
            float trueAccBySelf = 1.0f;
            float trueAccInGame = 1.0f;
            int actualWeightInGame = statisticsProvider.GetCurrentActualWeightInGame();
            int targetWeightInGame = statisticsProvider.GetCurrentTargetWeightInGame();
            int actualWeight = statisticsProvider.GetCurrentActualWeightBySelf();
            int targetWeight = statisticsProvider.GetCurrentTargetWeightBySelf();
            if ((targetWeight > 0) && (actualWeight <= targetWeight))
            {
                trueAccBySelf = actualWeight * 1.0f / targetWeight;
            }
            if ((targetWeightInGame > 0) && (actualWeightInGame <= targetWeightInGame))
            {
                trueAccInGame = actualWeightInGame * 1.0f / targetWeightInGame;
            }
            MiscToolsForMDMod.instance.Log(string.Format("targetWeight:{0};actualWeight:{1}", targetWeight, actualWeight));
            MiscToolsForMDMod.instance.Log(string.Format("targetWeightInGame:{0};actualWeightInGame:{1}", targetWeightInGame, actualWeightInGame));
            MiscToolsForMDMod.instance.Log(string.Format("trueAccBySelf:{0};trueAccInGame:{1}", trueAccBySelf, trueAccInGame));
            float trueAcc = MiscToolsForMDMod.config.indicator.ap.manual ? trueAccBySelf : trueAccInGame;
            float acc = Mathf.RoundToInt(trueAcc / unit) * unit;
            // See Assets.Scripts.GameCore.HostComponent.TaskStageTarget.GetAccuracy
            if (trueAcc < acc && (acc == 0.6f || acc == 0.7f || acc == 0.8f || acc == 0.9f || acc == 1.0f))
            {
                acc -= unit;
            }
            accuracyText = string.Format("{0:P}", acc);
            if (acc < 1f && acc >= 0f)
            {
                if (statisticsProvider.IsStrictlyMissed())
                {
                    accuracyStyle.normal.textColor = missColor;
                }
                else
                {
                    accuracyStyle.normal.textColor = greatColor;
                }
            }
            else
            {
                accuracyStyle.normal.textColor = apColor;
            }
        }

        private void IndicatorWindow(int windowId)
        {
            GUILayout.BeginVertical(null);
            if (MiscToolsForMDMod.config.indicator.ap.enabled)
            {
                GUILayout.Label(accuracyText, accuracyStyle, null);
                GUILayout.Space(10f);
            }
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
    }
}
