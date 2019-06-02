using GubGub.Scripts.Enum;

namespace GubGub.Scripts.Command
{
    /// <summary>
    /// 特定のラベルに飛ぶためのパラメータを持つ
    /// </summary>
    public class JumpCommand : BaseScenarioCommand
    {
        public string LabelName { get; private set; }


        public JumpCommand() : base(EScenarioCommandType.Jump)
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
