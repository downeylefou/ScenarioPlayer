using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GubGub.Scripts.Data;
using GubGub.Scripts.Enum;
using GubGub.Scripts.View;
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
        [SerializeField] private BackLogPresenter backLogPresenter;

        [SerializeField] private ScenarioMessagePresenter scenarioMessagePresenter;

        [SerializeField] private ScenarioSelectionPresenter selectionPresenter;

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
        /// マウスホイールを行った
        /// </summary>
        public Subject<float> onMouseWheel = new Subject<float>();

        /// <summary>
        ///  メッセージビューのタイプ
        /// </summary>
        public EScenarioMessageViewType MessageViewType { get; set; }


        /// <summary>
        ///  立ち位置と、そこに表示されている立ち絵のリスト
        /// </summary>
        private readonly Dictionary<EScenarioStandPosition, ImageView> _standImages =
            new Dictionary<EScenarioStandPosition, ImageView>()
            {
                {EScenarioStandPosition.Left, null}, {EScenarioStandPosition.Center, null},
                {EScenarioStandPosition.Right, null}
            };

        /// <summary>
        /// メッセージビューの管理クラス
        /// </summary>
        public ScenarioMessagePresenter MessagePresenter => scenarioMessagePresenter;
        
        /// <summary>
        /// バックログビューの管理クラス
        /// </summary>
        public BackLogPresenter BackLogPresenter => backLogPresenter;

        /// <summary>
        /// 選択肢ビューの管理クラス
        /// </summary>
        public ScenarioSelectionPresenter SelectionPresenter => selectionPresenter;


        public async Task Initialize()
        {
            await InitializeMessageView();
            AddEventListeners();
        }

        private void AddEventListeners()
        {
            clickArea.GetComponent<Image>().OnPointerClickAsObservable().Subscribe(onAnyClick).AddTo(this);
            
            clickArea.GetComponent<Image>().UpdateAsObservable()
                .Select(_ => Input.GetAxis("Mouse ScrollWheel"))
                .Where(axis => Math.Abs(axis) > 0)
                .Subscribe(onMouseWheel).AddTo(this);
        }

        /// <summary>
        ///  メッセージビューを初期化
        /// </summary>
        /// <returns></returns>
        private async Task InitializeMessageView()
        {
            await MessagePresenter.Initialize();
            
            // メッセージビューを初期位置に配置
            ChangeMessageViewPosition(EScenarioMessageViewPosition.Bottom);
        }

        #region public method
        
        /// <summary>
        /// ビューを表示する
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
        }
        
        /// <summary>
        /// ビューを非表示にする
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        
        /// <summary>
        /// 表示を初期化する
        /// </summary>
        public void ResetView()
        {
            MessagePresenter.ClearText();
            SelectionPresenter.Clear();
            
            RemoveAllStand();
            RemoveImage();
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
        /// <param name="messageData"></param>
        public void ShowMessage(ScenarioMessageData messageData)
        {
            MessagePresenter.ShowMessage(messageData);
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
        /// <param name="imageView"></param>
        /// <param name="position"></param>
        public void AddStand(ImageView imageView, EScenarioStandPosition position)
        {
            imageView.gameObject.transform.SetParent(standImageRoot.transform);
            _standImages[position] = imageView;
        }

        /// <summary>
        ///  指定した位置の立ち絵オブジェクトを取得する
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public ImageView GetStandImageView(EScenarioStandPosition position)
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
            Destroy(_standImages[position]?.gameObject);
        }

        /// <summary>
        ///  画面全体を覆うフェード画像を取得する
        /// </summary>
        /// <returns></returns>
        public Image GetFadeImage()
        {
            return fadeImage.GetComponent<Image>();
        }
        
        #endregion
        
        /// <summary>
        /// 全ての立ち絵を削除
        /// </summary>
        private void RemoveAllStand()
        {
            RemoveStand(EScenarioStandPosition.Left);
            RemoveStand(EScenarioStandPosition.Center);
            RemoveStand(EScenarioStandPosition.Right);
        }

        /// <summary>
        /// 全ての背景を削除
        /// </summary>
        private void RemoveImage()
        {
            foreach (var child in backgroundRoot.transform.GetComponentsInChildren<Transform>())
            {
                if (child != backgroundRoot.transform)
                {
                    Destroy(child.gameObject);
                }
            }
            
            backgroundRoot.transform.DetachChildren();
        }
    }
}
