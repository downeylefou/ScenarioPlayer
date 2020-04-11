using GubGub.Scripts.View.Interface;
using UnityEngine;

namespace GubGub.Scripts.View
{
    /// <summary>
    /// 顔ウィンドウクラス
    /// </summary>
    public class ScenarioFaceWindow : MonoBehaviour, IFaceWindow
    {
        [SerializeField] private GameObject faceRoot;
        [SerializeField] private CanvasGroup canvasGroup;

        private Vector3 tempLocalPosition;
        
        
        /// <summary>
        /// ウィンドウに画像を表示する
        /// </summary>
        /// <param name="imageView"></param>
        public void UpdateImageView(ImageView imageView)
        {
            DestroyChildren();

            tempLocalPosition = imageView.RectTransform.localPosition;

            imageView.transform.parent = faceRoot.transform;
            imageView.RectTransform.anchorMin = Vector2.zero;
            imageView.RectTransform.anchorMax = Vector2.one;
            imageView.RectTransform.anchoredPosition3D = Vector3.zero;
            imageView.RectTransform.offsetMax = Vector2.zero;
            imageView.RectTransform.offsetMin = Vector2.zero;

            imageView.RectTransform.localPosition = tempLocalPosition;
        }

        public void Clear()
        {
            DestroyChildren();
        }

        public void Show()
        {
            canvasGroup.alpha = 1;
        }

        public void Hide()
        {
            canvasGroup.alpha = 0;
        }

        private void DestroyChildren()
        {
            foreach (Transform child in faceRoot.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}