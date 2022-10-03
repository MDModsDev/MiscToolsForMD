namespace MiscToolsForMD.SDK
{
    public static class MiscTools
    {
        /// <summary>
        /// Realtime game statistics provider
        /// </summary>
        public static GameStatisticsProvider realtimeGameStatics
        {
            get
            {
                return InstancesManager.GetInstance<GameStatisticsProvider>();
            }
        }
    }
}
