using GubGub.Scripts.Enum;

namespace GubGub.Scripts.Command
{
    /// <summary>
    ///  フェードアウト用のパラメータを持つ
    /// </summary>
    public class FadeOutCommand : BaseScenarioCommand
    {
        private readonly int defaultFadeMilliSecond = 1000;
        private readonly float defaultAlpha = 1.0f;

        public string colorString;
        public int fadeMilliSecond;

        public float alpha;


        public FadeOutCommand() : base(EScenarioCommandType.FadeOut)
        {
        }

        protected override void Reset()
        {
            colorString = EScenarioColorType.Black.GetColorString();
            fadeMilliSecond = defaultFadeMilliSecond;
            alpha = defaultAlpha;
        }

        protected sealed override void MapParameters()
        {
            var colorName = GetString(0, EScenarioColorType.Black.GetName());
            var colorEnum = (EScenarioColorTypeExtension.GetEnum(colorName));
            colorString = colorEnum.GetColorString();

            fadeMilliSecond = GetInt(1, defaultFadeMilliSecond);
            alpha = GetFloat("alpha", defaultAlpha);
        }
    }
}
