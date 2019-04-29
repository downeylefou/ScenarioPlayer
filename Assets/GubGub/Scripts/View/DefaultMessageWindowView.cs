using GubGub.Scripts.View.Interface;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GubGub.Scripts.View
{
    /// <summary>
    ///  通常のシナリオメッセージウィンドウ
    /// </summary>
    public class DefaultMessageWindowView : MonoBehaviour, IMessageWindowView
    {
        public UnityAction OnOptionButton { get; set; }
        public UnityAction OnCloseButton { get; set; }
        public UnityAction OnLogButton { get; set; }
        public UnityAction<bool> OnAutoButton { get; set; }
        public UnityAction<bool> OnSkipButton { get; set; }

        [SerializeField] private Toggle skipButton;
        [SerializeField] private Toggle autoButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button optionButton;
        [SerializeField] private Button logButton;
        
        
        public string MessageText
        {
            get => messageText.text;
            set => messageText.text = value;
        }
        
        public string NameText
        {
            get => nameText.text;
            set => nameText.text = value;
        }
        
        public Image NextIcon => nextIcon; 

        [SerializeField] private Text messageText;
        [SerializeField] private Text nameText;
        
        [SerializeField] private Image nextIcon;


        public void Start()
        {
            skipButton.onValueChanged.AddListener(OnSkipButton);
            autoButton.onValueChanged.AddListener(OnAutoButton);
            closeButton.onClick.AddListener(OnCloseButton);
            optionButton.onClick.AddListener(OnOptionButton);
            logButton.onClick.AddListener(OnLogButton);
        }
        
        public void SetParent(Transform parent,  bool worldPositionStays)
        {
            gameObject.transform.SetParent(parent, worldPositionStays);
        }
        
        /// <summary>
        /// オートボタンのトグル状態を設定する
        /// </summary>
        /// <param name="isAuto"></param>
        public void SetAutoButtonState(bool isAuto)
        {
            autoButton.isOn = isAuto;
        }
        
        /// <summary>
        /// スキップボタンのトグル状態を設定する
        /// </summary>
        /// <param name="isAuto"></param>
        public void SetSkipButtonState(bool isAuto)
        {
            skipButton.isOn = isAuto;
        }
    }
}
