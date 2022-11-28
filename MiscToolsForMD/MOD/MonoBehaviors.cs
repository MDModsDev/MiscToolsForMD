using Assets.Scripts.Database;
using Assets.Scripts.PeroTools.Commons;
using FormulaBase;
using MiscToolsForMD.SDK;
using MiscToolsForMD.Lyric;
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
        private readonly Dictionary<string, string> keyDisplayNames = SDK.PublicDefines.keyDisplayNames;
        private readonly List<KeyInfo> keyInfos = new List<KeyInfo>();
        private string accuracyText = "", lyricContent = "";
        private List<Lyric.Lyric> lyrics;
        private Rect windowRect, lyricWindowRect;
        private GUIStyle accuracyStyle, labelStyle, lyricStyle;
        private Color32 apColor, displayColor, pressingColor, missColor, greatColor;
        internal Lang lang = Lang.GetLang();
        private readonly bool keyEnabled = 
            MiscToolsForMDMod.instance.GetPreferenceValue<bool>(InternalDefines.PreferenceNames.IndicatorCategory.keyEnabled);
        private readonly bool lyricEnabled =
            MiscToolsForMDMod.instance.GetPreferenceValue<bool>(InternalDefines.PreferenceNames.LyricCategory.enabled);
        private readonly bool apEnabled =
            MiscToolsForMDMod.instance.GetPreferenceValue<bool>(InternalDefines.PreferenceNames.IndicatorCategory.apEnabled);


        public Indicator(IntPtr intPtr) : base(intPtr)
        {
        }

        public Indicator() : base(ClassInjector.DerivedConstructorPointer<Indicator>()) => ClassInjector.DerivedConstructorBody(this);

        public void Start()
        {
            if (apEnabled || keyEnabled)
            {
                if (MiscToolsForMDMod.instance.GetPreferenceValue<Vector2>(InternalDefines.PreferenceNames.IndicatorCategory.coordinate).x < 0)
                {
                    Vector2 indicatorCoordinate = MiscToolsForMDMod.instance.GetPreferenceValue<Vector2>(InternalDefines.PreferenceNames.IndicatorCategory.coordinate);
                    indicatorCoordinate.x =
                        (Screen.width - MiscToolsForMDMod.instance.GetPreferenceValue<Vector2>(InternalDefines.PreferenceNames.IndicatorCategory.size).x) / 2;
                    MiscToolsForMDMod.instance.UpdatePreferenceValue(InternalDefines.PreferenceNames.IndicatorCategory.coordinate, indicatorCoordinate);
                }
                if (MiscToolsForMDMod.instance.GetPreferenceValue<Vector2>(InternalDefines.PreferenceNames.IndicatorCategory.coordinate).y < 0)
                {
                    Vector2 indicatorCoordinate = MiscToolsForMDMod.instance.GetPreferenceValue<Vector2>(InternalDefines.PreferenceNames.IndicatorCategory.coordinate);
                    indicatorCoordinate.y = 20;
                    MiscToolsForMDMod.instance.UpdatePreferenceValue(InternalDefines.PreferenceNames.IndicatorCategory.coordinate, indicatorCoordinate);
                }
                windowRect = new Rect()
                {
                    x = MiscToolsForMDMod.instance.GetPreferenceValue<Vector2>(InternalDefines.PreferenceNames.IndicatorCategory.coordinate).x,
                    y = MiscToolsForMDMod.instance.GetPreferenceValue<Vector2>(InternalDefines.PreferenceNames.IndicatorCategory.coordinate).y,
                    width = MiscToolsForMDMod.instance.GetPreferenceValue<Vector2>(InternalDefines.PreferenceNames.IndicatorCategory.size).x,
                    height = MiscToolsForMDMod.instance.GetPreferenceValue<Vector2>(InternalDefines.PreferenceNames.IndicatorCategory.size).y,
                };
                PrepareColors();
                PrepareStyles();
                accuracyStyle.normal.textColor = apColor;
                accuracyText = string.Format("{0:P}", 1);
                PrepareKeys();
            }

            if (lyricEnabled)
            {
                PrepareLyrics();
            }
            if (MiscToolsForMDMod.instance.GetPreferenceValue<bool>(InternalDefines.PreferenceNames.MainCategory.debug))
            {
                MusicDisplayInfo musicDisplayInfo = MiscTools.realtimeGameStatics.GetMusicDisplayInfo();
                MiscToolsForMDMod.instance.Log(string.Format("Song name:{0};author:{1}", musicDisplayInfo.musicName, musicDisplayInfo.authorName));
                string musicDatasJsonPath = Path.Combine(SDK.PublicDefines.basePath, "MusicDatas");
                string musicDatasJsonFile = Path.Combine(musicDatasJsonPath, string.Format("{0}-{1}-{2}.json", musicDisplayInfo.musicName, musicDisplayInfo.authorName, DataHelper.selectedMusicLevel));
                MiscTools.realtimeGameStatics.ExportMusicDatasTo(musicDatasJsonFile);
                MiscToolsForMDMod.instance.Log("Exported MusicDatas to " + musicDatasJsonFile);
            }
        }

        public void Update()
        {
            if (keyEnabled)
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
            if (lyricEnabled)
            {
                float time = Singleton<StageBattleComponent>.instance.timeFromMusicStart;
                lyricContent = Lyric.Lyric.GetLyricByTime(lyrics, time).content;
            }
            UpdateAccuracy();
        }

        public void OnGUI()
        {
            if (apEnabled || keyEnabled)
            {
                windowRect = GUILayout.Window(InternalDefines.windowRectId, windowRect, (GUI.WindowFunction)IndicatorWindow, "MiscToolsUI", null);
            }
            if (lyricEnabled)
            {
                lyricWindowRect = GUILayout.Window(InternalDefines.lyricWindowId, lyricWindowRect, (GUI.WindowFunction)LyricWindow, "Lyric", null);
            }
        }

        public void OnDestroy()
        {
            MiscToolsForMDMod.indicator = null;
        }

        private void PrepareStyles()
        {
            accuracyStyle = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = MiscToolsForMDMod.instance.GetPreferenceValue<int>(InternalDefines.PreferenceNames.IndicatorCategory.apSize)
            };
            lyricStyle = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = MiscToolsForMDMod.instance.GetPreferenceValue<int>(InternalDefines.PreferenceNames.LyricCategory.fontSize)
            };
            labelStyle = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = MiscToolsForMDMod.instance.GetPreferenceValue<int>(InternalDefines.PreferenceNames.MainCategory.fontSize)
            };
        }

        private void PrepareKeys()
        {
            List<string> workingKeys = MiscTools.realtimeGameStatics.GetControlKeys();
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
                            fontSize = MiscToolsForMDMod.instance.GetPreferenceValue<int>(InternalDefines.PreferenceNames.IndicatorCategory.keySize)
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

        private void PrepareLyrics()
        {
            MusicDisplayInfo musicDisplayInfo = MiscTools.realtimeGameStatics.GetMusicDisplayInfo();
            MiscToolsForMDMod.instance.Log(string.Format("Song name:{0};author:{1}", musicDisplayInfo.musicName, musicDisplayInfo.authorName));
            if (MiscToolsForMDMod.instance.GetPreferenceValue<Vector2>(InternalDefines.PreferenceNames.LyricCategory.coordinate).x < 0)
            {
                Vector2 lyricCoordinate = MiscToolsForMDMod.instance.GetPreferenceValue<Vector2>(InternalDefines.PreferenceNames.LyricCategory.coordinate);
                lyricCoordinate.x =
                    (MiscToolsForMDMod.instance.GetPreferenceValue<Vector2>(InternalDefines.PreferenceNames.LyricCategory.size).x) / 2;
                MiscToolsForMDMod.instance.UpdatePreferenceValue(InternalDefines.PreferenceNames.LyricCategory.coordinate, lyricCoordinate);
            }

            if (MiscToolsForMDMod.instance.GetPreferenceValue<Vector2>(InternalDefines.PreferenceNames.LyricCategory.coordinate).y < 0)
            {
                Vector2 lyricCoordinate = MiscToolsForMDMod.instance.GetPreferenceValue<Vector2>(InternalDefines.PreferenceNames.LyricCategory.coordinate);
                lyricCoordinate.y =
                    Screen.height - MiscToolsForMDMod.instance.GetPreferenceValue<Vector2>(InternalDefines.PreferenceNames.LyricCategory.size).y - 100;
                MiscToolsForMDMod.instance.UpdatePreferenceValue(InternalDefines.PreferenceNames.LyricCategory.coordinate, lyricCoordinate);
            }
            lyricWindowRect = new Rect()
            {
                x = MiscToolsForMDMod.instance.GetPreferenceValue<Vector2>(InternalDefines.PreferenceNames.LyricCategory.coordinate).x,
                y = MiscToolsForMDMod.instance.GetPreferenceValue<Vector2>(InternalDefines.PreferenceNames.LyricCategory.coordinate).y,
                height = MiscToolsForMDMod.instance.GetPreferenceValue<Vector2>(InternalDefines.PreferenceNames.LyricCategory.size).x,
                width = MiscToolsForMDMod.instance.GetPreferenceValue<Vector2>(InternalDefines.PreferenceNames.LyricCategory.size).y,
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
                    MiscToolsForMDMod.instance.LoggerInstance.Error(ex.ToString(), "Failed to get lyric through source " + source.name);
                }
            }
            if (!successGetLyric || lyrics.Count == 0)
            {
                MiscToolsForMDMod.instance.UpdatePreferenceValue(InternalDefines.PreferenceNames.LyricCategory.enabled, false);
                MiscToolsForMDMod.instance.LoggerInstance.Error("No available lyric. We will disable lyric displaying.");
            }
        }

        private void PrepareColors()
        {
            apColor = MiscToolsForMDMod.instance.GetPreferenceValue<Color>(InternalDefines.PreferenceNames.IndicatorCategory.apColor);
            greatColor = MiscToolsForMDMod.instance.GetPreferenceValue<Color>(InternalDefines.PreferenceNames.IndicatorCategory.greatColor);
            missColor = MiscToolsForMDMod.instance.GetPreferenceValue<Color>(InternalDefines.PreferenceNames.IndicatorCategory.missColor);
            displayColor = MiscToolsForMDMod.instance.GetPreferenceValue<Color>(InternalDefines.PreferenceNames.IndicatorCategory.keyDisplay);
            pressingColor = MiscToolsForMDMod.instance.GetPreferenceValue<Color>(InternalDefines.PreferenceNames.IndicatorCategory.keyPressed);
        }

        private void UpdateAccuracy()
        {
            const float unit = 0.0001f;
            float trueAccBySelf = 1.0f;
            float trueAccInGame = 1.0f;
            int actualWeightInGame = MiscTools.realtimeGameStatics.GetCurrentActualWeightInGame();
            int targetWeightInGame = MiscTools.realtimeGameStatics.GetCurrentTargetWeightInGame();
            int actualWeight = MiscTools.realtimeGameStatics.GetCurrentActualWeightBySelf();
            int targetWeight = MiscTools.realtimeGameStatics.GetCurrentTargetWeightBySelf();
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
            float trueAcc = MiscToolsForMDMod.instance.GetPreferenceValue<bool>(InternalDefines.PreferenceNames.IndicatorCategory.apManual) ? trueAccBySelf : trueAccInGame;
            float acc = Mathf.RoundToInt(trueAcc / unit) * unit;
            // See Assets.Scripts.GameCore.HostComponent.TaskStageTarget.GetAccuracy
            if (trueAcc < acc && (acc == 0.6f || acc == 0.7f || acc == 0.8f || acc == 0.9f || acc == 1.0f))
            {
                acc -= unit;
            }
            accuracyText = string.Format("{0:P}", acc);
            if (acc < 1f && acc >= 0f)
            {
                if (MiscTools.realtimeGameStatics.IsStrictlyMissed())
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
            if (MiscToolsForMDMod.instance.GetPreferenceValue<bool>(InternalDefines.PreferenceNames.IndicatorCategory.apEnabled))
            {
                GUILayout.Label(accuracyText, accuracyStyle, null);
                GUILayout.Space(10f);
            }
            if (MiscToolsForMDMod.instance.GetPreferenceValue<bool>(InternalDefines.PreferenceNames.IndicatorCategory.keyEnabled))
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
