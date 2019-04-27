using System.Collections.Generic;
using System.Linq;

namespace GubGub.Scripts.Enum
{
    /// <summary>
    ///  シナリオメッセージビューの位置
    /// </summary>
    public enum EScenarioMessageViewPosition
    {
        Bottom,
        Top,
        Center,
    }

    /// <summary>
    ///  EScenarioMessageViewPositionの拡張クラス
    /// </summary>
    internal static class EScenarioMessageViewPositionExtension
    {
        private static readonly Dictionary<EScenarioMessageViewPosition, string> NameList =
            new Dictionary<EScenarioMessageViewPosition, string>()
            {
                {EScenarioMessageViewPosition.Bottom, "bottom"},
                {EScenarioMessageViewPosition.Top, "top"},
                {EScenarioMessageViewPosition.Center, "center"},
            };

        /// <summary>
        ///  Enumに対応した文字列を取得する
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetName(this EScenarioMessageViewPosition value)
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
        public static EScenarioMessageViewPosition GetEnum(string value)
        {
            return NameList.FirstOrDefault(x => x.Value == value).Key;
        }
    }
}
