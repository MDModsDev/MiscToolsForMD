using Assets.Scripts.GameCore.GamePlay;
using Assets.Scripts.GameCore.HostComponent;
using Assets.Scripts.PeroTools.Commons;
using Assets.Scripts.PeroTools.Managers;
using FormulaBase;
using GameLogic;
using HarmonyLib;
using MelonLoader;
using Newtonsoft.Json;
using PeroPeroGames.GlobalDefines;
using System.Collections.Generic;
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
        private bool canFix = true;

        public override void OnApplicationLateStart()
        {
            if (System.IO.File.Exists(Defines.configPath))
            {
                config = JsonConvert.DeserializeObject<Config>(System.IO.File.ReadAllText(Defines.configPath));
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
                MethodInfo addComboMiss = typeof(TaskStageTarget).GetMethod("AddComboMiss");
                MethodInfo addComboMissPatch = typeof(MiscToolsForMDMod).GetMethod(nameof(AddComboMiss), BindingFlags.Static | BindingFlags.NonPublic);
                HarmonyInstance.Patch(addComboMiss, null, new HarmonyMethod(addComboMissPatch));
                MethodInfo setPlayResult = typeof(TaskStageTarget).GetMethod("SetPlayResult");
                MethodInfo setPlayResultPatch = typeof(MiscToolsForMDMod).GetMethod(nameof(SetPlayResult), BindingFlags.Static | BindingFlags.NonPublic);
                HarmonyInstance.Patch(setPlayResult, null, new HarmonyMethod(setPlayResultPatch));
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

        private static void SetPlayResult(int idx, uint result, bool isMulEnd)
        {
            // See Assets.Scripts.GameCore.HostComponent.TaskStageTarget.GetTrueAccuracyNew
            // and Assets.Scripts.GameCore.HostComponent.TaskStageTarget.SetPlayResult
            if (result <= 0U)
            {
                return;
            }
            MusicData musicData = Singleton<StageBattleComponent>.instance.GetMusicDataByIdx(idx);
            TaskResult playResult = (TaskResult)Singleton<BattleEnemyManager>.instance.GetPlayResult(idx);
            if (musicData.isLongPressing || !musicData.noteData.addCombo)
            {
                return;
            }
            instance.canFix = false;
            if (musicData.isLongPressEnd || musicData.isLongPressStart)
            {
                instance.Log("LongPressStart/End captured.");
                indicator.targetWeight += 2;
                if (result == (uint)TaskResult.Prefect)
                {
                    indicator.actualWeight += 2;
                }
                else if (result == (uint)TaskResult.Great)
                {
                    indicator.actualWeight += 1;
                }
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
                    if (result == (uint)TaskResult.Prefect)
                    {
                        indicator.actualWeight += 2;
                    }
                    else if (result == (uint)TaskResult.Great)
                    {
                        indicator.actualWeight += 1;
                    }
                    instance.Log("Normal note captured.");
                }
            }
            indicator.UpdateAccuracy();
            instance.canFix = true;
            instance.Log("idx:" + idx + ";result:" + result + ";isMulEnd:" + isMulEnd + ";targetWeight:" + indicator.targetWeight + ";actualWeight:" + indicator.actualWeight);
        }

        private static void AddComboMiss(int value)
        {
            if (instance.canFix)
            {
                indicator.targetWeight += 2 * value;
                indicator.isMiss = true;
                indicator.UpdateAccuracy();
                instance.Log("Player misses note without contact was captured.");
            }
            instance.Log("value:" + value + ";targetWeight:" + indicator.targetWeight + ";actualWeight:" + indicator.actualWeight);
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

        private static int GetCurrentTargetWeightInGame(int idx)
        {
            // See Assets.Scripts.GameCore.HostComponent.TaskStageTarget.GetTrueAccuracyNew
            Il2CppSystem.Collections.Generic.List<MusicData> musicDatas = Singleton<StageBattleComponent>.instance.GetMusicData();
            List<MusicData> validMusicDatas = new();
            int num;
            if (idx < 0 || idx >= musicDatas.Count)
            {
                num = musicDatas.Count - 1;
            }
            else
            {
                num = idx;
            }
            for (int i = 0; i <= num; i++)
            {
                validMusicDatas.Add(musicDatas[i]);
            }
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
            System.IO.File.WriteAllText(Defines.configPath, JsonConvert.SerializeObject(config, Formatting.Indented));
        }
    }
}
