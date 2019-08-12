using GubGub.Scripts.Enum;
using UnityEngine;

namespace GubGub.Scripts.Command
{
    /// <inheritdoc />
    /// <summary>
    /// シナリオの再生を中断するコマンド
    /// </summary>
    public class StopScenarioCommand : BaseScenarioCommand
    {

        public StopScenarioCommand() : base(EScenarioCommandType.StopScenario)
        {
        }
        
        protected override void Reset()
        {
        }

        protected override void MapParameters()
        {
        }
    }
}
