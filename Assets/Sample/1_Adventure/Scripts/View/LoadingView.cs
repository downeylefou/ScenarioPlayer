using DG.Tweening;
using GubGub.Scripts.Lib;
using UniRx.Async;
using UnityEngine;
using UnityEngine.UI;

namespace Sample._1_Adventure.Scripts.View
{
    /// <summary>
    /// ローディングなどで画面を覆うビュー
    /// </summary>
    public class LoadingView : MonoBehaviour
    {
        [SerializeField] private Image left;
        [SerializeField] private Image right;
        [SerializeField] private CanvasGroup canvasGroup;

        private const float Duration = 1.0f;


        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Initialize()
        {
            ChangeBlockRayCasts(false);
        }

        /// <summary>
        /// ローディングを表示する
        /// </summary>
        public async UniTask Show()
        {
            ChangeBlockRayCasts(true);

            var sequence = DOTween.Sequence();
            await sequence
                .Append(left.rectTransform.DOLocalMoveX(0, Duration))
                .Join(right.rectTransform.DOLocalMoveX(0, Duration))
                .Play();
        }

        /// <summary>
        /// ローディングを非表示にする
        /// </summary>
        public async UniTask Hide()
        {
            var moveValue = left.rectTransform.sizeDelta.x;

            var sequence = DOTween.Sequence();
            await sequence
                .Append(left.rectTransform.DOLocalMoveX(-moveValue, Duration))
                .Join(right.rectTransform.DOLocalMoveX(moveValue, Duration))
                .OnComplete(() => ChangeBlockRayCasts(false))
                .Play();
        }

        public async UniTask Wait(float duration = 1f)
        {
            await DOVirtual.DelayedCall(duration, () => { }).Play();
        }
        
        private void ChangeBlockRayCasts(bool isBlock)
        {
            canvasGroup.blocksRaycasts = isBlock;
        }
    }
}