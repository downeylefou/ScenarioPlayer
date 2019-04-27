using UnityEngine;
using UnityEngine.UI;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DG.Tweening;

namespace GubGub.Scripts.Lib
{
	/// <summary>
	/// 基本クラスの拡張メソッド用クラス
	/// </summary>
	public static class ExtensionMethods
	{
		/// <summary>
		/// Image.Colorのアルファを変更
		/// </summary>
		/// <param name="image"></param>
		/// <param name="alpha"></param>
		public static void SetAlpha(this Image image, float alpha)
		{
			var color = image.color;
			color = new Color(color.r, color.g, color.b, alpha);
			image.color = color;
		}
		
		/// <summary>
		/// RectTransformの localScaleを変更
		/// </summary>
		/// <param name="tr"></param>
		/// <param name="scale"></param>
		public static void SetScale(this RectTransform tr, Vector3 scale)
		{
			tr.localScale = scale;
		}
		
		/// <summary>
		/// TransformのX,Y座標を設定
		/// </summary>
		public static void SetXy(this Transform tr, float x, float y)
		{
			var pos = tr.position;
			pos.x = x;
			pos.y = y;
			tr.localPosition = pos;
		}
		
		/// <summary>
		///  TweenerをAwaitableにする
		/// </summary>
		/// <param name="self"></param>
		/// <returns></returns>
		public static TaskAwaiter<Tween> GetAwaiter( this Tween self )
		{
			var source = new TaskCompletionSource<Tween>();

			TweenCallback onComplete = null;
			onComplete = () =>
			{
				self.onComplete -= onComplete;
				source.SetResult( self );
			};
			self.onComplete += onComplete;

			return source.Task.GetAwaiter();
		}
		
		/// <summary>
		/// X,Y座標のVector2を取得する
		/// </summary>
		public static Vector2 Xy(this Vector3 v)
		{
			return new Vector2(v.x, v.y);
		}
	}
}
