using System;
using System.IO;
using System.Text;
using GubGub.Scripts.Enum;
using UniRx.Async;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace GubGub.Scripts.Lib
{
    /// <summary>
    ///  リソースマネージャー
    /// TODO リソースは全て非同期で読み込んでいるが、シーン開始前に必要なものを読み込んでキャッシュしておく
    /// </summary>
    public static class ResourceManager
    {
        private static EResourceLoadType ResourceLoadType => ResourceLoadSetting.ResourceLoadType;

        private static readonly StringBuilder _stringBuilder = new StringBuilder(300);

        /// <summary>
        ///  Spriteの読み込み
        ///  Texture/フォルダから取得する
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static async UniTask<Sprite> LoadSprite(string filePath)
        {
            return await LoadAssetAsync<Sprite>(filePath);
        }
    
        /// <summary>
        ///  AudioClipの読み込み
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static async UniTask<AudioClip> LoadSound(string filePath)
        {
            return await LoadAssetAsync<AudioClip>(filePath);
        }
        
        /// <summary>
        /// テキストアセットの読み込み
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static async UniTask<TextAsset> LoadText(string filePath)
        {
            return await LoadAssetAsync<TextAsset>(filePath);
        }

        /// <summary>
        /// 読み込み方法に対応したリソースの非同期読み込みを行う
        /// </summary>
        /// <param name="filePath"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static async UniTask<T> LoadAssetAsync<T>(string filePath)where T :  Object
        {
            if (ResourceLoadType == EResourceLoadType.StreamingAssets)
            {
                return await LoadAssetBundleAsync<T>(filePath);
            }
            else
            {
                return await LoadResourcesAssetAsync<T>(filePath);
            }
        }

        /// <summary>
        /// Resourcesからリソースの非同期読み込みを行う
        /// </summary>
        /// <param name="filePath"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static async UniTask<T> LoadResourcesAssetAsync<T>(string filePath) where T :  Object
        {
            // パスから拡張子を除く
            var fileName = GetPathWithoutExtension(filePath);
            
            var request = Resources.LoadAsync<T>(fileName);
            await request;

            if (request.asset == null)
            {
                Debug.LogError("asset not found : " + filePath);
            }
            return request.asset as T;
        }

        /// <summary>
        /// アセットバンドルから非同期でリソースの読み込みを行う
        /// </summary>
        /// <param name="filePath"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static async UniTask<T> LoadAssetBundleAsync<T>(string filePath) where T : Object
        {
            // アセットバンドル名に固定のサフィックスを追加
            // TODO: ユーザーがインスペクタから設定可能にする
            if (filePath.IndexOf(".asset", StringComparison.Ordinal) == -1)
            {
                filePath += ".asset";
            }
            
            var request = GetAssetBundleRequest(GetFilePathForTargetPlatform(filePath));
            await request;

            if (request.Result.isHttpError || request.Result.isNetworkError)
            {
                Debug.LogError(request.Result.error + ":" + filePath);
            }
            else
            {
                var assetBundle = DownloadHandlerAssetBundle.GetContent(request.Result);

                // パスからファイル名だけ取得
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var asset = assetBundle.LoadAsset<T>(fileName);
                
                if (asset == null)
                {
                    Debug.LogError("can't load asset:" + filePath);
                }

                return asset;
            }

            return null;
        }

        /// <summary>
        /// アセットバンドル取得のリクエストを行う
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static async UniTask<UnityWebRequest> GetAssetBundleRequest(string filePath)
        {
            var request = UnityWebRequestAssetBundle.GetAssetBundle(filePath);

            return await request.SendWebRequest();
        }
        
        /// <summary>
        /// プラットフォームごとのパスを作成
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static string GetFilePathForTargetPlatform(string filePath)
        {
            _stringBuilder.Clear();

            // プラットフォームごとの StreamingAssetsのパス
            if (ResourceLoadType == EResourceLoadType.StreamingAssets)
            {
                #if UNITY_EDITOR || WebPlayer
                _stringBuilder.Append(Application.dataPath);
                _stringBuilder.Append("/StreamingAssets/");
                #elif Android
                // TODO: 未検証
                _stringBuilder.Append("jar:file://");
                _stringBuilder.Append(Application.dataPath);
                _stringBuilder.Append("!/assets/");
                #endif
            }
            
            // アセットバンドルのターゲット名
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                _stringBuilder.Append("windows standalone(64-bit)/");
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                _stringBuilder.Append("android");
            }
            _stringBuilder.Append(filePath);

            return _stringBuilder.ToString();
        }
        
        /// <summary>
        /// パスから拡張子を削除する
        /// </summary>
        private static string GetPathWithoutExtension(string path)
        {
            var extension = Path.GetExtension(path);
            if (string.IsNullOrEmpty(extension))
            {
                return path;
            }
            return path.Replace( extension, string.Empty );
        }
    }
}
