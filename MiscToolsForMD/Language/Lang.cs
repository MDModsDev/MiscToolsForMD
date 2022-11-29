using Account;
using Assets.Scripts.PeroTools.Commons;
using MiscToolsForMD.MOD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MiscToolsForMD.Language
{
    internal class Lang
    {
        public string id;
        public Dictionary<ControlType, string> localizedControlTypes;
        public Dictionary<string, string> localizedConfigNames;
        public Dictionary<string, string> localizedConfigDescriptions;

        private static LanguageProvider currentLanguageProvider;

        public static Lang GetLang(SystemLanguage language = SystemLanguage.Unknown)
        {
            if (language == SystemLanguage.Unknown)
            {
                language = Singleton<GameAccountSystem>.instance.GetLanguage();
            }
            if ((currentLanguageProvider == null) ||
                (currentLanguageProvider.GetType().GetCustomAttribute<ProvideLanguageAttribute>().providedLanguage != language))
            {
                currentLanguageProvider = (LanguageProvider)Activator.CreateInstance(Assembly.GetExecutingAssembly().GetTypes().Where(
                    t => (!t.IsAbstract) && typeof(LanguageProvider).IsAssignableFrom(t)
                ).First(t => t.GetCustomAttribute<ProvideLanguageAttribute>().providedLanguage == language));
            }
            return currentLanguageProvider.GetLang();
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    internal class ProvideLanguageAttribute : Attribute
    {
        public SystemLanguage providedLanguage = SystemLanguage.Unknown;

        public ProvideLanguageAttribute(SystemLanguage systemLanguage)
        {
            providedLanguage = systemLanguage;
        }
    }
}
