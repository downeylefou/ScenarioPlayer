using System;
using GubGub.Scripts.Enum;

namespace GubGub.Scripts.Command
{
    /// <summary>
    /// ウェイト時間のパラメータを持つ
    /// </summary>
    public class WaitCommand : BaseScenarioCommand
    {
        public const int DefaultWaitMilliSecond = 1000;

        public int waitMilliSecond;


        public WaitCommand() : base(EScenarioCommandType.Wait)
        {
        }

        protected override void Reset()
        {
            waitMilliSecond = DefaultWaitMilliSecond;
        }

        protected sealed override void MapParameters()
        {
            if(rawParams.Count > 0)
                waitMilliSecond = Convert.ToInt16(rawParams[0]);
        }
    }
}
