using System;
using System.Threading.Tasks;
using GubGub.Scripts.Enum;
using GubGub.Scripts.Lib;
using UniRx;
using UnityEngine;

namespace GubGub.Scripts.Main
{
    /// <summary>
    /// 設定に応じてシナリオプレイヤーの再生を行う
    /// </summary>
    public class ScenarioStarter : MonoBehaviour
    {
        [SerializeField] private string loadScenarioPath;

        [SerializeField] private ScenarioPresenter presenter;

        /// <summary>
        /// Awakeで自動的にシナリオのロードを行う
        /// </summary>
        [SerializeField] private bool loadOnAwake;

        /// <summary>
        /// シナリオのロードが完了すると、自動的に再生を始める
        /// </summary>
        [SerializeField] private bool isAutoPlay;

        /// <summary>
        /// リソースのアセットバンドルを再生前にロードしておくか
        /// </summary>
        [SerializeField] private bool isResourcePreload;
        
        /// <summary>
        /// リソースの読み込み先
        /// </summary>
        [SerializeField] private EResourceLoadType resourceLoadType;

        /// <summary>
        /// アセットバンドル末尾の拡張子
        /// </summary>
        [SerializeField] private string assetbundleSuffix;
        
        /// <summary>
        /// シナリオ終了を通知するストリーム
        /// </summary>
        public IObservable<Unit> IsEndScenario => _isEndScenario;
        
        private readonly Subject<Unit> _isEndScenario = new Subject<Unit>();

        
        /// <summary>
        /// 
        /// </summary>
        private async void Awake()
        {
            // 設定クラスにスタータの入力値を入れる
            ResourceLoadSetting.ResourceLoadType = resourceLoadType;
            ResourceLoadSetting.AssetBundleSuffix = assetbundleSuffix;

            Bind();
            
            if (loadOnAwake)
            {
                await LoadScenario();
            }
        }

        private void Bind()
        {
            presenter.IsEndScenario.Subscribe(_ => OnScenarioEnd()).AddTo(this);
        }

        /// <summary>
        /// シナリオの再生が終了した
        /// </summary>
        private void OnScenarioEnd()
        {
            presenter.Hide();
            
            _isEndScenario.OnNext(Unit.Default);
        }
        
        /// <summary>
        /// シナリオ、リソースの読み込みを行う
        /// </summary>
        /// <param name="scenarioFilePath"></param>
        /// <returns></returns>
        public async Task LoadScenario(string scenarioFilePath = null)
        {
            if (scenarioFilePath != null)
            {
                loadScenarioPath = scenarioFilePath;
            }

            // ローカル読み込みならプリロードは行わない
            var isPreload = (isResourcePreload &&
                             !resourceLoadType.Equals(EResourceLoadType.Resources));
            
            await presenter.LoadScenario(loadScenarioPath, isPreload);
                            
            if (isAutoPlay)
            {
                await StartScenario();
            }
        }

        /// <summary>
        /// シナリオの再生を開始する
        /// </summary>
        /// <returns></returns>
        public async Task StartScenario()
        {
            await presenter.StartScenario();
        }
    }
}
