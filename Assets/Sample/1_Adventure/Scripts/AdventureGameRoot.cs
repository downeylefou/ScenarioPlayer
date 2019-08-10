using Sample._0_Test.Scripts;
using UniRx;
using UnityEngine;

namespace Sample._1_Adventure.Scripts
{
    /// <summary>
    /// アドベンチャーゲームサンプルのルートクラス
    /// </summary>
    public class AdventureGameRoot : MonoBehaviour
    {
        [SerializeField] private TitleScene titleScene;
        [SerializeField] private GameScene gameScene;

        
        private void Start()
        {
            // タイトルシーンでボタンが押されたらゲームシーンに移動する
            titleScene.OnClickStartButton.
                Subscribe(_ => MoveGameScene()).AddTo(this);
        }

        /// <summary>
        /// ゲームシーンに移動する
        /// </summary>
        private void MoveGameScene()
        {
            gameScene.gameObject.SetActive(true);
            titleScene.gameObject.SetActive(false);
        }
    }
    

}
