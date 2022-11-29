using MelonLoader;
using MiscToolsForMD.MOD;
using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

namespace MiscToolsForMD.CompatibleLayer
{
    public static class CompatibleUtils
    {
        public static void UpdatePreferences(MelonPreferences_Category preferencesCategory = null)
        {
            if (preferencesCategory == null)
            {
                preferencesCategory = MelonPreferences.GetCategory(InternalDefines.PreferenceNames.MainCategory.name);
            }
            Config legacyConfig = GetLegacyConfig();
            if (legacyConfig != null)
            {
                SetPreferenceValue(preferencesCategory, InternalDefines.PreferenceNames.MainCategory.debug, legacyConfig.debug);
                SetPreferenceValue(preferencesCategory, InternalDefines.PreferenceNames.MainCategory.fontSize, legacyConfig.size);
                SetPreferenceValue(preferencesCategory, InternalDefines.PreferenceNames.LyricCategory.enabled, legacyConfig.lyric.enabled);
                SetPreferenceValue(preferencesCategory, InternalDefines.PreferenceNames.LyricCategory.fontSize, legacyConfig.lyric.size);
                SetPreferenceValue(preferencesCategory, InternalDefines.PreferenceNames.LyricCategory.coordinate, new Vector2(legacyConfig.lyric.x, legacyConfig.lyric.y));
                SetPreferenceValue(preferencesCategory, InternalDefines.PreferenceNames.LyricCategory.size, new Vector2(legacyConfig.lyric.width, legacyConfig.lyric.height));
                SetPreferenceValue(preferencesCategory, InternalDefines.PreferenceNames.IndicatorCategory.apEnabled, legacyConfig.indicator.ap.enabled);
                SetPreferenceValue(preferencesCategory, InternalDefines.PreferenceNames.IndicatorCategory.apManual, legacyConfig.indicator.ap.manual);
                SetPreferenceValue(preferencesCategory, InternalDefines.PreferenceNames.IndicatorCategory.apSize, legacyConfig.indicator.ap.size);
                if (ColorUtility.DoTryParseHtmlColor(legacyConfig.indicator.ap.ap, out Color32 tempColor))
                {
                    SetPreferenceValue(preferencesCategory, InternalDefines.PreferenceNames.IndicatorCategory.apColor, tempColor);
                }
                if (ColorUtility.DoTryParseHtmlColor(legacyConfig.indicator.ap.great, out tempColor))
                {
                    SetPreferenceValue(preferencesCategory, InternalDefines.PreferenceNames.IndicatorCategory.greatColor, tempColor);
                }
                if (ColorUtility.DoTryParseHtmlColor(legacyConfig.indicator.ap.miss, out tempColor))
                {
                    SetPreferenceValue(preferencesCategory, InternalDefines.PreferenceNames.IndicatorCategory.missColor, tempColor);
                }
                SetPreferenceValue(preferencesCategory, InternalDefines.PreferenceNames.IndicatorCategory.keyEnabled, legacyConfig.indicator.key.enabled);
                SetPreferenceValue(preferencesCategory, InternalDefines.PreferenceNames.IndicatorCategory.keySize, legacyConfig.indicator.key.size);
                if (ColorUtility.DoTryParseHtmlColor(legacyConfig.indicator.key.display, out tempColor))
                {
                    SetPreferenceValue(preferencesCategory, InternalDefines.PreferenceNames.IndicatorCategory.keyDisplay, tempColor);
                }
                if (ColorUtility.DoTryParseHtmlColor(legacyConfig.indicator.key.pressing, out tempColor))
                {
                    SetPreferenceValue(preferencesCategory, InternalDefines.PreferenceNames.IndicatorCategory.keyPressed, tempColor);
                }
                SetPreferenceValue(preferencesCategory, InternalDefines.PreferenceNames.IndicatorCategory.coordinate, new Vector2(legacyConfig.indicator.x, legacyConfig.indicator.y));
                SetPreferenceValue(preferencesCategory, InternalDefines.PreferenceNames.IndicatorCategory.size, new Vector2(legacyConfig.indicator.width, legacyConfig.indicator.height));
                SetPreferenceValue(preferencesCategory, InternalDefines.PreferenceNames.HardCoreCategory.enabled, legacyConfig.hardcore.enabled);
                SetPreferenceValue(preferencesCategory, InternalDefines.PreferenceNames.SoftCoreCategory.enabled, legacyConfig.softcore.enabled);
            }
        }

        private static void SetPreferenceValue<T>(MelonPreferences_Category preferencesCategory, string identifier, T value)
            where T : new()
        {
            preferencesCategory.GetEntry<T>(identifier).Value = value;
        }

        private static Config GetLegacyConfig()
        {
            if (File.Exists(LegacyInternalDefines.configPath))
            {
                try
                {
                    Config config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(LegacyInternalDefines.configPath));
                    File.Delete(LegacyInternalDefines.configPath);
                    return config;
                }
                catch (Exception)
                {
                }
            }
            else if (File.Exists(Path.Combine("UserData", "MiscToolsForMD.json")))
            {
                try
                {
                    Config config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Path.Combine("UserData", "MiscToolsForMD.json")));
                    File.Delete(Path.Combine("UserData", "MiscToolsForMD.json"));
                    return config;
                }
                catch (Exception)
                {
                }
            }
            return null;
        }
    }
}
