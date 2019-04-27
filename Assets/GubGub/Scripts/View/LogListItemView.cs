using GubGub.Scripts.Data;

namespace GubGub.Scripts.View
{
    /// <summary>
    ///  シナリオバックログのリストアイテム
    /// </summary>
    public class LogListItemView
    {
        private ScenarioLogData _scenarioLogData;


        public void Start()
        {
//            OnVoiceButton = OnVoiceButtonClick;
        }

        /// <summary>
        ///  ボイスボタンの表示状態を設定する
        /// </summary>
        /// <param name="value"></param>
        public void SetVoiceButtonVisible(bool value)
        {
//            _voiceButton.gameObject.SetActive(false);
        }

        /// <summary>
        ///  表示更新
        /// </summary>
        /// <param name="scenarioLogData"></param>
        public void Refresh(ScenarioLogData scenarioLogData)
        {
//            _scenarioLogData = scenarioLogData;
//            NameText = _scenarioLogData.SpeakerName;
//            TalkText = _scenarioLogData.Message;
//
//            if (!string.IsNullOrEmpty(_scenarioLogData.VoicePath))
//            {
//                _voiceButton.gameObject.SetActive(true);
//            }
//            else
//            {
//                _voiceButton.gameObject.SetActive(false);
//            }
        }

        /// <summary>
        ///  ボイスボタンクリック時、モデルのストリームに値を通知する
        /// </summary>
        private void OnVoiceButtonClick()
        {
            if (_scenarioLogData?.PlayVoicePathStream != null)
            {
                _scenarioLogData.PlayVoicePathStream.OnNext(_scenarioLogData.VoicePath);
            }
        }
    }
}
