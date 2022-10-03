using MelonLoader;
using System;
using System.Collections.Generic;

namespace MiscToolsForMD.SDK
{
    public class MiscTools : ISingleOnly
    {
        private static readonly List<MelonMod> calledModsList = new List<MelonMod>();
        /// <summary>
        /// Realtime game statistics provider
        /// </summary>
        public GameStatisticsProvider realtimeGameStatics
        {
            get
            {
                return InstancesManager.GetInstance<GameStatisticsProvider>();
            }
        }
        /// <summary>
        /// Init SDK and get SDK instance.<br/>
        /// You have to run it in your mod's entrance.
        /// </summary>
        /// <param name="mod">
        /// The instance of your mod, we will use first mod instance's LoggerInstance to print our output.<br/>
        /// Most of the time, you can pass "this" simply.
        /// </param>
        public static MiscTools InitSDK(MelonMod mod)
        {
            AddCallerMod(mod);
            return InstancesManager.GetInstance<SDK>();
        }
        private static void AddCallerMod(MelonMod mod)
        {
            if (!calledModsList.Contains(mod))
            {
                calledModsList.Add(mod);
            }
        }

        private static MelonMod GetFirstMod()
        {
            if (calledModsList.Count == 0)
            {
                return null;
            }
            return calledModsList[0];
        }
    }
}
