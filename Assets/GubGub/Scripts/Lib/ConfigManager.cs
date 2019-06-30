using GubGub.Scripts.Data;
using GubGub.Scripts.Enum;

namespace GubGub.Scripts.Lib
{
    /// <summary>
    /// シナリオのコンフィグ管理クラス
    /// </summary>
    public static class ConfigManager
    {
        public static ScenarioConfigData Config { get; private set; }

        /// <summary>
        /// 設定クラスを取得する
        /// </summary>
        /// <returns></returns>
        public static void Initialize()
        {
            Config = new ScenarioConfigData();
            LoadConfig();
        }
        
        /// <summary>
        /// 値を設定する
        /// </summary>
        public static void SetParam(EScenarioConfigKey key, float value)
        {
            Config.SetParam(key, value);
        }

        /// <summary>
        /// 設定を保存する
        /// </summary>
        public static void SaveConfig()
        {
            SetAllParameter();
            
            PlayerDataManager.Save();
        }

        /// <summary>
        /// 保存するパラメータを一括でセットする
        /// </summary>
        private static void SetAllParameter()
        {
            PlayerDataManager.SetFloat(
                EScenarioConfigKey.BgmVolume.GetName(), Config.bgmVolume.Value);
            
            PlayerDataManager.SetFloat(
                EScenarioConfigKey.SeVolume.GetName(), Config.seVolume.Value);
        }

        /// <summary>
        /// 設定をロードする
        /// </summary>
        private static void LoadConfig()
        {
            Config.bgmVolume.Value = PlayerDataManager.LoadFloat(
                EScenarioConfigKey.BgmVolume.GetName(), 0.5f);
            
            Config.seVolume.Value = PlayerDataManager.LoadFloat(
                EScenarioConfigKey.SeVolume.GetName(), 1f);
        }
    }
}