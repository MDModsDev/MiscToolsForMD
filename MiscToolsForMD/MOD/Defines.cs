using System.IO;
using UnityEngine;

namespace MiscToolsForMD.MOD
{
    public static class PublicDefines
    {
        public static readonly string id = "MiscToolsForMD.MOD";
    }

    internal static class InternalDefines
    {
        public const int windowRectId = 0;
        public const int lyricWindowId = 1;
        public static readonly Color32 defaultApColor = new Color(1, 0.8f, 0, 1); // #FFD700
        public static readonly Color32 defaultGreatColor = new Color(0.2f, 0.4f, 0.9f); // #4169E1
        public static readonly Color32 defaultMissColor = new Color(1, 1, 1); // #FFFFFF
        public static readonly Color32 defaultKeyDisplayColor = Color.white;
        public static readonly Color32 defaultKeyPressedColor = Color.black;

        public static class PreferenceNames
        {
            public static class MainCategory
            {
                public const string name = "MiscToolsForMD";
                public const string debug = "debug";
                public const string fontSize = "font_size";
            }

            public static class LyricCategory
            {
                public const string name = "lyric";
                public const string enabled = name + "_enabled";
                public const string fontSize = name + "_font_size";
                public const string coordinate = name + "_coordinate";
                public const string size = name + "_size";
            }

            public static class IndicatorCategory
            {
                public const string apEnabled = name + "_ap_enabled";
                public const string apManual = name + "_ap_manual";
                public const string apSize = name + "_ap_size";
                public const string apColor = name + "_ap_color";
                public const string greatColor = name + "_great_color";
                public const string missColor = name + "_miss_color";
                public const string keyEnabled = name + "_key_enabled";
                public const string keySize = name + "_key_size";
                public const string keyDisplay = name + "_key_display";
                public const string keyPressed = name + "_key_pressed";
                public const string name = "indicator";
                public const string coordinate = name + "_coordinate";
                public const string size = name + "_size";
            }

            public static class HardCoreCategory
            {
                public const string name = "hardcore";
                public const string enabled = name + "_enabled";
            }

            public static class SoftCoreCategory
            {
                public const string name = "softcore";
                public const string enabled = name + "_enabled";
            }
        }
    }
}

namespace MiscToolsForMD.CompatibleLayer
{
    internal static class LegacyInternalDefines
    {
        public static readonly string configPath = Path.Combine(SDK.PublicDefines.basePath, "MiscToolsForMD.json");
    }
}
