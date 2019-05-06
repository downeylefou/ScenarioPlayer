using System.Collections.Generic;
using System.IO;
using GubGub.Scripts.Enum;
using UniRx.Async;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace GubGub.Scripts.Lib
{
    /// <summary>
    ///  リソースマネージャー
    /// </summary>
    public static class ResourceManager
    {
        private static EResourceLoadType ResourceLoadType => ResourceLoadSetting.ResourceLoadType;

        /// <summary>
        /// 読み込み済みアセットバンドルのリスト
        /// </summary>
        public static readonly Dictionary<string, AssetBundle> LoadedAssetBundles = 
            new Dictionary<string, AssetBundle>();

        
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
        /// リソースの一括読み込みを行う
        /// </summary>
        /// <param name="resourceNames"></param>
        public static async UniTask StartBulkLoad(Dictionary<string, EResourceType> resourceNames)
        {
            var loadFileList = new List<string>();
            
            foreach (var pair in resourceNames)
            {
                var pathWithPrefixAdded = 
                    ResourcePathUtility.GetResourcePathWithPrefix(pair.Key, pair.Value);
                if (!ContainLoadedAssetBundles(pathWithPrefixAdded))
                {
                    loadFileList.Add(pathWithPrefixAdded);
                }
            }
            
            // アセットバンドルの読み込みを並列実行して待機する
            var loadTasks = new List<UniTask<bool>>();
            foreach (var filePath in loadFileList)
            {
                loadTasks.Add(LoadAssetBundleAsync(filePath));
            }

            await UniTask.WhenAll(loadTasks);
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
                return await GetOrLoadAssetBundleAsync<T>(filePath);
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
        /// 非同期でアセットバンドルの読み込みを行う
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static async UniTask<bool> LoadAssetBundleAsync(string filePath)
        {
            // アセットバンドル読み込み用のパスに変更してリクエストを行う
            var request = GetAssetBundleRequest(
                ResourcePathUtility.GetAssetBundleRequestPath(filePath));
            await request;

            if (request.Result.isHttpError || request.Result.isNetworkError)
            {
                Debug.LogError(request.Result.error + ":" + filePath);
                return false;
            }
            
            // 読み込み済みリストに追加
            var assetBundle = DownloadHandlerAssetBundle.GetContent(request.Result);
            AddLoadedAssetBundles(assetBundle, filePath);
            
            return true;
        }
        
        /// <summary>
        /// 非同期でアセットバンドルを読み込み、リソースを取得する
        /// </summary>
        /// <param name="filePath"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static async UniTask<T> GetOrLoadAssetBundleAsync<T>(string filePath) where T : Object
        {
            // すでにアセットバンドルが読み込み済みなら、そちらから取得する
            if (ContainLoadedAssetBundles(filePath))
            {
                return LoadAssetFromAssetBundle<T>(filePath);
            }

            // 読み込んでからリソースを取得する
            var isLoadSuccess = await LoadAssetBundleAsync(filePath);
            return (isLoadSuccess)? LoadAssetFromAssetBundle<T>(filePath) : null;
        }

        /// <summary>
        /// 読み込み済みアセットバンドルから、リソースをロードして取得する
        /// </summary>
        /// <param name="filePath"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static T LoadAssetFromAssetBundle<T>(string filePath)where T : Object
        {
            // 読み込み済みリストから取得
            if (!LoadedAssetBundles.TryGetValue(filePath, out var assetBundle))
            {
                Debug.LogError("can't find asset from loaded AssetBundles:" + filePath);
                return null;
            }
            
            // パスからファイル名だけ取得
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var asset = assetBundle.LoadAsset<T>(fileName);
                
            if (asset == null)
            {
                Debug.LogError("can't load asset:" + filePath);
            }

            return asset;
        }

        /// <summary>
        /// 読み込み済みアセットバンドルリストに追加する
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="filePath"></param>
        private static void AddLoadedAssetBundles(AssetBundle bundle, string filePath)
        {
            LoadedAssetBundles.Add(filePath, bundle);
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
        
        /// <summary>
        /// 読み込み済みアセットバンドルリストに存在するファイル名か
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static bool ContainLoadedAssetBundles(string fileName)
        {
            return LoadedAssetBundles.ContainsKey(fileName);
        }
    }
}
