using GubGub.Scripts.Enum;
using GubGub.Scripts.Lib.ResourceSetting;

namespace GubGub.Scripts.Command
{
    /// <summary>
    ///  顔ウィンドウ表示用パラメータを持つ
    /// </summary>
    public class FaceCommand : BaseScenarioCommand
    {
        public string Name { get; private set; }
        public float OffsetX { get; private set; }
        public float OffsetY { get; private set; }
        public float ScaleX { get; private set; }
        public float ScaleY { get; private set; }
        public bool Reverse { get; private set; }

        public string FilePath { get; set; }


        public FaceCommand() : base(EScenarioCommandType.Face)
        {
        }

        /// <summary>
        /// 設定シートからパラメータを更新する
        /// </summary>
        /// <param name="entity"></param>
        public override void UpdateParameter(IResourceSettingEntity entity)
        {
            if (entity is CharacterSheetEntity characterEntity)
            {
                FilePath = characterEntity.FilePath;
                OffsetX += characterEntity.FaceX;
                OffsetY += characterEntity.FaceY;
                ScaleX = (characterEntity.FaceScaleX != 0 && ScaleX == 1) ? characterEntity.FaceScaleX : ScaleX;
                ScaleY = (characterEntity.FaceScaleY != 0 && ScaleY == 1) ? characterEntity.FaceScaleY : ScaleY;
            }
        }

        protected override void Reset()
        {
            Name = "";
            OffsetX = 0;
            OffsetY = 0;
            ScaleX = 1;
            ScaleY = 1;
            Reverse = false;
        }

        protected sealed override void MapParameters()
        {
            Name = GetString(1, null);
            FilePath = Name;

            if (rawParams.Count >= 2)
            {
                OffsetX = GetInt(3, 0);
            }

            if (rawParams.Count >= 3)
            {
                OffsetY = GetInt(4, 0);
            }

            ScaleX = GetInt("scaleX", 1);
            ScaleY = GetInt("scaleY", 1);
            Reverse = GetBool("reverse", false);
        }
    }
}