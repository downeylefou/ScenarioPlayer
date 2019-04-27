using GubGub.Scripts.Enum;

namespace GubGub.Scripts.Command
{
    /// <inheritdoc />
    /// <summary>
    ///  メッセージウインドウを表示するためのパラメータを持つ
    /// </summary>
    public class ShowWindowCommand : BaseScenarioCommand
    {
        public EScenarioMessageViewType ViewType { get; private set; }
        public EScenarioMessageViewPosition Position { get; private set; }
        public int MarginX { get; private set; }
        public int MarginY { get; private set; }


        public ShowWindowCommand() : base(EScenarioCommandType.ShowWindow)
        {
        }

        protected override void Reset()
        {
            ViewType = EScenarioMessageViewType.Default;
            Position = EScenarioMessageViewPosition.Bottom;
            MarginX = 0;
            MarginY = 0;
        }

        protected sealed override void MapParameters()
        {
            ViewType = EScenarioMessageViewTypeExtension.GetEnum(GetString(0, ""));
            Position = EScenarioMessageViewPositionExtension.GetEnum(GetString(1, ""));

            if (rawParams.Count >= 3)
            {
                MarginX = GetInt(2, 0);
            }

            if (rawParams.Count >= 4)
            {
                MarginY = GetInt(3, 0);
            }
        }
    }
}
