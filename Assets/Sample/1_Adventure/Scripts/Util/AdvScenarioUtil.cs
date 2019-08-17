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
    }
}