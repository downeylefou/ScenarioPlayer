using GubGub.Scripts.Enum;

namespace GubGub.Scripts.Command
{
    /// <summary>
    /// 特定のラベルを表すためのパラメータを持つ
    /// </summary>
    public class LabelCommand : BaseScenarioCommand
    {
        public string LabelName { get; private set; }


        public LabelCommand() : base(EScenarioCommandType.Label)
        {
        }

        protected override void Reset()
        {
            LabelName = "";
        }

        protected sealed override void MapParameters()
        {
            if (rawParams.Count > 0)
            {
                LabelName = GetString(0, null);
            }
        }
    }
}
