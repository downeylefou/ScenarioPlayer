using UnityEngine;
using UnityEngine.UI;

namespace Sample._1_Adventure.Scripts.Map
{
    /// <summary>
    /// ゲームシーンの背景を表示する
    /// </summary>
    public class BackgroundPresenter : MonoBehaviour
    {
        [SerializeField] private Image map1;
        [SerializeField] private Image map2;

        public void ChangeBackground(int mapNumber)
        {
            map1.gameObject.SetActive(mapNumber == 1);
            map2.gameObject.SetActive(mapNumber == 2);
        }
    }
}
