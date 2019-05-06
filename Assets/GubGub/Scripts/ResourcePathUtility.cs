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
        
        private static Dictionary<EResourceType, string> _prefixList =
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

            StringBuilder.Append(_prefixList.FirstOrDefault(data => data.Key.Equals(type)).Value);
            StringBuilder.Append(filePath);

            return StringBuilder.ToString();
        }
        
        /// <summary>
        /// プラットフォームごとのパスを作成
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static string GetFilePathForTargetPlatform(string filePath)
        {
            StringBuilder.Clear();

            // プラットフォームごとの StreamingAssetsのパス
            if (ResourceLoadSetting.ResourceLoadType == EResourceLoadType.StreamingAssets)
            {
                #if UNITY_EDITOR || WebPlayer
                StringBuilder.Append(Application.dataPath);
                StringBuilder.Append("/StreamingAssets/");
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
                StringBuilder.Append("windows standalone(64-bit)/");
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                StringBuilder.Append("android");
            }
            StringBuilder.Append(filePath);

            return StringBuilder.ToString();
        }
        
        /// <summary>
        /// 固定のサフィックスを追加したアセットバンドル名を取得する
        /// TODO: ユーザーがインスペクタから設定可能にする
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
