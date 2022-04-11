using Account;
using Assets.Scripts.PeroTools.Commons;
using System.Collections.Generic;
using UnityEngine;

namespace MiscToolsForMD
{
    internal class Lang
    {
        public string id;
        public string localizedAccuracy;
        public Dictionary<ControlType, string> localizedControlTypes;

        public static Lang GetLang()
        {
            return GetLang(Singleton<GameAccountSystem>.instance.GetLanguage());
        }

        public static Lang GetLang(SystemLanguage language)
        {
            Lang lang = new();
            lang.id = language switch
            {
                SystemLanguage.ChineseSimplified => "zh-cn",
                _ => "en-us",
            };
            lang.localizedAccuracy = language switch
            {
                SystemLanguage.ChineseSimplified => "准确度：{0:P}",
                _ => "Accuracy: {0:P}"
            };
            lang.localizedControlTypes = language switch
            {
                SystemLanguage.ChineseSimplified => new Dictionary<ControlType, string>()
                {
                    { ControlType.Air,"空中" },
                    { ControlType.Fever,"Fever" },
                    { ControlType.Ground,"地面" }
                },
                _ => new Dictionary<ControlType, string>() {
                    { ControlType.Air, "Air" },
                    { ControlType.Fever,"Fever" },
                    { ControlType.Ground,"Ground" }
                }
            };
            return lang;
        }
    }
}
