using Newtonsoft.Json;
using System;
using System.IO;

namespace MiscToolsForMD.SDK
{
    public static class MiscTools
    {
        private static bool initialized = false;
        private static readonly Config config = InstancesManager.GetInstance<Config>();

        /// <summary>
        /// Realtime game statistics provider
        /// </summary>
        public static GameStatisticsProvider realtimeGameStatics
        {
            get
            {
                InitToolsIfNeeded();
                return InstancesManager.GetInstance<GameStatisticsProvider>();
            }
        }

        private static void InitToolsIfNeeded()
        {
            if (!initialized)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(InternalDefines.logPath));
                if (File.Exists(InternalDefines.logPath))
                {
                    File.Delete(InternalDefines.logPath);
                }
                if (File.Exists(InternalDefines.configPath))
                {
                    try
                    {
                        Config configInFile = JsonConvert.DeserializeObject<Config>(File.ReadAllText(InternalDefines.configPath));
                        config.UpdateFrom(configInFile);
                        Logger.logLevel = config.debug ? LogLevel.DEBUG : LogLevel.INFO;
                        Logger.Info("Successfully updated config from file.");
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(string.Format("Failed to load config from {0} because {1}", InternalDefines.configPath, ex.Message));
                    }
                }
                else
                {
                    Logger.Info("No config file found, using default config.");
                }
                initialized = true;
            }
        }
    }
}
