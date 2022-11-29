using Account;
using Assets.Scripts.PeroTools.Commons;
using System.Collections.Generic;
using UnityEngine;

namespace MiscToolsForMD.MOD
{
    internal class Lang
    {
        public string id;
        public Dictionary<ControlType, string> localizedControlTypes;
        public Dictionary<string, string> localizedConfigNames;
        public Dictionary<string, string> localizedConfigDescriptions;
        private static Lang instance;

        public static Lang GetLang(SystemLanguage language = SystemLanguage.Unknown)
        {
            if (instance != null)
            {
                return instance;
            }
            if (language == SystemLanguage.Unknown)
            {
                language = Singleton<GameAccountSystem>.instance.GetLanguage();
            }
            Lang lang = new Lang();
            switch (language)
            {
                case SystemLanguage.ChineseSimplified:
                    lang.id = "zh-cn";
                    lang.localizedControlTypes = new Dictionary<ControlType, string>()
                    {
                        { ControlType.Air,"空中" },
                        { ControlType.Fever,"Fever" },
                        { ControlType.Ground,"地面" }
                    };
                    lang.localizedConfigNames = new Dictionary<string, string>()
                    {
                        {InternalDefines.PreferenceNames.MainCategory.name, "MiscToolsForMD" },
                        {InternalDefines.PreferenceNames.MainCategory.fontSize, "MOD 字体大小" },
                        {InternalDefines.PreferenceNames.MainCategory.debug, "调试模式" },
                        {InternalDefines.PreferenceNames.LyricCategory.enabled, "启用歌词显示" },
                        {InternalDefines.PreferenceNames.LyricCategory.fontSize, "歌词显示字体大小" },
                        {InternalDefines.PreferenceNames.LyricCategory.coordinate, "歌词面板位置" },
                        {InternalDefines.PreferenceNames.LyricCategory.size, "歌词面板大小" },
                        {InternalDefines.PreferenceNames.IndicatorCategory.apEnabled, "启用实时准确度显示" },
                        {InternalDefines.PreferenceNames.IndicatorCategory.apManual, "使用模组记录的比重代替游戏的比重以计算实时准确度" },
                        {InternalDefines.PreferenceNames.IndicatorCategory.apSize, "准确度指示字体大小" },
                        {InternalDefines.PreferenceNames.IndicatorCategory.apColor, "玩家处于 All Perfect 状态时的字体颜色" },
                        {InternalDefines.PreferenceNames.IndicatorCategory.greatColor, "玩家处于 FullCombo 状态时的字体颜色" },
                        {InternalDefines.PreferenceNames.IndicatorCategory.missColor, "玩家处于 Miss 状态时的字体颜色" },
                        {InternalDefines.PreferenceNames.IndicatorCategory.keyEnabled, "启用实时按键指示" },
                        {InternalDefines.PreferenceNames.IndicatorCategory.keySize, "按键指示字体大小" },
                        {InternalDefines.PreferenceNames.IndicatorCategory.keyDisplay, "按键未按下时的字体颜色" },
                        {InternalDefines.PreferenceNames.IndicatorCategory.keyPressed, "按键按下时的字体颜色" },
                        {InternalDefines.PreferenceNames.IndicatorCategory.coordinate, "准确度指示面板的位置" },
                        {InternalDefines.PreferenceNames.IndicatorCategory.size, "准确度指示面板的大小" },
                        {InternalDefines.PreferenceNames.HardCoreCategory.enabled, "启用 HardCore 模式" },
                        {InternalDefines.PreferenceNames.SoftCoreCategory.enabled, "启用 SoftCore 模式" }
                    };
                    lang.localizedConfigDescriptions = new Dictionary<string, string>()
                    {
                        {InternalDefines.PreferenceNames.MainCategory.name, "设置 MiscToolsForMD" },
                        {InternalDefines.PreferenceNames.MainCategory.fontSize, "调整 MOD 的字体大小" },
                        {InternalDefines.PreferenceNames.MainCategory.debug, "是否启用调试模式" },
                        {InternalDefines.PreferenceNames.LyricCategory.enabled, "是否启用歌词显示" },
                        {InternalDefines.PreferenceNames.LyricCategory.fontSize, "调整歌词显示字体大小" },
                        {InternalDefines.PreferenceNames.LyricCategory.coordinate, "调整歌词面板位置，负数表示自动计算" },
                        {InternalDefines.PreferenceNames.LyricCategory.size, "调整歌词面板大小" },
                        {InternalDefines.PreferenceNames.IndicatorCategory.apEnabled, "是否启用实时准确度显示" },
                        {InternalDefines.PreferenceNames.IndicatorCategory.apManual, "是否使用模组计算的准确度代替游戏的数据" },
                        {InternalDefines.PreferenceNames.IndicatorCategory.apSize, "调整准确度指示字体大小" },
                        {InternalDefines.PreferenceNames.IndicatorCategory.apColor, "调整玩家处于 All Perfect 状态时的准确度字体颜色" },
                        {InternalDefines.PreferenceNames.IndicatorCategory.greatColor, "调整玩家处于 FullCombo 状态时的准确度字体颜色" },
                        {InternalDefines.PreferenceNames.IndicatorCategory.missColor, "调整玩家处于 Miss 状态时的准确度字体颜色" },
                        {InternalDefines.PreferenceNames.IndicatorCategory.keyEnabled, "是否启用实时按键指示" },
                        {InternalDefines.PreferenceNames.IndicatorCategory.keySize, "调整按键指示字体大小" },
                        {InternalDefines.PreferenceNames.IndicatorCategory.keyDisplay, "调整按键未按下时的字体颜色" },
                        {InternalDefines.PreferenceNames.IndicatorCategory.keyPressed, "调整按键按下时的字体颜色" },
                        {InternalDefines.PreferenceNames.IndicatorCategory.coordinate, "调整指示器的位置，负数表示自动计算" },
                        {InternalDefines.PreferenceNames.IndicatorCategory.size, "调整指示器面板的大小" },
                        {InternalDefines.PreferenceNames.HardCoreCategory.enabled, "是否启用 HardCore 模式" },
                        {InternalDefines.PreferenceNames.SoftCoreCategory.enabled, "是否启用 SoftCore 模式" }
                    };
                    break;

                default:
                    lang.id = "en-us";
                    lang.localizedControlTypes = new Dictionary<ControlType, string>()
                    {
                        { ControlType.Air, "Air" },
                        { ControlType.Fever,"Fever" },
                        { ControlType.Ground,"Ground" }
                    };
                    lang.localizedConfigNames = new Dictionary<string, string>()
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
                    };
                    lang.localizedConfigDescriptions = new Dictionary<string, string>()
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
                    };
                    break;
            }
            instance = lang;
            return lang;
        }
    }
}
