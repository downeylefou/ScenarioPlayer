using GubGub.Scripts.Enum;

namespace GubGub.Scripts
{
    /// <summary>
    /// リソースのパスを解決するクラス
    /// </summary>
    public static class ResourceLoadSetting
    {
        // アセットバンドルのルートからファイル名までのディレクトリパス
        public static string BackgroundResourcePrefix = "texture/background/";
        public static string StandResourcePrefix = "texture/character/";
        public static string BgmResourcePrefix = "sound/bgm/";
        public static string SeResourcePrefix = "sound/se/";
        public static string VoiceResourcePrefix = "sound/voice/";
        public static string ScenarioResourcePrefix = "scenario/";
        
        /// <summary>
        /// リソース設定ファイルのパス
        /// </summary>
        public static string ResourceSettingPath = "scenario/setting/ScenarioResourceSetting";
        
        /// <summary>
        /// リソースの読み込み先
        /// </summary>
        public static EResourceLoadType ResourceLoadType { get; set; }
        
        /// <summary>
        /// アセットバンドル末尾の拡張子
        /// </summary>
        public static string AssetBundleSuffix { get; set; }
        
        /// <summary>
        /// リソースを取得するサーバーのホスト
        /// </summary>
        public static string ServerHostUrl { get; set; }
    }
}
