namespace MiscToolsForMD.SDK
{
    public interface ISingleOnly
    { }

    public static class InstancesManager
    {
        private static readonly List<ISingleOnly> instances = new();

        /// <summary>
        /// Get single instance globally.
        /// </summary>
        /// <typeparam name="T">
        /// Any object implements ISingleOnly interface.
        /// </typeparam>
        /// <returns>
        /// The globally single instance of type given.
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
    }
}
