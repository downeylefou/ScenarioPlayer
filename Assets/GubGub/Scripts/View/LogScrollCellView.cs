using FancyScrollView;
using GubGub.Scripts.Data;
using UnityEngine;
using UnityEngine.UI;

namespace GubGub.Scripts.View
{
    /// <summary>
    /// シナリオバックログでスクロールに表示されるセルビュー
    /// </summary>
    public class LogScrollCellView : FancyCell<ScenarioLogData>
    {
        /// <summary>
        /// ビューの表示を更新するアニメータ
        /// </summary>
        [SerializeField] private Animator animator;

        [SerializeField] private Text nameText;
        [SerializeField] private Text messageText;
        [SerializeField] private string voicePath;
        [SerializeField] private Button voiceButton;
        
        private float _currentPosition;

        private ScenarioLogData _scenarioLogData;
        

        static class AnimatorHash
        {
            public static readonly int Scroll = Animator.StringToHash("scroll");
        }

        private void Awake()
        {
            voiceButton.onClick.AddListener(OnVoiceButton);
        }

        /// <summary>
        /// 表示の更新
        /// </summary>
        /// <param name="itemData"></param>
        public override void UpdateContent(ScenarioLogData itemData)
        {
            _scenarioLogData = itemData;
            
            nameText.text = _scenarioLogData.SpeakerName;
            messageText.text = _scenarioLogData.Message;
            voicePath = _scenarioLogData.VoicePath;

            var voiceButtonEnable = (!string.IsNullOrEmpty(voicePath));
            if (voiceButton != null && voiceButton.gameObject != null)
            {
                voiceButton.gameObject.SetActive(voiceButtonEnable);
            }
        }
    
        /// <summary>
        /// アニメータからアンカーを変更し、表示位置を更新する
        /// </summary>
        /// <param name="position"></param>
        public override void UpdatePosition(float position)
        {
            _currentPosition = position;
            if (animator.isActiveAndEnabled)
            {
                animator.Play(AnimatorHash.Scroll, -1, position);
            }
            animator.speed = 0;
        }
        
        /// <summary>
        /// ボイスボタン
        /// ストリームに通知する
        /// </summary>
        private void OnVoiceButton()
        {
            _scenarioLogData?.PlayVoiceStream?.OnNext(_scenarioLogData.VoicePath);
        }
        
        private void OnEnable() => UpdatePosition(_currentPosition);
        
    }
}
