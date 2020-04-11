using System;

namespace Sample._1_Adventure.Scripts.Data
{
    /// <summary>
    /// シナリオで使用するパラメータ
    /// </summary>
    public enum AdvScenarioParameter
    {
        MashRoomCount,
        AcornCount,
    }

    /// <summary>
    /// AdvScenarioParameterの拡張クラス
    /// </summary>
    public static class AdvScenarioParameterExtension
    {
        /// <summary>
        /// ラベル名を文字列で取得する
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static string GetName(this AdvScenarioParameter parameter)
        {
            return Enum.GetName(typeof(AdvScenarioParameter), parameter);
        }
    }
}