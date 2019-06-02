using System.Collections.Generic;
using System.Threading.Tasks;
using GubGub.Scripts;
using GubGub.Scripts.Lib;
using GubGub.Scripts.Main;
using UniRx;
using UnityEngine;

namespace Sample.Scripts
{
    /// <summary>
    /// シナリオプレイヤーを使用するゲーム側のルートクラス
    /// </summary>
    public class GameRoot : MonoBehaviour
    {
        private ScenarioStarter _scenarioStarter;

        private readonly List<string> _scenarioPathList = new List<string>();

        private int _scenarioCount;

    
        private void Start()
        {
            _scenarioStarter = FindObjectOfType<ScenarioStarter>();

            Bind();

            InitializeScenario();
        }

        private void Bind()
        {
            _scenarioStarter.IsEndScenario.Subscribe(_ => OnScenarioEnd()).AddTo(this);
        }

        private void InitializeScenario()
        {
            _scenarioPathList.Add("test/test_scenario2");
            _scenarioPathList.Add("test/test_scenario");
        
            PlayScenario();
        }

        private async void PlayScenario()
        {
//            if (_scenarioPathList.Count > _scenarioCount)
//            {
//                await _scenarioStarter.LoadScenario(_scenarioPathList[_scenarioCount]);
//            }
            await _scenarioStarter.LoadScenario();
        }

        /// <summary>
        /// シナリオ終了時、まだシナリオが残っていれば再生する
        /// </summary>
        private async Task OnScenarioEnd()
        {
            _scenarioCount++;
            if (_scenarioCount < _scenarioPathList.Count)
            {
                await Task.Delay(1000);
                PlayScenario();
            }
        }
    }
}