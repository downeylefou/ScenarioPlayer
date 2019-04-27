using System.Threading.Tasks;
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
        /// 
        /// </summary>
        private async void Awake()
        {
            if (loadOnAwake)
            {
                await LoadScenario();
            }
        }
        
        /// <summary>
        /// シナリオ、リソースの読み込みを行う
        /// </summary>
        /// <returns></returns>
        public async Task LoadScenario()
        {
            await presenter.LoadScenario(loadScenarioPath);
                            
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
