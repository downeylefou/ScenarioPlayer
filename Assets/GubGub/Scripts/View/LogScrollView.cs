using System.Collections.Generic;
using FancyScrollView;
using GubGub.Scripts.Data;
using GubGub.Scripts.View.Interface;
using UnityEngine;
using UnityEngine.UI;

namespace GubGub.Scripts.View
{
    /// <summary>
    /// バックログのスクロールビュー
    /// </summary>
    public class LogScrollView : FancyScrollView<ScenarioLogData>, ILogScrollView
    {
        [SerializeField] private Scroller scroller = default;
        [SerializeField] private GameObject cellPrefab = default;
        
        protected override GameObject CellPrefab => cellPrefab;

        private void Start()
        {
            scroller.OnValueChanged(base.UpdatePosition);
            UpdateData(new List<ScenarioLogData>());
        }

        public void UpdateData(IList<ScenarioLogData> items)
        {
            base.UpdateContents(items);
            scroller.SetTotalCount(items.Count);
        }

        public void Jump(int index)
        {
            scroller.JumpTo(index);
        }
    }
}

