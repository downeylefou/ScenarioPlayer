using System.Threading.Tasks;
using GubGub.Scripts.Main;
using Sample._0_Test.Scripts;
using Sample._1_Adventure.Scripts.Scene;
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
            titleScene.gameObject.SetActive(true);
            gameScene.gameObject.SetActive(false);
            
            // ローディングの初期化
            LoadingUtil.Initialize(FindObjectOfType<LoadingView>());

            await InitializeScenarioPlayer();

            Bind();

            // デバグ用
//            await DebugGameSceneStarter();
        }

        /// <summary>
        /// デバグ用    タイトルからすぐにゲームシーンに移動する
        /// </summary>
        /// <returns></returns>
        private async UniTask DebugGameSceneStarter()
        {
            gameScene.gameObject.SetActive(true);
            titleScene.gameObject.SetActive(false);
            gameScene.Initialize();
            await gameScene.StartScene();
        }

        /// <summary>
        /// シナリオプレイヤー周りを初期化する
        /// </summary>
        /// <returns></returns>
        private async UniTask InitializeScenarioPlayer()
        {
            // シナリオプレイヤーの初期化
            await scenarioStarter.Initialize();
            AdvScenarioUtil.Initialize(scenarioStarter);
            AdvScenarioUtil.Hide();

            // 設定ファイルの後にシナリオを読み込む
            await AdvScenarioUtil.LoadResourceSetting();
            await AdvScenarioUtil.LoadScenario("advTest");
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
            
            gameScene.Initialize();
            await gameScene.StartScene();
        }
    }
}