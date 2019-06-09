using System.Collections.Generic;
using GubGub.Scripts.Command;
using GubGub.Scripts.View;
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
        /// 表示する選択肢のもととなるプレハブ
        /// </summary>
        [SerializeField] private GameObject selectionPrefab;

        /// <summary>
        /// 選択肢がクリックされたことを通知する
        /// </summary>
        public Subject<string> onSelect = new Subject<string>();
        
        /// <summary>
        /// 選択肢ビューのリスト
        /// </summary>
        private readonly List<ScenarioSelectionView> _viewList = new List<ScenarioSelectionView>();
        
        
        public void Initialize()
        {
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
        }
                
        /// <summary>
        /// 選択肢ビューを追加する
        /// </summary>
        /// <param name="command"></param>
        public void AddSelection(SelectionCommand command)
        {
            var view = Instantiate(selectionPrefab, transform)
                .GetComponent<ScenarioSelectionView>();

            view.Initialize(command.SelectionText, command.LabelName, OnClick);
            
            _viewList.Add(view);

            // クリック時のコールバック
            // ビューから渡されるラベル名を通知する
            void OnClick(string labelName)
            {
                onSelect.OnNext(labelName);
            }
        }
        
    }
}