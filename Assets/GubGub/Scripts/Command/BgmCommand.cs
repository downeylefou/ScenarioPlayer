using System;
using GubGub.Scripts.Enum;

namespace GubGub.Scripts.Command
{
    /// <summary>
    /// Bgm再生のパラメータを持つ
    /// </summary>
    public class BgmCommand : BaseScenarioCommand
    {
        public string FileName { get; private set; }


        public BgmCommand() : base(EScenarioCommandType.Bgm)
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
