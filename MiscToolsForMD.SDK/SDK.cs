using MelonLoader;
using System;
using System.Collections.Generic;

namespace MiscToolsForMD.SDK
{
    public class InstancesManager
    {
        private static readonly List<ISingleOnly> instances = new List<ISingleOnly>();
        private static HarmonyLib.Harmony harmonyInstance;

        /// <summary>
        /// Get single instance globally.
        /// </summary>
        /// <typeparam name="T">
        /// Any object implements ISingleOnly interface.
        /// </typeparam>
        /// <param name="id">
        /// The ID of that ISingleOnly object.
        /// </param>
        /// <param name="realID">
        /// If the ID has been used, we will generate a new ID and pass it to realID.
        /// Or we will pass ID to realID.
        /// </param>
        /// <param name="replace">
        /// If replace existing instance.
        /// NOTE: It is dangerous if you replace existing instance with another type. You should think twice before using it.
        /// </param>
        /// <returns>
        /// The instance of type T (globally single).
        /// </returns>
        public static T GetInstance<T>(string id, out string realID, bool replace = false)
            where T : ISingleOnly, new()
        {
            ISingleOnly instance = instances.Find(iSingleOnly => iSingleOnly.GetID() == id);
            realID = id;
            if ((instance == null) || !(instance is T))
            {
                if (!replace && (instance != null))
                {
                    int count = instances.FindLastIndex(iSingleOnly => iSingleOnly.GetID() == id) + 1;
                    realID = string.Format("{0}#{1}", id, count);
                }
                instance = CreateInstance<T>(realID);
            }
            return (T)instance;
        }

        internal static bool RemoveInstance<T>(string id)
            where T : ISingleOnly, new()
        {
            ISingleOnly instance = instances.Find(iSingleOnly => iSingleOnly.GetID() == id);
            if ((instance != null) && (instance is T))
            {
                instance.OnRemove();
                instances.Remove(instance);
                return true;
            }
            return false;
        }

        internal static void AddHarmonyInstance(HarmonyLib.Harmony instance)
        {
            if (harmonyInstance == null)
            {
                harmonyInstance = instance;
            }
        }

        internal static HarmonyLib.Harmony GetHarmony()
        {
            if (harmonyInstance == null)
            {
                throw new NullReferenceException(
                    "No available Harmony Instance, " +
                    "have you run InitSDK() method in your mod's entrance?");
            }
            return harmonyInstance;
        }

        private static T CreateInstance<T>(string id)
            where T : ISingleOnly, new()
        {
            T instance = Activator.CreateInstance<T>();
            instance.SetID(id);
            instances.Add(instance);
            return instance;
        }
    }

    public class SDK : ISingleOnly
    {
        /// <summary>
        /// Init SDK.
        /// You have to run it in your mod's entrance.
        /// We will use first Harmony instance for SDK or create one.
        /// </summary>
        /// <param name="harmony">
        /// The Harmony instance of your MOD, we will create one which ID is MiscToolsForMD.SDK if it is null.
        /// </param>
        /// <param name="loggerInstance">
        /// MelonLoader's LoggerInstance, will use Console.WriteLine if it is null.
        /// </param>
        public static SDK InitSDK(HarmonyLib.Harmony harmony = null, MelonLogger.Instance loggerInstance = null)
        {
            if (harmony == null)
            {
                harmony = new HarmonyLib.Harmony(PublicDefines.id);
            }
            InstancesManager.AddHarmonyInstance(harmony);
            InstancesManager.GetInstance<AttributeChecker>(PublicDefines.attrCheckerId, out _, true).CheckAll(loggerInstance);
            InstancesManager.GetInstance<GameStatisticsProvider>(PublicDefines.statisticProviderId, out _, true);
            return InstancesManager.GetInstance<SDK>(PublicDefines.id, out _, true);
        }

        public void SetID(string id)
        {
        }

        public string GetID()
        {
            return PublicDefines.id;
        }

        public void OnRemove()
        {
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
    {
        string GetID();

        void SetID(string id);

        void OnRemove();
    }

    public interface ILyricSource
    {
        string Name { get; }
        string Author { get; }
        string Id { get; }
        uint Priority { get; }

        List<Lyric> GetLyrics(string title, string artist);
    }
}
