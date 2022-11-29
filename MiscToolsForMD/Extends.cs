using MelonLoader;
using MiscToolsForMD.Language;
using System;

namespace MiscToolsForMD.Extends
{
    public static class MelonPreferencesEntryExtends
    {
        public static void CreateEntryIfNotExist<T>(this MelonPreferences_Category category, string identifier, T default_value, string display_name = null, string description = null, bool is_hidden = false, bool dont_save_default = false, MelonLoader.Preferences.ValueValidator validator = null, string oldidentifier = null)
            where T : new()
        {
            if (display_name == null)
            {
                try
                {
                    string displayNameInLang = Lang.GetLang().localizedConfigNames[identifier];
                    display_name = displayNameInLang;
                }
                catch (Exception) { }
            }
            if (description == null)
            {
                try
                {
                    string descriptionInLang = Lang.GetLang().localizedConfigDescriptions[identifier];
                    description = descriptionInLang;
                }
                catch (Exception) { }
            }
            if (!category.HasEntry(identifier))
            {
                category.CreateEntry(identifier, default_value, display_name, description, is_hidden, dont_save_default, validator, oldidentifier);
            }
        }
    }
}
