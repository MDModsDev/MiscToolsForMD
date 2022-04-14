using System;
using System.Collections.Generic;

namespace MiscToolsForMD.SDK
{
    public class InstancesManager
    {
        private static readonly List<ISingleOnly> instances = new List<ISingleOnly>();

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
        /// </param>
        /// <returns>
        /// The instance of type T (globally single).
        /// </returns>
        public static T GetInstance<T>(string id, out string realID)
            where T : ISingleOnly, new()
        {
            // NOTE: We will add a suffix to id if there has already been an instance while their types are not equal.
            //       This mpdified id will be passed to realID. realID will be id if there is no change.
            ISingleOnly instance = instances.Find(iSingleOnly => iSingleOnly.GetID() == id);
            realID = id;
            if (instance == null)
            {
                instance = Activator.CreateInstance<T>();
                instance.SetID(realID);
                instances.Add(instance);
            }
            else if (!(instance is T))
            {
                int count = instances.FindLastIndex(iSingleOnly => iSingleOnly.GetID() == id) + 1;
                instance = Activator.CreateInstance<T>();
                realID = string.Format("{0}#{1}", id, count);
                instance.SetID(realID);
                instances.Add(instance);
            }
            return (T)instance;
        }

        /// <summary>
        /// Remove instance globally.
        /// </summary>
        /// <typeparam name="T">
        /// Any object implements ISingleOnly
        /// </typeparam>
        /// <param name="id">
        /// The ID of the object you want to destroy.
        /// </param>
        /// <returns>
        /// If destroy successfully.
        /// </returns>
        public static bool RemoveInstance<T>(string id)
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
    }

    /// <summary>
    /// Music info
    /// musicName: title of the song
    /// authorName: author of the song
    /// </summary>
    public class MusicDisplayInfo
    {
        public string musicName;
        public string authorName;
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
