using System;

namespace GubGub.Scripts.Lib.ResourceSetting
{
    /// <summary>
    /// 設定ファイルのBGMシート用Entity
    /// </summary>
    [Serializable]
    public class BgmSheetEntity : IResourceSettingEntity
    {
        public string Name;
        public string FilePath;
    }
}