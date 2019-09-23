using System;

namespace GubGub.Scripts.Lib.ResourceSetting
{
    /// <summary>
    /// 設定ファイルのキャラクターシート用Entity
    /// </summary>
    [Serializable]
    public class CharacterSheetEntity : IResourceSettingEntity
    {
        public string Name;
        public string FilePath;
        public int OffsetX;
        public int OffsetY;
        
        // TODO: StandCommandで未実装
        public string TalkName;
        public float ScaleX;
        public float ScaleY;
        public float FaceX;
        public float FaceY;
        public float FaceScaleX;
        public float FaceScaleY;
    }


}