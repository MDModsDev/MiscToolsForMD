using MelonLoader;
using System;
using System.Collections.Generic;

namespace MiscToolsForMD.SDK
{
    public class InstancesManager
    {
        private static readonly List<ISingleOnly> instances = new List<ISingleOnly>();
        private static readonly List<MelonMod> calledModsList = new List<MelonMod>();
        private static readonly HarmonyLib.Harmony harmonyInstance = new HarmonyLib.Harmony(PublicDefines.id);

        /// <summary>
        /// Get single instance globally.
        /// </summary>
        /// <typeparam name="T">
        /// Any object implements ISingleOnly interface.
        /// </typeparam>
        /// <returns>
        /// The globally single instance of type <c>T</c>.
        /// </returns>
        public static T GetInstance<T>()
            where T : ISingleOnly, new()
        {
            foreach (ISingleOnly singleOnly in instances)
            {
                if (typeof(T).IsInstanceOfType(singleOnly))
                {
                    return (T)singleOnly;
                }
            }
            T instance = Activator.CreateInstance<T>();
            instances.Add(instance);
            AttributeChecker.Check(instance);
            return instance;
        }

        internal static void AddCallerMod(MelonMod mod)
        {
            if (!calledModsList.Contains(mod))
            {
                calledModsList.Add(mod);
            }
        }

        internal static MelonMod GetFirstMod()
        {
            if (calledModsList.Count == 0)
            {
                return null;
            }
            return calledModsList[0];
        }

        internal static HarmonyLib.Harmony GetHarmony()
        {
            return harmonyInstance;
        }
    }

    public class SDK : ISingleOnly
    {
        /// <summary>
        /// Init SDK and get SDK instance.<br/>
        /// You have to run it in your mod's entrance.
        /// </summary>
        /// <param name="mod">
        /// The instance of your mod, we will use first mod instance's LoggerInstance to print our output.<br/>
        /// Most of the time, you can pass "this" simply.
        /// </param>
        public static SDK InitSDK(MelonMod mod)
        {
            InstancesManager.GetInstance<GameStatisticsProvider>();
            InstancesManager.AddCallerMod(mod);
            return InstancesManager.GetInstance<SDK>();
        }
    }

    public class MusicDisplayInfo
    {
        /// <summary>
        /// Title of the song.
        /// </summary>
        public string musicName;

        /// <summary>
        /// Author of the song
        /// </summary>
        public string authorName;

        public override string ToString()
        {
            return string.Format("{0}-{1}", musicName, authorName);
        }
    }

    /// <summary>
    /// Lyric object used to display lyric in game
    /// </summary>
    public class Lyric
    {
        public string content;
        public float time;

        public static Lyric GetLyricByTime(List<Lyric> lst, float time)
        {
            Lyric lyricFound = lst.Find(lyric => lyric.time == time);
            if (lyricFound == null)
            {
                lyricFound = new Lyric() { time = time, content = "" };
            }
            return lyricFound;
        }
    }

    public interface ISingleOnly
    { }

    public interface ILyricSource
    {
        string Name { get; }
        string Author { get; }
        string Id { get; }
        uint Priority { get; }

        List<Lyric> GetLyrics(string title, string artist);
    }
}
