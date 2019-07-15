using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GubGub.Scripts.Enum;
using UnityEngine;

namespace GubGub.Scripts
{
    /// <summary>
    /// リソースパスに関するユーティリティクラス
    /// </summary>
    public static class ResourcePathUtility
    {
        private static readonly StringBuilder StringBuilder = new StringBuilder(300);
        
        private static readonly Dictionary<EResourceType, string> PrefixList =
            new Dictionary<EResourceType, string>()
            {
                {EResourceType.Background, ResourceLoadSetting.BackgroundResourcePrefix},
                {EResourceType.Stand, ResourceLoadSetting.StandResourcePrefix},
                {EResourceType.Bgm, ResourceLoadSetting.BgmResourcePrefix},
                {EResourceType.Se, ResourceLoadSetting.SeResourcePrefix},
                {EResourceType.Voice, ResourceLoadSetting.VoiceResourcePrefix},
            }; 
            
        /// <summary>
        /// アセットバンドルリクエスト用のパスを取得する
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetAssetBundleRequestPath(string filePath)
        {
            return GetFilePathForTargetPlatform(GetAdjustAssetBundleFilePath(filePath));
        }

        /// <summary>
        /// プレフィックス付きのリソースパスを取得する
        /// </summary>
        /// <returns></returns>
        public static string GetResourcePathWithPrefix(string filePath, EResourceType type)
        {
            StringBuilder.Clear();

            StringBuilder.Append(PrefixList.FirstOrDefault(data => data.Key.Equals(type)).Value);
            StringBuilder.Append(filePath);

            return StringBuilder.ToString();
        }
        
        /// <summary>
        /// リソース取得先と、プラットフォームごとのパスを作成
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static string GetFilePathForTargetPlatform(string filePath)
        {
            StringBuilder.Clear();

            if (ResourceLoadSetting.ResourceLoadType == EResourceLoadType.StreamingAssets)
            {
                StringBuilder.Append(Application.streamingAssetsPath);
                StringBuilder.Append("/");
            }
            else if (ResourceLoadSetting.ResourceLoadType == EResourceLoadType.Server)
            {
                StringBuilder.Append(ResourceLoadSetting.ServerHostUrl);
            }
            
            // アセットバンドルのターゲット名
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                StringBuilder.Append("windows standalone(64-bit)/");
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                StringBuilder.Append("android/");
            }
            StringBuilder.Append(filePath);

            return StringBuilder.ToString();
        }
        
        /// <summary>
        /// 固定のサフィックスを追加したアセットバンドル名を取得する
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static string GetAdjustAssetBundleFilePath(string filePath)
        {
            if (filePath.IndexOf(ResourceLoadSetting.AssetBundleSuffix, StringComparison.Ordinal) == -1)
            {
                return filePath + ResourceLoadSetting.AssetBundleSuffix;
            }

            return filePath;
        }
    }
}
