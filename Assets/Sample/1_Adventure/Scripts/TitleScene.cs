using System;
using GubGub.Scripts.Main;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Sample._0_Test.Scripts
{
    /// <summary>
    /// タイトルシーンクラス
    /// </summary>
    public class TitleScene : MonoBehaviour
    {
        /// <summary>
        /// スタートボタンが押されたことを通知する
        /// </summary>
        public IObservable<Unit> OnClickStartButton => _onClickStartButton;
        
        private readonly Subject<Unit> _onClickStartButton = new Subject<Unit>();
        
        [SerializeField] private Button startButton;
        
        // TODO: 表示テスト用　
        private ScenarioStarter _scenarioStarter;


        private void Start()
        {
            _scenarioStarter = FindObjectOfType<ScenarioStarter>();

            Bind();
        }

        private void Bind()
        {
            startButton.onClick.AddListener(() =>
            {
                startButton.enabled = false;
                _onClickStartButton.OnNext(Unit.Default);
            });
        }
    }

}
