using System;
using GubGub.Scripts.Enum;
using GubGub.Scripts.Lib;
using UniRx;
using UniRx.Async;
using UnityEngine;

namespace GubGub.Scripts.Main
{
    /// <summary>
    /// 設定に応じてシナリオプレイヤーの再生を行う
    /// </summary>
    public class ScenarioStarter : MonoBehaviour
    {
        /// <summary>
        /// 読み込むシナリオのパス
        /// </summary>
        [SerializeField] private string loadScenarioPath;

        /// <summary>
        /// リソースを取得するサーバーのホスト
        /// </summary>
        [SerializeField] private string serverHostUrl;

        /// <summary>
        /// シナリオプレイヤーのメインプレゼンター
        /// </summary>
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
        [SerializeField] private string assetBundleSuffix;

        /// <summary>
        /// シナリオ終了を通知するストリーム
        /// </summary>
        public IObservable<Unit> IsEndScenario => _isEndScenario;

        private readonly Subject<Unit> _isEndScenario = new Subject<Unit>();

        /// <summary>
        /// 初期化済みか
        /// </summary>
        private bool _isInitialized;


        private async void Awake()
        {
            await Initialize();
        }

        /// <summary>
        /// 初期化する
        /// </summary>
        public async UniTask Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            await presenter.Initialize();
            
            InitializeResourceLoadSetting();
            Bind();

            if (loadOnAwake)
            {
                await LoadScenario();
            }

            _isInitialized = true;
        }

        /// <summary>
        /// 設定クラスにスタータの入力値を入れる
        /// </summary>
        private void InitializeResourceLoadSetting()
        {
            ResourceLoadSetting.ResourceLoadType = resourceLoadType;
            ResourceLoadSetting.AssetBundleSuffix = assetBundleSuffix;
            ResourceLoadSetting.ServerHostUrl = serverHostUrl;
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

            // リソースを解放
            ResourceManager.UnloadAllAsset();

            _isEndScenario.OnNext(Unit.Default);
        }

        /// <summary>
        /// シナリオ、リソースの読み込みを行う
        /// </summary>
        /// <param name="scenarioFilePath"></param>
        /// <returns></returns>
        public async UniTask LoadScenario(string scenarioFilePath = null)
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
        public async UniTask StartScenario()
        {
            await presenter.StartScenario();
        }

        /// <summary>
        /// 指定ラベルからシナリオの再生を開始する
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public async UniTask PlayLabel(string label)
        {
            await presenter.StartScenario(label);
        }

        /// <summary>
        /// シナリオプレイヤーを非表示にする
        /// </summary>
        public void HideScenarioPlayer()
        {
            presenter.Hide();
        }
        
        /// <summary>
        /// パラメータを設定する
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        public void SetParameter<T>(string key, T value)
        {
            presenter.SetParameter(key, value);
        }
        
        /// <summary>
        /// パラメータを取得する
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetParameter<T>(string key)
        {
            return presenter.GetParameter<T>(key);
        }
    }
}