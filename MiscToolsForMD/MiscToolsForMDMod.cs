using Assets.Scripts.GameCore.GamePlay;
using HarmonyLib;
using MelonLoader;
using MiscToolsForMD.CompatibleLayer;
using MiscToolsForMD.Lyric;
using MiscToolsForMD.MOD;
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
        public static MiscToolsForMDMod instance;
        public static Indicator indicator;
        internal List<ILyricSource> lyricSources = new List<ILyricSource>();

        public override void OnInitializeMelon()
        {
            MelonPreferences_Category mainCategory = MelonPreferences.CreateCategory(InternalDefines.PreferenceNames.MainCategory.name, InternalDefines.PreferenceNames.MainCategory.name);
            mainCategory.CreateEntryIfNotExist(InternalDefines.PreferenceNames.MainCategory.debug, false, description: "Debug mode of the mod");
            mainCategory.CreateEntryIfNotExist(InternalDefines.PreferenceNames.MainCategory.fontSize, 24, description: "Font size");
            mainCategory.CreateEntryIfNotExist(InternalDefines.PreferenceNames.LyricCategory.enabled, false, description: "Enable lyric display");
            mainCategory.CreateEntryIfNotExist(InternalDefines.PreferenceNames.LyricCategory.fontSize, 36, description: "Font size of lyric panel");
            mainCategory.CreateEntryIfNotExist(InternalDefines.PreferenceNames.LyricCategory.coordinate, new Vector2(-1, -1), description: "The coordinate of lyric panel");
            mainCategory.CreateEntryIfNotExist(InternalDefines.PreferenceNames.LyricCategory.size, new Vector2(500, 100), description: "The size of lyric panel");
            mainCategory.CreateEntryIfNotExist(InternalDefines.PreferenceNames.IndicatorCategory.apEnabled, true, description: "If enable realtime AP indicator");
            mainCategory.CreateEntryIfNotExist(InternalDefines.PreferenceNames.IndicatorCategory.apManual, false, description: "If using realtime accuracy value calculated by mod");
            mainCategory.CreateEntryIfNotExist(InternalDefines.PreferenceNames.IndicatorCategory.apSize, 36, description: "Font size of accuracy text");
            mainCategory.CreateEntryIfNotExist(InternalDefines.PreferenceNames.IndicatorCategory.apColor, InternalDefines.defaultApColor, description: "Text color when player is AP");
            mainCategory.CreateEntryIfNotExist(InternalDefines.PreferenceNames.IndicatorCategory.greatColor, InternalDefines.defaultGreatColor, description: "Text color when player is Full Combo");
            mainCategory.CreateEntryIfNotExist(InternalDefines.PreferenceNames.IndicatorCategory.missColor, InternalDefines.defaultMissColor, description: "Text color when player is Missed");
            mainCategory.CreateEntryIfNotExist(InternalDefines.PreferenceNames.IndicatorCategory.keyEnabled, true, description: "If enable key indicator");
            mainCategory.CreateEntryIfNotExist(InternalDefines.PreferenceNames.IndicatorCategory.keySize, 36, description: "Font size of key text");
            mainCategory.CreateEntryIfNotExist(InternalDefines.PreferenceNames.IndicatorCategory.keyDisplay, InternalDefines.defaultKeyDisplayColor, description: "Font color of all key text");
            mainCategory.CreateEntryIfNotExist(InternalDefines.PreferenceNames.IndicatorCategory.keyPressed, InternalDefines.defaultKeyPressedColor, description: "Font color of pressed key text");
            mainCategory.CreateEntryIfNotExist(InternalDefines.PreferenceNames.IndicatorCategory.coordinate, new Vector2(-1, -1), description: "The coordinate of indicator panel");
            mainCategory.CreateEntryIfNotExist(InternalDefines.PreferenceNames.IndicatorCategory.size, new Vector2(500, 100), description: "The size of indicator panel");
            mainCategory.CreateEntryIfNotExist(InternalDefines.PreferenceNames.HardCoreCategory.enabled, false, description: "If enable HardCore mode");
            mainCategory.CreateEntryIfNotExist(InternalDefines.PreferenceNames.SoftCoreCategory.enabled, false, description: "If enable SoftCore mode");
        }

        public override void OnLateInitializeMelon()
        {
            CompatibleUtils.UpdatePreferences();
            MelonPreferences.SetEntryValue(InternalDefines.PreferenceNames.MainCategory.name, InternalDefines.PreferenceNames.MainCategory.debug,
                GetPreferenceValue<bool>(InternalDefines.PreferenceNames.MainCategory.debug) || MelonDebug.IsEnabled());
            LoggerInstance.Msg("Debug mode:" + GetPreferenceValue<bool>(InternalDefines.PreferenceNames.MainCategory.debug));
            if (GetPreferenceValue<bool>(InternalDefines.PreferenceNames.IndicatorCategory.apEnabled) ||
                GetPreferenceValue<bool>(InternalDefines.PreferenceNames.IndicatorCategory.keyEnabled) ||
                GetPreferenceValue<bool>(InternalDefines.PreferenceNames.LyricCategory.enabled))
            {
                MethodInfo start = typeof(GameOptimization).GetMethod(nameof(GameOptimization.Init));
                MethodInfo startPatch = typeof(MiscToolsForMDMod).GetMethod(nameof(Init), BindingFlags.Static | BindingFlags.NonPublic);
                HarmonyInstance.Patch(start, null, new HarmonyMethod(startPatch));
            }
            else
            {
                LoggerInstance.Msg("Nothing was applied.");
            }
            if (GetPreferenceValue<bool>(InternalDefines.PreferenceNames.LyricCategory.enabled))
            {
                lyricSources.Add(new LocalSource());
                // TODO: Load other lyric source
                lyricSources = lyricSources.OrderBy(lyricSource => lyricSource.priority).ToList();
            }
            instance = this;
            LoggerInstance.Msg("MiscToolsForMD Loads Completed.");
        }

        public void Log(object log, object normalLog = null)
        {
            StackTrace trace = new StackTrace();
            string callerName = "[" + trace.GetFrame(1).GetMethod().Name + "] ";
            if (GetPreferenceValue<bool>(InternalDefines.PreferenceNames.MainCategory.debug))
            {
                LoggerInstance.Msg(callerName + log);
            }
            else if (normalLog != null)
            {
                LoggerInstance.Msg(callerName + normalLog);
            }
        }

        public T GetPreferenceValue<T>(string identifier)
            where T : new()
        {
            return MelonPreferences.GetEntry<T>(InternalDefines.PreferenceNames.MainCategory.name, identifier).Value;
        }
        public void UpdatePreferenceValue<T>(string identifier, T value)
            where T : new()
        {
            MelonPreferences.GetEntry<T>(InternalDefines.PreferenceNames.MainCategory.name, identifier).Value = value;
        }

        private static void Init()
        {
            ClassInjector.RegisterTypeInIl2Cpp<Indicator>();
            GameObject ui = GameObject.Find("MiscToolsUI");
            if (ui is null)
            {
                ui = new GameObject("MiscToolsUI");
                instance.Log("Creating new GameObject");
            }
            else
            {
                instance.Log("Using existing GameObject");
            }
            indicator = ui.GetComponent<Indicator>();
            if (indicator is null)
            {
                indicator = ui.AddComponent<Indicator>();
            }
            instance.Log("Created UI");
        }
    }
}
