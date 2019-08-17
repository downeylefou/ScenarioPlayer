using GubGub.Scripts;
using Sample._1_Adventure.Scripts.Data;
using Sample._1_Adventure.Scripts.Util;
using UnityEngine;

namespace Sample._1_Adventure.Scripts
{
    /// <summary>
    /// ゲームシーンクラス
    /// </summary>
    public class GameScene : MonoBehaviour
    {
        public void StartScene()
        {
            AdvScenarioUtil.PlayLabel(AdvScenarioLabel.abcd);
            
        }
    
        // Update is called once per frame
        void Update()
        {
            
        }
    }
}
