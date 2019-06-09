using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GubGub.Scripts.View
{
    /// <summary>
    /// シナリオの選択肢のビュー
    /// </summary>
    public class ScenarioSelectionView : MonoBehaviour
    {
        [SerializeField] private Text selectionMessage;
        
        [SerializeField] private Button button;

        /// <summary>
        /// クリックイベント
        /// </summary>
        private UnityAction _onClick;

        /// <summary>
        /// 選択した際の遷移先ラベル名
        /// </summary>
        private string _labelName;

        
        private void Awake()
        {
            button.onClick.AsObservable().Subscribe(_ => OnClickButton());
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <param name="messageText"></param>
        /// <param name="labelName"></param>
        /// <param name="onClick"></param>
        public void Initialize(string messageText, string labelName, UnityAction onClick)
        {
            selectionMessage.text = messageText;
            _labelName = labelName;
            _onClick = onClick;
        }
        
        /// <summary>
        /// ボタンのクリック時
        /// コールバックがあれば実行する
        /// </summary>
        private void OnClickButton()
        {
            _onClick?.Invoke();
        }
    }
}