namespace Sample._1_Adventure.Scripts.Data
{
    /// <summary>
    /// シナリオ再生の起点となるラベル名
    /// </summary>
    public enum AdvScenarioLabel
    {
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