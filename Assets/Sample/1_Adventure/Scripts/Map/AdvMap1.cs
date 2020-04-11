using System;
using Sample._1_Adventure.Scripts.Data;
using Sample._1_Adventure.Scripts.Util;
using Sample._1_Adventure.Scripts.View;
using UnityEngine;

namespace Sample._1_Adventure.Scripts.Map
{
    /// <summary>
    /// ゲームシーン、1つめのマップ
    /// </summary>
    public class AdvMap1 : IAdvMap
    {
        [SerializeField] private AdvSymbolView mashRoom;
        [SerializeField] private AdvSymbolView mother;
        [SerializeField] private AdvSymbolView downButton;

        public override int GetMapNumber() => 1;

        public override void Initialize(IObserver<int> mapChangeStream)
        {
            onMapChangeStream = mapChangeStream;

            mashRoom.Button.onClick.AddListener(OnClickMashRoom);
            mother.Button.onClick.AddListener(OnClickMother);
            downButton.Button.onClick.AddListener(OnDownButton);
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

        protected override void UpdateSymbol()
        {
            mother.gameObject.SetActive(AdvParameter._1_isEndStartLabel);
            mashRoom.gameObject.SetActive(!AdvParameter._1_isGetMashRoom && AdvParameter._1_isEndStartLabel);
            downButton.gameObject.SetActive(AdvParameter._1_isEndStartLabel);
        }

        /// <summary>
        /// 下ボタンをクリックした
        /// </summary>
        private void OnDownButton()
        {
            if (AdvParameter._1_isGetMashRoom)
            {
                onMapChangeStream.OnNext(2);
            }
            else
            {
                HideMap();
                AdvScenarioUtil.PlayLabel(AdvScenarioLabel._1_mother_warning, ShowMap);
            }
        }

        /// <summary>
        /// キノコをクリックした
        /// </summary>
        private void OnClickMashRoom()
        {
            // キノコの所持数を増加
            AdvScenarioUtil.AddedMashRoomCount();

            // 再生停止時に表示を更新
            AdvScenarioUtil.PlayLabel(AdvScenarioLabel._1_mashRoom, () =>
            {
                AdvParameter._1_isGetMashRoom = true;
                UpdateSymbol();
            });
        }

        /// <summary>
        /// たぬき母をクリックした
        /// </summary>
        private void OnClickMother()
        {
            HideMap();
            AdvScenarioUtil.PlayLabel(AdvScenarioLabel._1_mother_count, ShowMap);
        }
    }
}