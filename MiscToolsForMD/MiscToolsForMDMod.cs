using Assets.Scripts.GameCore.GamePlay;
using HarmonyLib;
using MelonLoader;
using MiscToolsForMD.MOD;
using Newtonsoft.Json;
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

        public override void OnLateInitializeMelon()
        {
            if (File.Exists(InternalDefines.configPath))
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
            if (config.lyric.enabled)
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
            if (config.debug)
            {
                LoggerInstance.Msg(callerName + log);
            }
            else if (normalLog != null)
            {
                LoggerInstance.Msg(callerName + normalLog);
            }
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
