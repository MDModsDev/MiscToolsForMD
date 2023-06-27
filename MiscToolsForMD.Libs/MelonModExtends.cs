using HarmonyLib;
using MelonLoader;
using System.Reflection;

namespace MiscToolsForMD.SDK
{
    public static class MelonModExtends
    {
        public static void TryPatch(this MelonMod instance, string methodName, MethodInfo? original, MethodInfo? prefix = null, MethodInfo? postfix = null, MethodInfo? transpiler = null, MethodInfo? finalizer = null, MethodInfo? ilmanipulator = null)
        {
            if (original != null)
            {
                HarmonyMethod? prefixHarmonyMethod = null, postfixHarmonyMethod = null, transpilerHarmonyMethod = null, finalizerHarmonyMethod = null, ilmanipulatorHarmonyMethod = null;
                if (prefix != null)
                {
                    prefixHarmonyMethod = new HarmonyMethod(prefix);
                }
                if (postfix != null)
                {
                    postfixHarmonyMethod = new HarmonyMethod(postfix);
                }
                if (transpiler != null)
                {
                    transpilerHarmonyMethod = new HarmonyMethod(transpiler);
                }
                if (finalizer != null)
                {
                    finalizerHarmonyMethod = new HarmonyMethod(finalizer);
                }
                if (ilmanipulator != null)
                {
                    ilmanipulatorHarmonyMethod = new HarmonyMethod(ilmanipulator);
                }

                instance.HarmonyInstance.Patch(original, prefixHarmonyMethod, postfixHarmonyMethod, transpilerHarmonyMethod, finalizerHarmonyMethod, ilmanipulatorHarmonyMethod);
            }
            else
            {
                instance.LoggerInstance.Error(string.Format("Failed to patch {0}", methodName));
            }
        }
    }
}
