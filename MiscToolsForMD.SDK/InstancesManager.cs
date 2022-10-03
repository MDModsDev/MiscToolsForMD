using MelonLoader;

namespace MiscToolsForMD.SDK
{
    internal static class InstancesManager
    {
        private static readonly List<ISingleOnly> instances = new List<ISingleOnly>();
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


        public static HarmonyLib.Harmony GetHarmony()
        {
            return harmonyInstance;
        }
    }
}