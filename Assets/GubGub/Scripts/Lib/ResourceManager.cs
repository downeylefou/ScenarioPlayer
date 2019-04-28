using System.Threading.Tasks;
using UniRx.Async;
using UnityEngine;

namespace GubGub.Scripts.Lib
{
    /// <summary>
    ///  リソースマネージャー
    /// TODO リソースは全て非同期で読み込んでいるが、シーン開始前に必要なものを読み込んでキャッシュしておく
    /// </summary>
    public static class ResourceManager
    {
        /// <summary>
        ///  Spriteの読み込み
        ///  Texture/フォルダから取得する
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static async Task<Sprite> LoadSprite(string filePath)
        {
            return await LoadAssetAsync<Sprite>(filePath);
        }
    
        /// <summary>
        ///  AudioClipの読み込み
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static async Task<AudioClip> LoadSound(string filePath)
        {
            return await LoadAssetAsync<AudioClip>(filePath);
        }
        
        /// <summary>
        /// テキストアセットの読み込み
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static async Task<TextAsset> LoadText(string filePath)
        {
            return await LoadAssetAsync<TextAsset>(filePath);
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
