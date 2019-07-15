using System.Collections;
using System.Collections.Generic;
using Coffee.UIExtensions;
using GubGub.Scripts.Lib;
using UnityEngine;
using UnityEngine.UI;

namespace GubGub.Scripts.View
{
    /// <summary>
    /// シナリオビューで表示する画像クラス
    /// </summary>
    public class ImageView : MonoBehaviour
    {
        public Image Image { get; private set; }

        private UIEffect _uiEffect;
        private UIFlip _uiFlip;

        #region static method

        /// <summary>
        /// ImageViewオブジェクトを生成する
        /// </summary>
        /// <returns></returns>
        public static ImageView Instantiate(string objectName = "imageView")
        {
            var view = new GameObject {name = objectName};

            var imageView = view.AddComponent<ImageView>();
            imageView.Image = view.AddComponent<Image>();
            imageView._uiEffect = view.AddComponent<UIEffect>();
            imageView._uiEffect.material =
                Resources.Load<Material>("ScenarioPlayer/Material/UI-Effect");
            imageView._uiFlip = view.AddComponent<UIFlip>();
            return imageView;
        }

        #endregion

        /// <summary>
        /// 画像のSpriteを設定する
        /// </summary>
        /// <param name="sprite"></param>
        public void SetSprite(Sprite sprite)
        {
            Image.sprite = sprite;
        }

        /// <summary>
        /// アルファを設定する
        /// </summary>
        /// <param name="alpha"></param>
        public void SetAlpha(float alpha)
        {
            Image.SetAlpha(alpha);
        }

        /// <summary>
        /// 画像の反転状態を設定する
        /// </summary>
        /// <param name="isReverse"></param>
        public void SetReverse(bool isReverse)
        {
            _uiFlip.horizontal = isReverse;
        }
    }
}
