using GubGub.Scripts.Enum;
using UnityEngine;

namespace GubGub.Scripts.Command
{
    public class UnknownCommand : BaseScenarioCommand
    {
        public UnknownCommand() : base(EScenarioCommandType.Unknown)
        {
        }

        protected override void Reset()
        {
            Debug.Log("UnknownCommand: Reset()");
        }

        protected override void MapParameters()
        {
            Debug.Log("UnknownCommand: MapParameters()");
        }
    }
}