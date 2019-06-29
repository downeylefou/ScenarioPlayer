using System.Collections;
using System.Collections.Generic;
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
        
        
        private void Awake()
        {
            AddEventListener();
            Bind();
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
