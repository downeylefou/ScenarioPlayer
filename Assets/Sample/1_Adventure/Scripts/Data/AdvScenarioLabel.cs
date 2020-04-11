namespace Sample._1_Adventure.Scripts.Data
{
    /// <summary>
    /// シナリオ再生の起点となるラベル名
    /// </summary>
    public enum AdvScenarioLabel
    {
        _1_start,
        _1_mashRoom,
        _1_mother_count,
        _1_mother_warning,
        
        _2_acorn1,
        _2_acorn2,
    }

    /// <summary>
    /// AdvScenarioLabelの拡張クラス
    /// </summary>
    public static class AdvScenarioLabelExtension
    {
        /// <summary>
        /// ラベル名を文字列で取得する
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public static string GetName(this AdvScenarioLabel label)
        {
            return System.Enum.GetName(typeof(AdvScenarioLabel), label);
        }
    }
}