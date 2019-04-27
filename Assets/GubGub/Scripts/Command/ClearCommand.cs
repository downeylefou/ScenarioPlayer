using GubGub.Scripts.Enum;

namespace GubGub.Scripts.Command
{
    /// <summary>
    /// 表示の消去用パラメータを持つ
    /// </summary>
    public class ClearCommand : BaseScenarioCommand
    {
        public const int DefaultFadeTimeMilliSecond = 300;

        public EScenarioClearTargetType ClearTarget { get; private set; }
        public int FadeTimeMilliSecond { get; private set; }


        public ClearCommand() : base(EScenarioCommandType.Clear)
        {
        }

        protected override void Reset()
        {
            ClearTarget = EScenarioClearTargetType.All;
            FadeTimeMilliSecond = DefaultFadeTimeMilliSecond;
        }

        protected sealed override void MapParameters()
        {
            var targetString = GetString(0, "");
            ClearTarget = EScenarioClearTargetTypeExtension.GetEnum(targetString);

            FadeTimeMilliSecond = GetInt("fadeTime", DefaultFadeTimeMilliSecond);
        }
    }
}
