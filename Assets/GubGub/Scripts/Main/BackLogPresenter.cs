using System.Collections.Generic;
using GubGub.Scripts.Command;
using GubGub.Scripts.Data;
using GubGub.Scripts.View;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace GubGub.Scripts.Main
{
    /// <summary>
    /// シナリオのバックログ表示クラス
    /// </summary>
    public class BackLogPresenter : MonoBehaviour
    {
        [SerializeField] private LogScrollView logScrollView;

        [SerializeField] private Button dimmer;

        [SerializeField] private CanvasGroup canvasGroup;
        
        /// <summary>
        /// 暗幕のボタンイベント
        /// </summary>
        public UnityAction onTouchDimmer;
        
        private readonly List<ScenarioLogData> _logDataList = new List<ScenarioLogData>();


        /// <summary>
        /// セルビューからボイス再生を通知されるストリーム
        /// </summary>
        public Subject<string> PlayVoiceStream { get; } = new Subject<string>();


        private void Awake()
        {
            AddEventListener();
            
            Hide();
        }

        public void Show()
        {
            logScrollView.Jump(_logDataList.Count);

            SetVisible(true);
        }
        
        public void Hide()
        {
            SetVisible(false);
        }

        /// <summary>
        /// ログを追加して更新する
        /// </summary>
        /// <param name="command"></param>
        public void AddScenarioLog(MessageCommand command)
        {
            var log = new ScenarioLogData
            (
                PlayVoiceStream,
                message: command.Message,
                speakerName: command.SpeakerName,
                voicePath: command.VoiceName
            );
            
            _logDataList.Add(log);

            UpdateLog();
        }

        private void UpdateLog()
        {
            logScrollView.UpdateData(_logDataList);
        }
        
        private void AddEventListener()
        {
            dimmer.onClick.AddListener(() =>
            {
                onTouchDimmer?.Invoke();
            });
        }
        
        /// <summary>
        /// 表示状態を設定する
        /// gameObjectが非アクティブだとセルのアニメータが動作しないため、
        /// CanvasGroupでアルファだけ変更する
        /// </summary>
        /// <param name="isVisible"></param>
        private void SetVisible(bool isVisible)
        {
            canvasGroup.alpha = (isVisible)?    1 : 0;
            canvasGroup.interactable = isVisible;
            canvasGroup.blocksRaycasts = isVisible;
        }
    }
}
