using Assets.Scripts.GameCore.GamePlay;
using Assets.Scripts.GameCore.HostComponent;
using Assets.Scripts.GameCore.Managers;
using Assets.Scripts.PeroTools.Commons;
using FormulaBase;
using GameLogic;
using HarmonyLib;
using MelonLoader;
using MiscToolsForMD.MOD;
using MiscToolsForMD.SDK;
using Newtonsoft.Json;
using PeroPeroGames.GlobalDefines;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace MiscToolsForMD
{
    public class MiscToolsForMDMod : MelonMod
    {
        public static MiscToolsForMDMod instance;
        public static Indicator indicator;
        internal static Config config;
        internal List<ILyricSource> lyricSources = new List<ILyricSource>();

        public override void OnApplicationLateStart()
        {
            SDK.SDK.InitSDK(HarmonyInstance, LoggerInstance);
            if (File.Exists(MOD.InternalDefines.configPath))
            {
                try
                {
                    config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(InternalDefines.configPath)); ;
                }
                catch (Exception ex)
                {
                    LoggerInstance.Error("Failed to load config:" + ex.Message + ", we will create default one instead.");
                    config = new Config();
                    SaveConfig();
                }
            }
            else if (File.Exists(Path.Combine("UserData", "MiscToolsForMD.json")))
            {
                try
                {
                    config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Path.Combine("UserData", "MiscToolsForMD.json")));
                    SaveConfig();
                    File.Delete(Path.Combine("UserData", "MiscToolsForMD.json"));
                    LoggerInstance.Msg("Migrating config successful!");
                }
                catch (Exception ex)
                {
                    LoggerInstance.Error("Failed to migrate config:" + ex.Message + ", we will create default one instead.");
                    config = new Config();
                    SaveConfig();
                }
            }
            else
            {
                LoggerInstance.Msg("Creating default config...");
                config = new Config();
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
                MethodInfo setPlayResult = typeof(BattleEnemyManager).GetMethod(nameof(BattleEnemyManager.SetPlayResult));
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
                lyricSources = lyricSources.OrderBy(lyricSource => lyricSource.Priority).ToList();
            }
            instance = this;
            LoggerInstance.Msg("MiscToolsForMD Loads Completed.");
        }

        public void Log(object log, object normalLog = null)
        {
            StackTrace trace = new StackTrace();
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

        private static void SetPlayResult(int idx, byte result, bool isMulStart, bool isMulEnd, bool isLeft)
        {
            // See Assets.Scripts.GameCore.HostComponent.TaskStageTarget.GetTrueAccuracyNew
            // and Assets.Scripts.GameCore.HostComponent.BattleEnemyManager.SetPlayResult
            // AddComboMiss -> SetPlayResult -> OnNoteResult (Not sure)
            TaskResult resultEasier = (TaskResult)result;
            instance.Log(string.Format("idx:{0};result:{1};isMulStart:{2};isMulEnd:{3};isLeft:{4}", idx, resultEasier, isMulStart, isMulEnd, isLeft));
            if (!isMulStart)
            {
                if (resultEasier <= TaskResult.None)
                {
                    return;
                }
                MusicData musicData = Singleton<StageBattleComponent>.instance.GetMusicDataByIdx(idx);
                TaskResult playResult = (TaskResult)Singleton<BattleEnemyManager>.instance.GetPlayResult(idx);
                if (musicData.isLongPressing)
                {
                    return;
                }
                instance.Log("Note type:" + (NoteType)musicData.noteData.type);
                if (!musicData.noteData.addCombo)
                {
                    instance.Log("Note which doesn't add combo captured.");
                }
                else if (musicData.isLongPressEnd || musicData.isLongPressStart)
                {
                    switch (resultEasier)
                    {
                        case TaskResult.Prefect:
                            break;

                        case TaskResult.Great:
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
                        TaskResult playResult2 = (TaskResult)Singleton<BattleEnemyManager>.instance.GetPlayResult(musicData.doubleIdx);
                        if (playResult2 == TaskResult.None)
                        {
                            instance.Log("Current is first note of a double-press group.");
                        }
                        else
                        {
                            instance.Log("Current is second note of a double-press group.");
                        }
                        instance.Log("Double-Press captured.");
                    }
                    else
                    {
                        switch (resultEasier)
                        {
                            case TaskResult.Prefect:
                                break;

                            case TaskResult.Great:
                                break;

                            case TaskResult.Miss:
                                if (musicData.noteData.type == (uint)NoteType.Hide)
                                {
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
                indicator.UpdateAccuracy();
            }
        }

        private static void OnNoteResult(int result)
        {
            instance.Log("result:" + result);
            if (result == (int)TaskResult.None)
            {
                indicator.UpdateAccuracy();
                instance.Log("Missing Heart/Music");
            }
        }

        private static void AddComboMiss(int value)
        {
            instance.Log("value:" + value);
            if (value == 1)
            {
                indicator.UpdateAccuracy();
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

        private void SaveConfig()
        {
            string jsonStr = JsonConvert.SerializeObject(config, Formatting.Indented);
            Directory.CreateDirectory(Path.GetDirectoryName(InternalDefines.configPath));
            File.WriteAllText(InternalDefines.configPath, jsonStr);
        }
    }
}
