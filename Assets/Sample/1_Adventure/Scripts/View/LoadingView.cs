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

        private const float Duration = 1.0f;


        /// <summary>
        /// ローディングを表示する
        /// </summary>
        public async UniTask Show()
        {
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
                .Play();
        }

        public async UniTask Wait(float duration = 1f)
        {
            await DOVirtual.DelayedCall(duration, () => { }).Play();
        }
    }
}