using GubGub.Scripts.Enum;

namespace GubGub.Scripts
{
    /// <summary>
    /// リソースのパスを解決するクラス
    /// </summary>
    public static class ResourceLoadSetting
    {
        public static string BackgroundResourcePrefix = "texture/background/";
        public static string StandResourcePrefix = "texture/character/";
        public static string BgmResourcePrefix = "sound/bgm/";
        public static string SeResourcePrefix = "sound/se/";
        public static string ScenarioResourcePrefix = "scenario/";
        
        /// <summary>
        /// リソースの読み込み先
        /// </summary>
        public static EResourceLoadType ResourceLoadType { get; set; }
    }
}
