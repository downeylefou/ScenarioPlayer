using GubGub.Scripts.Main;
using Sample._0_Test.Scripts;
using Sample._1_Adventure.Scripts.Util;
using Sample._1_Adventure.Scripts.View;
using UniRx;
using UniRx.Async;
using UnityEngine;

namespace Sample._1_Adventure.Scripts
{
    /// <summary>
    /// アドベンチャーゲームサンプルのルートクラス
    /// </summary>
    public class AdventureGameRoot : MonoBehaviour
    {
        [SerializeField] private ScenarioStarter scenarioStarter;

        [SerializeField] private TitleScene titleScene;
        [SerializeField] private GameScene gameScene;

        private async void Start()
        {
            // ローディングの初期化
            LoadingUtil.Initialize(FindObjectOfType<LoadingView>());
            
            // シナリオプレイヤーの初期化
            await scenarioStarter.Initialize();
            AdvScenarioUtil.Initialize(scenarioStarter);
            AdvScenarioUtil.Hide();

            await AdvScenarioUtil.LoadScenario("advTest");

            Bind();
        }

        private void Bind()
        {
            // タイトルシーンでボタンが押されたらゲームシーンに移動する
            titleScene.OnClickStartButton.Subscribe(
                async _ => await MoveGameScene()).AddTo(this);
        }

        /// <summary>
        /// ゲームシーンに移動する
        /// </summary>
        private async UniTask MoveGameScene()
        {
            await LoadingUtil.Show();
            
            gameScene.gameObject.SetActive(true);
            titleScene.gameObject.SetActive(false);

            await LoadingUtil.Wait();
            await LoadingUtil.Hide();

            gameScene.StartScene();
        }
    }
}