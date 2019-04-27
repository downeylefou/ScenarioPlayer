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
        public UnityAction OnAutoButton { get; set; }
        public UnityAction OnSkipButton { get; set; }
        public string MessageText
        {
            get { return messageText.text; }
            set { messageText.text = value; }
        }
        
        public string NameText
        {
            get { return nameText.text; }
            set { nameText.text = value; }
        }
        
        public Image NextIcon => nextIcon;

        [SerializeField] private Text messageText;
        [SerializeField] private Text nameText;
        
        [SerializeField] private Image nextIcon;

//        public GameObject gameObject => gameObject;


        
        public void SetParent(Transform parent,  bool worldPositionStays)
        {
            gameObject.transform.SetParent(parent, worldPositionStays);
        }
    }
}
