using System.Collections;
using System.Collections.Generic;
using GubGub.Scripts.Data;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GubGub.Scripts.View
{
    /// <summary>
    /// シナリオコンフィグのビュークラス
    /// </summary>
    public class ScenarioConfigView : MonoBehaviour
    {
        [SerializeField] private Button closeButton;

        [SerializeField] private Slider bgmVolumeSlider;

        [SerializeField] private Slider seVolumeSlider;

        /// <summary>
        /// 閉じるボタンのイベント
        /// </summary>
        public UnityAction onCloseButton; 
        
        /// <summary>
        /// Bgmのボリューム値が変更されたことを通知する
        /// </summary>
        public FloatReactiveProperty changedBgmVolume = new FloatReactiveProperty();
  
        /// <summary>
        /// Seのボリューム値が変更されたことを通知する
        /// </summary>
        public FloatReactiveProperty changedSeVolume = new FloatReactiveProperty();
        
        
        /// <summary>
        /// 初期化処理
        /// <para>コンフィグデータから初期表示を変更する</para> 
        /// </summary>
        /// <param name="config"></param>
        public void Initialize(ScenarioConfigData config)
        {
            AddEventListener();
            Bind();
            
            bgmVolumeSlider.value = config.bgmVolume.Value;
            seVolumeSlider.value = config.seVolume.Value;
        }
        
        private void AddEventListener()
        {
            closeButton.onClick.AddListener(onCloseButton);
        }

        private void Bind()
        {
            bgmVolumeSlider.OnValueChangedAsObservable()
                .Subscribe(_ => changedBgmVolume.Value = _).AddTo(this);
            
            seVolumeSlider.OnValueChangedAsObservable()
                .Subscribe(_ => changedSeVolume.Value = _).AddTo(this);
        }


    }
    
}
