using GubGub.Scripts.Enum;

namespace GubGub.Scripts.Data
{
    /// <summary>
    ///  シナリオ再生用のオプションデータ
    /// </summary>
    public class ScenarioConfigData
    {
        /// <summary>
        ///  どのメッセージビューを使用するか
        /// </summary>
        public EScenarioMessageViewType MessageViewType { get; set; }

        /// <summary>
        /// メッセージの一文字を表示する時間
        /// </summary>
        public int MessageSpeedMilliSecond { get; set; }

        /// <summary>
        ///  オート時の一文字あたりの待機時間
        /// </summary>
        public int AutoMessageSpeedMilliSecond { get; set; }

        /// <summary>
        ///  オートモード時改ページの最小待ち時間
        /// </summary>
        public int MinAutoWaitTimeMilliSecond { get; set; }

        /// <summary>
        ///  スキップ時の一文字あたりの待機時間
        /// </summary>
        public int SkipMessageSpeedMilliSecond { get; set; }
        
        /// <summary>
        ///  スキップ時の改ページの最小待ち時間
        /// </summary>
        public int MinSkipWaitTimeMilliSecond { get; set; }

        public ScenarioConfigData()
        {
            MessageViewType = EScenarioMessageViewType.Default;
            MessageSpeedMilliSecond = 30;
            AutoMessageSpeedMilliSecond = 100;
            MinAutoWaitTimeMilliSecond = 1000;
            SkipMessageSpeedMilliSecond = 20;
            MinSkipWaitTimeMilliSecond = 50;
        }
    }
}
