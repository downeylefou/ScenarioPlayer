using System;
using Sample._1_Adventure.Scripts.Data;
using Sample._1_Adventure.Scripts.Util;
using Sample._1_Adventure.Scripts.View;
using UnityEngine;

namespace Sample._1_Adventure.Scripts.Map
{
    /// <summary>
    /// ゲームシーン、2つめのマップ
    /// </summary>
    public class AdvMap2 : IAdvMap
    {
        [SerializeField] private AdvSymbolView acorn1;
        [SerializeField] private AdvSymbolView acorn2;
        [SerializeField] private AdvSymbolView spiderWeb;
        [SerializeField] private AdvSymbolView upButton;
        [SerializeField] private AdvSymbolView rightButton;

        public override int GetMapNumber() => 2;

        public override void Initialize(IObserver<int> mapChangeStream)
        {
            onMapChangeStream = mapChangeStream;

            acorn1.Button.onClick.AddListener(OnClickAcorn1);
            acorn2.Button.onClick.AddListener(OnClickAcorn2);
            spiderWeb.Button.onClick.AddListener(OnClickSpiderWeb);
            upButton.Button.onClick.AddListener(OnUpButton);
            rightButton.Button.onClick.AddListener(OnRightButton);
        }

        public override void OnStart()
        {
            if (!AdvParameter._1_isEndStartLabel)
            {
                // 初回遷移時のシナリオを再生
                AdvScenarioUtil.PlayLabel(AdvScenarioLabel._1_start, () =>
                {
                    AdvParameter._1_isEndStartLabel = true;
                    UpdateSymbol();
                });
            }
        }

        /// <summary>
        /// マップに表示しているシンボルの状態を更新する
        /// </summary>
        protected override void UpdateSymbol()
        {
            acorn1.gameObject.SetActive(!AdvParameter._2_isGetAcorn1);
            acorn2.gameObject.SetActive(!AdvParameter._2_isGetAcorn2);
//            spiderWeb.gameObject.SetActive(!AdvParameter._1_isGetMashRoom);
        }

        /// <summary>
        /// 上ボタンをクリックした
        /// </summary>
        private void OnUpButton()
        {
            onMapChangeStream.OnNext(1);
        }
        
        /// <summary>
        /// 右ボタンをクリックした
        /// </summary>
        private void OnRightButton()
        {
            onMapChangeStream.OnNext(3);
        }

        /// <summary>
        /// ドングリ1をクリックした
        /// </summary>
        private void OnClickAcorn1()
        {
            // ドングリの所持数を増加
            AdvScenarioUtil.AddedAcornCount();

            // 再生停止時に表示を更新
            AdvScenarioUtil.PlayLabel(AdvScenarioLabel._2_acorn1, () =>
            {
                AdvParameter._2_isGetAcorn1 = true;
                UpdateSymbol();
            });
        }
        
        /// <summary>
        /// ドングリ2をクリックした
        /// </summary>
        private void OnClickAcorn2()
        {
            // ドングリの所持数を増加
            AdvScenarioUtil.AddedAcornCount();

            // 再生停止時に表示を更新
            AdvScenarioUtil.PlayLabel(AdvScenarioLabel._2_acorn2, () =>
            {
                AdvParameter._2_isGetAcorn2 = true;
                UpdateSymbol();
            });
        }

        /// <summary>
        /// クモ・クモの巣をクリックした
        /// </summary>
        private void OnClickSpiderWeb()
        {
            HideMap();
            AdvScenarioUtil.PlayLabel(AdvScenarioLabel._1_mother_count, ShowMap);
        }
    }
}