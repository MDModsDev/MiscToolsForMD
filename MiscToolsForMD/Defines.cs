using System.IO;

namespace MiscToolsForMD.MOD
{
    public static class PublicDefines
    {
        public static readonly string id = "MiscToolsForMD.MOD";
    }

    internal static class InternalDefines
    {
        public static readonly string configPath = Path.Combine(SDK.PublicDefines.basePath, "MiscToolsForMD.json");
        public const int windowRectId = 0;
        public const int lyricWindowId = 1;
    }
}
