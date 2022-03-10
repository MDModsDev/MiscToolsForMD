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
                MethodInfo addCount = typeof(TaskStageTarget).GetMethod("AddCount");
                MethodInfo addCountPatch = typeof(MiscToolsForMDMod).GetMethod(nameof(AddCount), BindingFlags.Static | BindingFlags.NonPublic);
                HarmonyInstance.Patch(addCount, null, new HarmonyMethod(addCountPatch));
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
            MusicData musicData = Singleton<StageBattleComponent>.instance.GetMusicDataByIdx(idx);
            if (musicData != null && !musicData.isLongPressing && !musicData.noteData.addCombo)
            {
                indicator.targetWeight += 1;
                if (result == (uint)TaskResult.Prefect)
                {
                    indicator.actualWeight += 1;
                }
                indicator.UpdateAccuracy();
            }
            instance.Log("idx:" + idx + ";result:" + result + ";isMulEnd:" + isMulEnd);
        }

        private static void AddComboMiss(int value)
        {
            if (instance.canFix)
            {
                indicator.targetWeight += 2 * value;
                indicator.isMiss = true;
                indicator.UpdateAccuracy();
            }
            instance.Log("value:" + value);
        }

        private static void AddCount(uint result, int value)
        {
            instance.canFix = false;
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
            instance.canFix = true;
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
