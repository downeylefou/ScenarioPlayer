using System.Collections.Generic;
using System.Linq;

namespace GubGub.Scripts.Enum
{
    /// <summary>
    ///  シナリオスクリプトの立ち位置
    /// </summary>
    public enum EScenarioStandPosition
    {
        Left,
        Center,
        Right,
    }

    /// <summary>
    ///  EScenarioStandPositionの拡張クラス
    /// </summary>
    internal static class EScenarioStandPositionExtension
    {
        public static Dictionary<EScenarioStandPosition, string> NameList { get; private set; }=
            new Dictionary<EScenarioStandPosition, string>()
            {
                {EScenarioStandPosition.Left, "left"},
                {EScenarioStandPosition.Center, "center"},
                {EScenarioStandPosition.Right, "right"},
            };

        /// <summary>
        ///  Enumに対応した文字列を取得する
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetName(this EScenarioStandPosition value)
        {
            return NameList[value];
        }

        public static bool IsContain(string value)
        {
            return NameList.ContainsValue(value);
        }

        /// <summary>
        ///  文字列に対応したEnumを取得する
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static EScenarioStandPosition GetEnum(string value)
        {
            return NameList.FirstOrDefault(x => x.Value == value).Key;
        }
    }
}
