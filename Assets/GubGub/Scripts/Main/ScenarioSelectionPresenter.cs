using System.Collections.Generic;
using GubGub.Scripts.Command;
using GubGub.Scripts.View;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;

namespace GubGub.Scripts.Main
{
    /// <summary>
    /// シナリオの選択肢のプレゼンタ
    /// </summary>
    public class ScenarioSelectionPresenter : MonoBehaviour
    {
        /// <summary>
        /// 選択肢の最大数
        /// </summary>
        private static readonly int _maxSelectionNum = 8;
        
        /// <summary>
        /// 選択肢がクリックされたことを通知する
        /// </summary>
        public Subject<string> onSelect = new Subject<string>();
        
        /// <summary>
        /// ビューごとの余白
        /// </summary>
        private const float MarginY = 10;
        
        /// <summary>
        /// 表示する選択肢のもととなるプレハブ
        /// </summary>
        [SerializeField] private GameObject selectionPrefab;

        /// <summary>
        /// 選択肢ビューのリスト
        /// </summary>
        private readonly List<ScenarioSelectionView> _viewList = new List<ScenarioSelectionView>();

        private float _defaultY;

        private RectTransform _cacheTransform;
        private RectTransform RectTransform =>
            _cacheTransform ? _cacheTransform : _cacheTransform = GetComponent<RectTransform>();

        private void Awake()
        {
            _defaultY = RectTransform.localPosition.y;
        }
        
        /// <summary>
        /// 表示を初期化する
        /// </summary>
        public void Clear()
        {
            foreach (var view in _viewList)
            {
                Destroy(view.gameObject);
            }
            _viewList.Clear();
            
            // Y座標を戻す
            var pos = RectTransform.localPosition;

            pos.y = _defaultY;
            RectTransform.localPosition = pos;
        }
        
        /// <summary>
        /// 選択肢ビューを追加する
        /// </summary>
        /// <param name="command"></param>
        public void AddSelection(SelectionCommand command)
        {
            // 表示最大数に達していたら追加しない
            if (_viewList.Count == _maxSelectionNum)
            {
                return;
            }
            
            var view = Instantiate(selectionPrefab, transform)
                .GetComponent<ScenarioSelectionView>();

            view.Initialize(command.SelectionText, command.LabelName, OnClick);
            _viewList.Add(view);

            // 座標を設定
            var viewRect = view.GetComponent<RectTransform>();
            var pos = viewRect.localPosition;
            pos.y -= _viewList.Count * (viewRect.sizeDelta.y + MarginY );
            viewRect.localPosition = pos;

            // クリック時のコールバック
            // ビューから渡されるラベル名を通知する
            void OnClick(string labelName)
            {
                onSelect.OnNext(labelName);
            }

            AdjustPosition();
        }
        
        /// <summary>
        /// 表示状態を変更する
        /// </summary>
        /// <param name="isVisible"></param>
        public void ChangeVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        /// <summary>
        /// 全体の座標を調整
        /// </summary>
        private void AdjustPosition()
        {
            var pos = RectTransform.localPosition;
            var viewHeight = (_viewList.Count > 0)?
                _viewList[0].GetComponent<RectTransform>().sizeDelta.y : 0;

            pos.y = _defaultY + (_viewList.Count - 1) * (viewHeight + MarginY ) / 2;
            RectTransform.localPosition = pos;
        }
    }
}