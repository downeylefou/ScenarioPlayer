using GubGub.Scripts.Enum;

namespace GubGub.Scripts.Command
{
    /// <summary>
    /// 背景画像表示用パラメータを持つ
    /// </summary>
    public class ImageCommand : BaseScenarioCommand
    {
        public const int DefaultFadeTimeMilliSecond = 500;

        public string ImageName { get; private set; }
        public int FadeTimeMilliSecond { get; private set; }

        public bool IsWait { get; private set; } // フェード表示を待って次のコマンドに移るか


        public ImageCommand() : base(EScenarioCommandType.Image)
        {
        }

        protected override void Reset()
        {
            ImageName = "";
            FadeTimeMilliSecond = DefaultFadeTimeMilliSecond;
        }

        protected sealed override void MapParameters()
        {
            ImageName = GetString(0, null);
            FadeTimeMilliSecond = GetInt("fadeTime", DefaultFadeTimeMilliSecond);
        }
    }
}
