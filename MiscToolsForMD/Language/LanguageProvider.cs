using System;

namespace MiscToolsForMD.Language
{
    internal abstract class LanguageProvider
    {
        public LanguageProvider()
        {
            if (!GetType().IsDefined(typeof(ProvideLanguageAttribute), true))
            {
                throw new InvalidOperationException(
                    "You need to add MiscToolsForMD.Language.ProvideLanguageAttribute to your provider."
                );
            }
        }

        public abstract Lang GetLang();
    }
}
