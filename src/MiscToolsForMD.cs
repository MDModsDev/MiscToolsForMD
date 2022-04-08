using Assets.Scripts.GameCore.GamePlay;
using Assets.Scripts.GameCore.HostComponent;
using Assets.Scripts.GameCore.Managers;
using Assets.Scripts.PeroTools.Commons;
using Assets.Scripts.PeroTools.Managers;
using FormulaBase;
using GameLogic;
using HarmonyLib;
using MelonLoader;
using Newtonsoft.Json;
using PeroPeroGames.GlobalDefines;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace MiscToolsForMD
{
    public class MiscToolsForMDMod : MelonMod
    {
        public List<ILyricSource> lyricSources = new();

        public static Config config;
        public static MiscToolsForMDMod instance;
        public static Indicator indicator;

        private bool skipSetPlayResult = false;
        private bool skipOnNoteResult = false;

        public override void OnApplicationLateStart()
        {
            if (System.IO.File.Exists(Defines.configPath))
            {
                try
                {
                    config = JsonConvert.DeserializeObject<Config>(System.IO.File.ReadAllText(Defines.configPath));
                }
                catch (Exception ex)
                {
                    LoggerInstance.Error("Failed to load config:" + ex.Message + ", we will create default one instead.");
                    config = new();
                    SaveConfig();
                }
            }
            else
            {
                config = new();
                SaveConfig();
            }
            config.debug = config.debug || MelonDebug.IsEnabled();
            LoggerInstance.Msg("Debug mode:" + config.debug);
            if (config.indicator.ap.enabled || config.indicator.key.enabled || config.lyric.enabled)
            {
                MethodInfo start = typeof(GameOptimization).GetMethod(nameof(GameOptimization.Init));
                MethodInfo startPatch = typeof(MiscToolsForMDMod).GetMethod(nameof(Init), BindingFlags.Static | BindingFlags.NonPublic);
                HarmonyInstance.Patch(start, null, new HarmonyMethod(startPatch));
            }
            else
            {
                LoggerInstance.Msg("Nothing was applied.");
            }
            if (config.indicator.ap.enabled)
            {
                MethodInfo setPlayResult = typeof(TaskStageTarget).GetMethod(nameof(TaskStageTarget.SetPlayResult));
                MethodInfo setPlayResultPatch = typeof(MiscToolsForMDMod).GetMethod(nameof(SetPlayResult), BindingFlags.Static | BindingFlags.NonPublic);
                HarmonyInstance.Patch(setPlayResult, null, new HarmonyMethod(setPlayResultPatch));
                MethodInfo onNoteResult = typeof(StatisticsManager).GetMethod(nameof(StatisticsManager.OnNoteResult));
                MethodInfo onNoteResultPatch = typeof(MiscToolsForMDMod).GetMethod(nameof(OnNoteResult), BindingFlags.Static | BindingFlags.NonPublic);
                HarmonyInstance.Patch(onNoteResult, null, new HarmonyMethod(onNoteResultPatch));
                MethodInfo addComboMiss = typeof(TaskStageTarget).GetMethod(nameof(TaskStageTarget.AddComboMiss));
                MethodInfo addComboMissPatch = typeof(MiscToolsForMDMod).GetMethod(nameof(AddComboMiss), BindingFlags.Static | BindingFlags.NonPublic);
                HarmonyInstance.Patch(addComboMiss, null, new HarmonyMethod(addComboMissPatch));
            }
            if (config.lyric.enabled)
            {
                lyricSources.Add(new LocalSource());
                // TODO: Load other lyric source
                lyricSources.OrderBy(lyricSource => lyricSource.Priority);
            }
            instance = this;
            LoggerInstance.Msg("MiscToolsForMD Loads Completed.");
        }

        private static void SetPlayResult(int idx, uint result, bool isMulEnd)
        {
            // See Assets.Scripts.GameCore.HostComponent.TaskStageTarget.GetTrueAccuracyNew
            // and Assets.Scripts.GameCore.HostComponent.TaskStageTarget.SetPlayResult
            // AddComboMiss -> SetPlayResult -> OnNoteResult
            if (instance.skipSetPlayResult)
            {
                instance.skipSetPlayResult = false;
                return;
            }
            instance.Log("idx:" + idx + ";result:" + result + ";isMulEnd:" + isMulEnd);
            if ((result <= (uint)TaskResult.None) || indicator.cache.IsIdRecorded(idx))
            {
                return;
            }
            MusicData musicData = Singleton<StageBattleComponent>.instance.GetMusicDataByIdx(idx);
            TaskResult playResult = (TaskResult)Singleton<BattleEnemyManager>.instance.GetPlayResult(idx);
            if (musicData.isLongPressing)
            {
                return;
            }
            if (!musicData.noteData.addCombo)
            {
                indicator.targetWeight += 2;
                if (result == (uint)TaskResult.Prefect)
                {
                    indicator.actualWeight += 2;
                }
                instance.Log("Note which doesn't add combo captured.");
            }
            else if (musicData.isLongPressEnd || musicData.isLongPressStart)
            {
                indicator.targetWeight += 2;
                switch (result)
                {
                    case (uint)TaskResult.Prefect:
                        indicator.actualWeight += 2;
                        break;

                    case (uint)TaskResult.Great:
                        indicator.actualWeight += 1;
                        break;

                    default:
                        instance.Log("A LongPressStart/End note's result is TaskResult.None/TaskResult.Miss");
                        break;
                }
                instance.Log("LongPressStart/End captured.");
            }
            else if (playResult == TaskResult.None || isMulEnd || musicData.doubleIdx > 0)
            {
                if (musicData.isDouble)
                {
                    byte playResult2 = Singleton<BattleEnemyManager>.instance.GetPlayResult(musicData.doubleIdx);
                    if (playResult2 == (byte)TaskResult.None)
                    {
                        instance.Log("Current is first note of a double-press group.");
                    }
                    else
                    {
                        indicator.targetWeight += 4;
                        if ((playResult2 == (byte)TaskResult.Prefect && result == (uint)TaskResult.Great) || (playResult2 == (byte)TaskResult.Great && result == (uint)TaskResult.Prefect) || (playResult2 == (byte)TaskResult.Great && result == (uint)TaskResult.Great))
                        {
                            indicator.actualWeight += 2;
                        }
                        else if (result == (uint)TaskResult.Prefect && playResult2 == (byte)TaskResult.Prefect)
                        {
                            indicator.actualWeight += 4;
                        }
                        instance.Log("Current is second note of a double-press group.");
                    }
                    instance.Log("Double-Press captured.");
                }
                else
                {
                    indicator.targetWeight += 2;
                    switch (result)
                    {
                        case (uint)TaskResult.Prefect:
                            indicator.actualWeight += 2;
                            break;

                        case (uint)TaskResult.Great:
                            indicator.actualWeight += 1;
                            break;

                        case (uint)TaskResult.Miss:
                            if (musicData.noteData.type == (uint)NoteType.Hide)
                            {
                                indicator.isMiss = true;
                                instance.Log("A ghost note is missed");
                            }
                            else
                            {
                                instance.Log("A normal note is missed");
                            }
                            break;

                        default:
                            instance.Log("A normal note's result is TaskResult.None");
                            break;
                    }
                    instance.Log("Normal note captured.");
                }
            }
            indicator.actualWeightInGame = GetCurrentActualWeightInGame();
            indicator.targetWeightInGame = GetCurrentTargetWeightInGame(idx);
            indicator.UpdateAccuracy();
            indicator.cache.AddRecordedId(idx);
            instance.Log("targetWeight:" + indicator.targetWeight + ";actualWeight:" + indicator.actualWeight);
            instance.Log("targetWeightInGame:" + indicator.targetWeightInGame + ";actualWeightInGame:" + indicator.actualWeightInGame);
            instance.skipOnNoteResult = true;
        }

        private static void OnNoteResult(int result)
        {
            if (instance.skipOnNoteResult)
            {
                instance.skipOnNoteResult = false;
                return;
            }
            instance.Log("result:" + result);
            if (result == (int)TaskResult.None)
            {
                indicator.isMiss = true;
                indicator.targetWeight += 2;
                indicator.UpdateAccuracy();
                instance.Log("Missing Heart/Music");
            }
        }

        private static void AddComboMiss(int value)
        {
            instance.Log("value:" + value);
            if (value == 1)
            {
                indicator.isMiss = true;
                indicator.targetWeight += 2;
                indicator.UpdateAccuracy();
                instance.skipSetPlayResult = true;
                instance.Log("Missing normal note.");
            }
        }

        private static void Init()
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

        private static int GetCurrentTargetWeightInGame(int idx)
        {
            // See Assets.Scripts.GameCore.HostComponent.TaskStageTarget.GetTrueAccuracyNew
            List<MusicData> validMusicDatas = indicator.cache.GetAllMusicDatasBeforeId(idx);
            int touchNum = validMusicDatas.Count(musicData => musicData.noteData.type == (uint)NoteType.Hp || musicData.noteData.type == (uint)NoteType.Music);
            int normalNum = validMusicDatas.Count(musicData => musicData.noteData.addCombo && !musicData.isLongPressing);
            int blockNum = validMusicDatas.Count(musicData => musicData.noteData.type == (uint)NoteType.Block);
            return (touchNum + normalNum + blockNum) * 2;
        }

        private static int GetCurrentActualWeightInGame()
        {
            // See Assets.Scripts.GameCore.HostComponent.TaskStageTarget.GetTrueAccuracyNew
            TaskStageTarget targetInstance = Singleton<TaskStageTarget>.instance;
            int actualTouchNum = targetInstance.GetCountValue(TaskCount.Music) + targetInstance.GetCountValue(TaskCount.Energy) + targetInstance.GetCountValue(TaskCount.TouhouRedPoint);
            int actualPerfectNum = targetInstance.GetHitCountByResult(TaskResult.Prefect);
            int actualGreatNum = targetInstance.GetHitCountByResult(TaskResult.Great);
            int actualBlockNum = targetInstance.GetCountValue(TaskCount.Block);
            return actualTouchNum * 2 + actualPerfectNum * 2 + actualGreatNum + actualBlockNum * 2;
        }

        public static List<string> GetControlKeys()
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
            keys.Insert(keys.Count / 2, keyConfig.FeverKey);
            return keys;
        }

        public void Log(object log, object normalLog = null)
        {
            StackTrace trace = new();
            string callerName = "[" + trace.GetFrame(1).GetMethod().Name + "] ";
            if (config.debug)
            {
                LoggerInstance.Msg(callerName + log);
            }
            else if (normalLog != null)
            {
                LoggerInstance.Msg(callerName + normalLog);
            }
        }

        public void SaveConfig()
        {
            System.IO.File.WriteAllText(Defines.configPath, JsonConvert.SerializeObject(config, Formatting.Indented));
        }
    }
}
