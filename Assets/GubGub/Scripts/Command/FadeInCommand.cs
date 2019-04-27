using GubGub.Scripts.Enum;

namespace GubGub.Scripts.Command
{
    /// <summary>
    ///  フェードイン用のパラメータを持つ
    /// </summary>
    public class FadeInCommand : BaseScenarioCommand
    {
        private const int DefaultFadeMilliSecond = 1000;
        private readonly float DefaultAlpha = 0f;

        public string colorString;
        public int fadeMilliSecond;

        public float alpha;


        public FadeInCommand() : base(EScenarioCommandType.FadeIn)
        {
        }

        protected override void Reset()
        {
            colorString = EScenarioColorType.Black.GetColorString();
            fadeMilliSecond = DefaultFadeMilliSecond;
            alpha = DefaultAlpha;
        }

        protected sealed override void MapParameters()
        {
            var colorName = GetString(0, EScenarioColorType.Black.GetName());
            var colorEnum = (EScenarioColorTypeExtension.GetEnum(colorName));
            colorString = (colorEnum != null) ? colorEnum.GetColorString() : colorString;

            fadeMilliSecond = GetInt(1, DefaultFadeMilliSecond);
            alpha = GetFloat("alpha", DefaultAlpha);
        }
    }
}
