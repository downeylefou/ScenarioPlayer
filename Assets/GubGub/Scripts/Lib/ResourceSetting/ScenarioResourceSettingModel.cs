using System.Linq;
using GubGub.Scripts.Enum;

namespace GubGub.Scripts.Lib.ResourceSetting
{
    /// <summary>
    /// リソース設定モデル
    /// </summary>
    public class ScenarioResourceSettingModel
    {
        /// <summary>
        /// リソース設定オブジェクト
        /// </summary>
        private ScenarioResourceSetting _resourceSetting;

        /// <summary>
        /// 設定オブジェクトを渡して初期化する
        /// </summary>
        /// <param name="setting"></param>
        public void Initialize(ScenarioResourceSetting setting)
        {
            _resourceSetting = setting;
        }

        /// <summary>
        /// リソース設定Entityを取得する
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IResourceSettingEntity GetSettingEntity(string resourceName, EResourceType type)
        {
            if (_resourceSetting == null || string.IsNullOrEmpty(resourceName))
            {
                return null;
            }

            switch (type)
            {
                case EResourceType.Character:
                    return _resourceSetting.character.FirstOrDefault(_ => _.Name == resourceName);
                case EResourceType.Bgm:
                    return _resourceSetting.bgm.FirstOrDefault(_ => _.Name == resourceName);
                default:
                    return null;
            }
        }
        
        /// <summary>
        /// リソース設定に記述されたファイルパスを取得する
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetResourcePath(string resourceName, EResourceType type)
        {
            if (_resourceSetting == null || string.IsNullOrEmpty(resourceName))
            {
                return null;
            }

            switch (type)
            {
                case EResourceType.Character:
                    return _resourceSetting.character.FirstOrDefault(_ => _.Name == resourceName)?.FilePath;
                case EResourceType.Bgm:
                    return _resourceSetting.bgm.FirstOrDefault(_ => _.Name == resourceName)?.FilePath;
                default:
                    return null;
            }
        }
    }
}