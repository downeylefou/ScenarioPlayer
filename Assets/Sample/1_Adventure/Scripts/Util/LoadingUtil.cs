using Sample._1_Adventure.Scripts.View;
using UniRx.Async;

namespace Sample._1_Adventure.Scripts.Util
{
    /// <summary>
    /// ローディングのユーティリティクラス
    /// </summary>
    public static class LoadingUtil
    {
        private static LoadingView _view;

        public static void Initialize(LoadingView view)
        {
            _view = view;
        }

        /// <summary>
        /// ローディングを表示する
        /// </summary>
        /// <returns></returns>
        public static async UniTask Show()
        {
            await _view.Show();
        }

        /// <summary>
        /// ローディングを非表示にする
        /// </summary>
        /// <returns></returns>
        public static async UniTask Hide()
        {
            await _view.Hide();
        }
        
        public static async UniTask Wait(float duration = 1f)
        {
            await _view.Wait(duration);
        }
    }
}