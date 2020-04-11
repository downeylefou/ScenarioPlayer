using Coffee.UIExtensions;
using UnityEngine;
using UnityEngine.UI;

namespace Sample._1_Adventure.Scripts.View
{
    /// <summary>
    /// マップ上に表示されるシンボル
    /// <para>共通コンポーネントをアタッチする</para>
    /// </summary>
    public class AdvSymbolView : MonoBehaviour
    {
        private UIShadow uiShadow;

        public Button Button => m_button;

        private Button m_button;

        private void Awake()
        {
            if (m_button == null)
            {
                m_button = gameObject.GetComponent<Button>();
            }
            
            if (uiShadow == null)
            {
                uiShadow = gameObject.AddComponent<UIShadow>();
                uiShadow.effectDistance = new Vector2(5, 5);
                uiShadow.style = ShadowStyle.Outline8;
            }
        }
    }
}