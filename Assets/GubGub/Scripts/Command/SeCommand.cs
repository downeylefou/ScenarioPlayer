using GubGub.Scripts.Enum;

namespace GubGub.Scripts.Command
{
    /// <summary>
    /// Se再生のパラメータを持つ
    /// </summary>
    public class SeCommand : BaseScenarioCommand
    {
        public string FileName { get; private set; }


        public SeCommand() : base(EScenarioCommandType.Se)
        {
        }

        protected override void Reset()
        {
            FileName = "";
        }

        protected sealed override void MapParameters()
        {
            if (rawParams.Count > 0)
            {
                FileName = rawParams[0];
            }
        }
    }
}
