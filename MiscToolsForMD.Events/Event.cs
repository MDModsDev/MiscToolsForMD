using System.Text;
using System.Text.Json;

namespace MiscToolsForMD.SDK
{
    public class Event<T> : ISingleOnly
        where T : notnull
    {
        private readonly List<Action<T>> callbacks = new();

        /// <summary>
        /// Add a callback function to the event
        /// </summary>
        /// <param name="callback">The callback function</param>
        /// <returns>If add succesfully</returns>
        public bool AddCallback(Action<T> callback)
        {
            if (callbacks.Contains(callback))
            {
                return false;
            }
            callbacks.Add(callback);
            return true;
        }

        /// <summary>
        /// Remove a callback function from the event
        /// </summary>
        /// <param name="callback">the callback function</param>
        /// <returns>If remove successfully</returns>
        public bool RemoveCallback(Action<T> callback)
        {
            if (callbacks.Contains(callback))
            {
                return callbacks.Remove(callback);
            }
            return false;
        }

        /// <summary>
        /// Invoke all callbacks related to the event
        /// </summary>
        /// <param name="t">The params to the callback function</param>

        public void InvokeCallbacks(T t)
        {
            JsonSerializerOptions options = new()
            {
                WriteIndented = true,
            };
            SDKLogger.Debug(
                string.Format(
                    "[{0}]: Calling callbacks with argument {1}",
                    DiagnosticUtils.GetCallerFullName(),
                    Encoding.UTF8.GetString(JsonSerializer.SerializeToUtf8Bytes(t, options))
                )
            );
            callbacks.ForEach(
                delegate (Action<T> callback)
                {
                    try { callback(t); }
                    catch (Exception) { }
                }
            );
        }
    }
}
