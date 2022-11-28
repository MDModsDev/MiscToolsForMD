namespace MiscToolsForMD.CompatibleLayer
{
    public class Config
    {
        public bool debug = false;
        public int size = 24;
        public LyricConfig lyric = new LyricConfig();
        public IndicatorConfig indicator = new IndicatorConfig();
        public HardCoreConfig hardcore = new HardCoreConfig();
        public SoftCoreConfig softcore = new SoftCoreConfig();
    }

    public class LyricConfig
    {
        public bool enabled = true;
        public int size = 36;
        public int x = -1;
        public int y = -1;
        public int width = 500;
        public int height = 100;
    }

    public class IndicatorConfig
    {
        public APIndicatorConfig ap = new APIndicatorConfig();
        public KeyIndicatorConfig key = new KeyIndicatorConfig();
        public int x = -1;
        public int y = -1;
        public int width = 500;
        public int height = 100;
    }

    public class APIndicatorConfig
    {
        public bool enabled = true;
        public bool manual = false;
        public int size = 36;
        public string ap = "#FFD700";
        public string great = "#4169E1";
        public string miss = "#FFFFFF";
    }

    public class KeyIndicatorConfig
    {
        public bool enabled = true;
        public int size = 24;
        public string display = "#000000";
        public string pressing = "#FFFFFF";
    }

    public class HardCoreConfig
    {
        public bool enabled = false;
    }

    public class SoftCoreConfig
    {
        public bool enabled = false;
    }
}
