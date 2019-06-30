using GubGub.Scripts.Data;
using GubGub.Scripts.View;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GubGub.Scripts.Main
{
    /// <summary>
    /// シナリオコンフィグビューの管理クラス
    /// </summary>
    public class ScenarioConfigPresenter : MonoBehaviour
    {
        [SerializeField] private ScenarioConfigView view;

        [SerializeField] private Button dimmer;
        
        /// <summary>
        /// Bgmのボリューム値が変更されたことを通知する
        /// </summary>
        public Subject<float> changedBgmVolume = new Subject<float>();
        
        /// <summary>
        /// Seのボリューム値が変更されたことを通知する
        /// </summary>
        public Subject<float> changedSeVolume = new Subject<float>();
        
        /// <summary>
        /// 暗幕のボタンイベント
        /// </summary>
        public UnityAction onTouchDimmer;

        
        public void Initialize(ScenarioConfigData config)
        {
            AddEventListener();
            Bind();
            
            view.Initialize(config);

            Hide();
        }
        
        public void Show()
        {
            gameObject.SetActive(true);
        }
        
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        
        private void AddEventListener()
        {
            dimmer.onClick.AddListener(OnClose);
            view.onCloseButton = OnClose;
        }

        private void Bind()
        {
            view.changedBgmVolume.Subscribe(changedBgmVolume).AddTo(this);
            view.changedSeVolume.Subscribe(changedSeVolume).AddTo(this);
        }

        private void OnClose()
        {
            onTouchDimmer?.Invoke();
        }
    }


}
