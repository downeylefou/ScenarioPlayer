using GubGub.Scripts.Main;
using Sample._1_Adventure.Scripts.Data;
using UniRx.Async;

namespace Sample._1_Adventure.Scripts.Util
{
    /// <summary>
    /// シナリオ再生用のユーティリティクラス
    /// </summary>
    public static class AdvScenarioUtil
    {
        private static ScenarioStarter _starter;

        /// <summary>
        /// スタータを渡して初期化する
        /// </summary>
        /// <param name="starter"></param>
        public static void Initialize(ScenarioStarter starter)
        {
            _starter = starter;
        }
        
        /// <summary>
        /// リソース設定ファイルの読み込みを行う
        /// </summary>
        /// <returns></returns>
        public static async UniTask LoadResourceSetting()
        {
            await _starter.LoadResourceSetting();
        }

        /// <summary>
        /// シナリオの読み込みを行う
        /// </summary>
        /// <param name="scenarioPath"></param>
        /// <returns></returns>
        public static async UniTask LoadScenario(string scenarioPath)
        {
            await _starter.LoadScenario(scenarioPath);
        }

        /// <summary>
        /// 指定ラベルの再生を行う
        /// </summary>
        /// <param name="label"></param>
        public static async void PlayLabel(AdvScenarioLabel label)
        {
            await _starter.PlayLabel(label.GetName());
        }

        /// <summary>
        /// シナリオプレイヤーを非表示にする
        /// </summary>
        public static void Hide()
        {
            _starter.HideScenarioPlayer();
        }

        /// <summary>
        /// パラメータを設定する
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        public static void SetParameter<T>(string key, T value)
        {
            _starter.SetParameter(key, value);
        }

        /// <summary>
        /// パラメータを取得する
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetParameter<T>(string key)
        {
            return _starter.GetParameter<T>(key);
        }
    }
}