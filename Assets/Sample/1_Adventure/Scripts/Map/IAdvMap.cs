using System;
using UnityEngine;

namespace Sample._1_Adventure.Scripts.Map
{
    public abstract class IAdvMap : MonoBehaviour
    {
        protected IObserver<int> onMapChangeStream;

        /// <summary>
        /// マップ番号を取得する
        /// </summary>
        /// <returns></returns>
        public abstract int GetMapNumber();

        /// <summary>
        /// 初期化処理
        /// </summary>
        public abstract void Initialize(IObserver<int> mapChangeStream);

        /// <summary>
        /// マップ開始（表示）前の処理
        /// </summary>
        public void OnBeforeStart()
        {
            UpdateSymbol();
        }

        /// <summary>
        /// マップに表示しているシンボルの状態を更新する
        /// </summary>
        protected abstract void UpdateSymbol();

        /// <summary>
        /// マップ開始時の処理
        /// </summary>
        public abstract void OnStart();

        /// <summary>
        /// マップを表示する
        /// </summary>
        public void ShowMap()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// マップを非表示にする
        /// </summary>
        public void HideMap()
        {
            gameObject.SetActive(false);
        }
    }
}