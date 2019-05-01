using System.Collections.Generic;
using GubGub.Scripts.Command;
using GubGub.Scripts.Data;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace GubGub.Scripts.View
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
        
        private List<ScenarioLogData> logDataList = new List<ScenarioLogData>();


        /// <summary>
        /// セルビューからボイス再生を通知されるストリーム
        /// </summary>
        public Subject<string> PlayVoiceStream => _playVoiceStream;

        private readonly Subject<string> _playVoiceStream = new Subject<string>();

        
        private void Awake()
        {
            AddEventListener();
        }

        public void Show()
        {
            logScrollView.Jump(logDataList.Count);

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
                _playVoiceStream,
                message: command.Message,
                speakerName: command.SpeakerName,
                voicePath: command.VoiceName
            );
            
            logDataList.Add(log);

            UpdateLog();
        }

        private void UpdateLog()
        {
            logScrollView.UpdateData(logDataList);
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
