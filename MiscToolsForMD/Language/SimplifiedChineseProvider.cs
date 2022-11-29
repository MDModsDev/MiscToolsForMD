using MiscToolsForMD.MOD;
using System.Collections.Generic;
using UnityEngine;

namespace MiscToolsForMD.Language
{
    [ProvideLanguage(SystemLanguage.ChineseSimplified)]
    internal class SimplifiedChineseProvider : LanguageProvider
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
                id = "zh-cn",
                localizedControlTypes = new Dictionary<ControlType, string>()
                {
                    { ControlType.Air,"空中" },
                    { ControlType.Fever,"Fever" },
                    { ControlType.Ground,"地面" }
                },
                localizedConfigNames = new Dictionary<string, string>()
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
                },
                localizedConfigDescriptions = new Dictionary<string, string>()
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
                }
            };
            return instance;
        }
    }
}
