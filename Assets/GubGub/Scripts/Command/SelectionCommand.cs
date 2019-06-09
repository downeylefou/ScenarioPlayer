using GubGub.Scripts.Enum;

namespace GubGub.Scripts.Command
{
    /// <summary>
    /// 選択肢のパラメータを持つ
    /// </summary>
    public class SelectionCommand : BaseScenarioCommand
    {
        public string LabelName { get; private set; }
        
        public string SelectionText { get; private set; }


        public SelectionCommand() : base(EScenarioCommandType.Selection)
        {
        }

        protected override void Reset()
        {
            LabelName = "";
            SelectionText = "";
        }

        protected sealed override void MapParameters()
        {
            LabelName = GetString(0, null);
            SelectionText = GetString(1, null);
        }
    }
}
