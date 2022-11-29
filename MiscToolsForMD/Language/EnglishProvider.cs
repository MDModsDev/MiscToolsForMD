using MiscToolsForMD.MOD;
using System.Collections.Generic;
using UnityEngine;

namespace MiscToolsForMD.Language
{
    [ProvideLanguage(SystemLanguage.English)]
    internal class EnglishProvider : LanguageProvider
    {
        private Lang instance;

        public override Lang GetLang()
        {
            if (instance != null)
            {
                return instance;
            }
            instance = new Lang()
            {
                id = "en-us",
                localizedControlTypes = new Dictionary<ControlType, string>()
                {
                    { ControlType.Air, "Air" },
                    { ControlType.Fever, "Fever" },
                    { ControlType.Ground, "Ground" }
                },
                localizedConfigNames = new Dictionary<string, string>()
                {
                    {InternalDefines.PreferenceNames.MainCategory.name, "MiscToolsForMD" },
                    {InternalDefines.PreferenceNames.MainCategory.fontSize, "Font size of the MOD" },
                    {InternalDefines.PreferenceNames.MainCategory.debug, "Debug mode" },
                    {InternalDefines.PreferenceNames.LyricCategory.enabled, "Enable lyric display" },
                    {InternalDefines.PreferenceNames.LyricCategory.fontSize, "Font size of the lyric text" },
                    {InternalDefines.PreferenceNames.LyricCategory.coordinate, "Coordinate of the lyric panel, negative number means calculated automatically" },
                    {InternalDefines.PreferenceNames.LyricCategory.size, "Size of the lyric panel" },
                    {InternalDefines.PreferenceNames.IndicatorCategory.apEnabled, "Enable realtime accuracy indicator" },
                    {InternalDefines.PreferenceNames.IndicatorCategory.apManual, "Use weights collected by MOD instead game's to calculate accuracy" },
                    {InternalDefines.PreferenceNames.IndicatorCategory.apSize, "Font size of the accuracy text" },
                    {InternalDefines.PreferenceNames.IndicatorCategory.apColor, "Font color of All Perfect" },
                    {InternalDefines.PreferenceNames.IndicatorCategory.greatColor, "Font color of FullCombo" },
                    {InternalDefines.PreferenceNames.IndicatorCategory.missColor, "Font color of Miss" },
                    {InternalDefines.PreferenceNames.IndicatorCategory.keyEnabled, "Enable realtime key indicator" },
                    {InternalDefines.PreferenceNames.IndicatorCategory.keySize, "Font size of the key text" },
                    {InternalDefines.PreferenceNames.IndicatorCategory.keyDisplay, "Font color of normal keys" },
                    {InternalDefines.PreferenceNames.IndicatorCategory.keyPressed, "Font color of pressing keys" },
                    {InternalDefines.PreferenceNames.IndicatorCategory.coordinate, "Coordinate of the accuracy panel, negative number means calculated automatically" },
                    {InternalDefines.PreferenceNames.IndicatorCategory.size, "Size of the accuracy panel" },
                    {InternalDefines.PreferenceNames.HardCoreCategory.enabled, "Enable HardCore mode" },
                    {InternalDefines.PreferenceNames.SoftCoreCategory.enabled, "Enable SoftCore mode" }
                },
                localizedConfigDescriptions = new Dictionary<string, string>()
                {
                    {InternalDefines.PreferenceNames.MainCategory.name, "Settings for MiscToolsForMD" },
                    {InternalDefines.PreferenceNames.MainCategory.fontSize, "Adjust MOD's font size" },
                    {InternalDefines.PreferenceNames.MainCategory.debug, "If enable debug mode for the MOD" },
                    {InternalDefines.PreferenceNames.LyricCategory.enabled, "If enable lyric display" },
                    {InternalDefines.PreferenceNames.LyricCategory.fontSize, "Adjust font size of lyric text" },
                    {InternalDefines.PreferenceNames.LyricCategory.coordinate, "Adjust coordinate of lyric panel" },
                    {InternalDefines.PreferenceNames.LyricCategory.size, "Adjust size of lyric panel" },
                    {InternalDefines.PreferenceNames.IndicatorCategory.apEnabled, "If enable realtime accuracy indicator" },
                    {InternalDefines.PreferenceNames.IndicatorCategory.apManual, "If using accuracy value calculated by MOD" },
                    {InternalDefines.PreferenceNames.IndicatorCategory.apSize, "Adjust font size of accuracy text" },
                    {InternalDefines.PreferenceNames.IndicatorCategory.apColor, "Adjust font color of accuracy text when player is All Perfect" },
                    {InternalDefines.PreferenceNames.IndicatorCategory.greatColor, "Adjust font color of accuracy text when player is FullCombo" },
                    {InternalDefines.PreferenceNames.IndicatorCategory.missColor, "Adjust font color of accuracy text when player is Miss" },
                    {InternalDefines.PreferenceNames.IndicatorCategory.keyEnabled, "If enable realtime key indicator" },
                    {InternalDefines.PreferenceNames.IndicatorCategory.keySize, "Adjust font size of the key" },
                    {InternalDefines.PreferenceNames.IndicatorCategory.keyDisplay, "Adjust font color of the non-pressed keys" },
                    {InternalDefines.PreferenceNames.IndicatorCategory.keyPressed, "Adjust font color of the pressed keys" },
                    {InternalDefines.PreferenceNames.IndicatorCategory.coordinate, "Adjust coordinate of the indicator" },
                    {InternalDefines.PreferenceNames.IndicatorCategory.size, "Adjust size of the indicator" },
                    {InternalDefines.PreferenceNames.HardCoreCategory.enabled, "If enable HardCore mode" },
                    {InternalDefines.PreferenceNames.SoftCoreCategory.enabled, "If enable SoftCore mode" }
                }
            };
            return instance;
        }
    }
}
