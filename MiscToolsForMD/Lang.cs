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

        public static Lang GetLang(SystemLanguage language = SystemLanguage.Unknown)
        {
            if (language == SystemLanguage.Unknown)
            {
                language = Singleton<GameAccountSystem>.instance.GetLanguage();
            }
            Lang lang = new Lang();
            switch (language)
            {
                case SystemLanguage.ChineseSimplified:
                    lang.id = "zh-cn";
                    break;

                default:
                    lang.id = "en-us";
                    break;
            }
            switch (language)
            {
                case SystemLanguage.ChineseSimplified:
                    lang.localizedControlTypes = new Dictionary<ControlType, string>()
                {
                    { ControlType.Air,"空中" },
                    { ControlType.Fever,"Fever" },
                    { ControlType.Ground,"地面" }
                };
                    break;

                default:
                    lang.localizedControlTypes = new Dictionary<ControlType, string>() {
                    { ControlType.Air, "Air" },
                    { ControlType.Fever,"Fever" },
                    { ControlType.Ground,"Ground" }
                };
                    break;
            }
            return lang;
        }
    }
}
