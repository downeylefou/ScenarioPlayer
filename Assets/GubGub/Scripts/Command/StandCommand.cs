using GubGub.Scripts.Enum;
using GubGub.Scripts.Lib.ResourceSetting;

namespace GubGub.Scripts.Command
{
    /// <summary>
    ///  立ち絵と感情アイコンの表示用パラメータを持つ
    /// </summary>
    public class StandCommand : BaseScenarioCommand
    {
        private const int DefaultFadeTimeMilliSecond = 100;
        private const int DefaultEmotionTimeMilliSecond = 500;

        private const string EmotionOff = "off";

        public string EmotionName { get; private set; }
        public string Position { get; private set; }
        public string StandName { get; private set; }

        public int OffsetX { get; private set; }
        public int OffsetY { get; private set; }

        public int FadeTimeMilliSecond { get; private set; }
        public bool IsWait { get; private set; } // フェード表示を待って次のコマンドに移るか
        public bool Reverse { get; private set; }

        public string FilePath { get; set; }


        public StandCommand() : base(EScenarioCommandType.Stand)
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
                OffsetX += characterEntity.OffsetX;
                OffsetY += characterEntity.OffsetY;
            }
        }

        protected override void Reset()
        {
            EmotionName = EmotionOff;
            Position = EScenarioStandPosition.Center.GetName();
            StandName = "";
            OffsetX = 0;
            OffsetY = 0;

            FadeTimeMilliSecond = DefaultFadeTimeMilliSecond;
            Reverse = false;
            IsWait = false;
        }

        protected sealed override void MapParameters()
        {
            EmotionName = GetString(0, EmotionOff);
            StandName = GetString(1, null);
            Position = GetString(2, EScenarioStandPosition.Center.GetName());

            if (rawParams.Count >= 4)
            {
                OffsetX = GetInt(3, 0);
            }

            if (rawParams.Count >= 5)
            {
                OffsetY = GetInt(4, 0);
            }

            FadeTimeMilliSecond = GetInt("fadeTime", DefaultFadeTimeMilliSecond);
            Reverse = GetBool("reverse", false);
            IsWait = GetBool("wait", false);
        }
    }
}