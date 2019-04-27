using System.Threading.Tasks;
using UniRx.Async;
using UnityEngine;

namespace GubGub.Scripts.Lib
{
    /// <summary>
    ///  リソースマネージャー
    /// TODO リソースは全て非同期で読み込んでいるが、シーン開始前に必要なものを読み込んでキャッシュしておく
    /// </summary>
    public class ResourceManager
    {
        /// <summary>
        ///  Spriteの読み込み
        ///  Texture/フォルダから取得する
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static async Task<Sprite> LoadSprite(string filePath)
        {
            const string prefix = "texture/";
            return await LoadAssetAsync<Sprite>(prefix + filePath);
        }
    
        /// <summary>
        ///  AudioClipの読み込み
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static async Task<AudioClip> LoadSound(string filePath)
        {
            const string prefix = "sound/";
            return await LoadAssetAsync<AudioClip>(prefix + filePath);
        }
        
        /// <summary>
        /// テキストアセットの読み込み
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static async Task<TextAsset> LoadText(string filePath)
        {
            const string prefix = "scenario/";
            return await LoadAssetAsync<TextAsset>(prefix + filePath);
        }
        
        /// <summary>
        /// リソースの非同期読み込みを行う
        /// </summary>
        /// <param name="filePath"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static async Task<T> LoadAssetAsync<T>(string filePath) where T :  Object
        {
            var request = Resources.LoadAsync<T>(filePath);
            await request;

            if (request.asset == null)
            {
                Debug.LogError("asset not found : " + filePath);
            }
            return request.asset as T;
        }
    }
}
