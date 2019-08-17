using GubGub.Scripts;
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
