using System.Collections.Generic;
using System.Threading.Tasks;
using GubGub.Scripts.Enum;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GubGub.Scripts.Main
{
    /// <inheritdoc />
    /// <summary>
    ///  シナリオビュークラス
    /// </summary>
    public class ScenarioView : MonoBehaviour
    {
        [SerializeField] private ScenarioMessagePresenter scenarioMessagePresenter;
        [SerializeField] private GameObject backgroundRoot;
        [SerializeField] private GameObject standImageRoot;
        [SerializeField] private GameObject clickArea;
        [SerializeField] private GameObject fadeImage;
        [SerializeField] private GameObject messageWindowPosBottom;
        [SerializeField] private GameObject messageWindowPosTop;
        [SerializeField] private GameObject messageWindowPosCenter;

        /// <summary>
        ///  画面内のどこかをクリックした
        /// </summary>
        public Subject<PointerEventData> onAnyClick = new Subject<PointerEventData>();

        /// <summary>
        ///  メッセージビューのタイプ
        /// </summary>
        public EScenarioMessageViewType MessageViewType { get; set; }


        /// <summary>
        ///  立ち位置と、そこに表示されている立ち絵オブジェクトのリスト
        /// </summary>
        private Dictionary<EScenarioStandPosition, GameObject> _standImages =
            new Dictionary<EScenarioStandPosition, GameObject>()
            {
                {EScenarioStandPosition.Left, null}, {EScenarioStandPosition.Center, null},
                {EScenarioStandPosition.Right, null}
            };

        /// <summary>
        /// メッセージビューの管理クラス
        /// </summary>
        public ScenarioMessagePresenter MessagePresenter { get; private set; }


        public async Task Initialize()
        {
            await InitializeMessageView();
            AddEventListeners();
        }

        private void AddEventListeners()
        {
            clickArea.GetComponent<Image>().OnPointerClickAsObservable().Subscribe(onAnyClick);
        }

        /// <summary>
        ///  メッセージビューを初期化
        /// </summary>
        /// <returns></returns>
        private async Task InitializeMessageView()
        {
            MessagePresenter = Instantiate(scenarioMessagePresenter, transform);
            await MessagePresenter.Initialize();
            
            // メッセージビューを初期位置に配置
            ChangeMessageViewPosition(EScenarioMessageViewPosition.Bottom);
        }

        /// <summary>
        /// メッセージウィンドウの基本ポジションを変更
        /// </summary>
        /// <param name="position"></param>
        public void ChangeMessageViewPosition(EScenarioMessageViewPosition position)
        {
            switch (position)
            {
                case EScenarioMessageViewPosition.Bottom:
                    MessagePresenter.SetViewParent(messageWindowPosBottom.transform);
                    break;
                case EScenarioMessageViewPosition.Top:
                    MessagePresenter.SetViewParent(messageWindowPosTop.transform);
                    break;
                default:
                    MessagePresenter.SetViewParent(messageWindowPosCenter.transform);
                    break;
            }
        }

        /// <summary>
        ///  メッセージを表示
        /// </summary>
        /// <param name="message"></param>
        /// <param name="speakerName"></param>
        /// <param name="messageSpeed"></param>
        public void ShowMessage(string message, string speakerName, int messageSpeed)
        {
            MessagePresenter.ShowMessage(message, speakerName, messageSpeed);
        }

        /// <summary>
        ///  背景オブジェクトを追加
        /// </summary>
        /// <param name="imageObj"></param>
        /// <returns></returns>
        public void AddImage(GameObject imageObj)
        {
            imageObj.transform.SetParent(backgroundRoot.transform);
        }

        /// <summary>
        ///  立ち絵オブジェクトを追加
        /// </summary>
        /// <param name="standObj"></param>
        /// <param name="position"></param>
        public void AddStand(GameObject standObj, EScenarioStandPosition position)
        {
            standObj.transform.SetParent(standImageRoot.transform);
            _standImages[position] = standObj;
        }

        /// <summary>
        ///  指定した位置の立ち絵オブジェクトを取得する
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public GameObject GetStandObj(EScenarioStandPosition position)
        {
            return _standImages[position];
        }

        /// <summary>
        ///  指定した位置の立ち絵オブジェクトを消去する
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public void RemoveStand(EScenarioStandPosition position)
        {
            Destroy(_standImages[position].gameObject);
        }

        /// <summary>
        ///  画面全体を覆うフェード画像を取得する
        /// </summary>
        /// <returns></returns>
        public Image GetFadeImage()
        {
            return fadeImage.GetComponent<Image>();
        }
    }
}
