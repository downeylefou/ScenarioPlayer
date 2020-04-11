using Sample._1_Adventure.Scripts.Data;
using Sample._1_Adventure.Scripts.Map;
using Sample._1_Adventure.Scripts.Util;
using UniRx.Async;
using UnityEngine;

namespace Sample._1_Adventure.Scripts.Scene
{
    /// <summary>
    /// ゲームシーンクラス
    /// </summary>
    public class GameScene : MonoBehaviour
    {
        [SerializeField] private AdvMapPresenter mapPresenter;

        public void Initialize()
        {
            mapPresenter.Initialize();

            InitializeScenarioParameter();
        }

        public async UniTask StartScene()
        {
            await mapPresenter.StartMap();
        }

        /// <summary>
        /// シナリオで使用するパラメータを初期化する
        /// </summary>
        private void InitializeScenarioParameter()
        {
            AdvScenarioUtil.SetParameter(AdvScenarioParameter.MashRoomCount, 0);
            AdvScenarioUtil.SetParameter(AdvScenarioParameter.AcornCount, 0);
        }
    }
}