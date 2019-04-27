using System.Collections.Generic;
using System.Linq;

namespace GubGub.Scripts.Enum
{
    /// <summary>
    /// シナリオのメッセージビュータイプ
    /// 異なるメッセージビューを使用する際、タイプを追加する
    /// </summary>
    public enum EScenarioMessageViewType
    {
        Default = 0,
    }

    /// <summary>
    ///  EScenarioMessageViewTypeの拡張クラス
    /// </summary>
    internal static class EScenarioMessageViewTypeExtension
    {
        private static readonly Dictionary<EScenarioMessageViewType, string> NameList =
            new Dictionary<EScenarioMessageViewType, string>()
            {
                {EScenarioMessageViewType.Default, "default"},
            };

        /// <summary>
        ///  Enumに対応した文字列を取得する
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetName(this EScenarioMessageViewType value)
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
        public static EScenarioMessageViewType GetEnum(string value)
        {
            return NameList.FirstOrDefault(x => x.Value == value).Key;
        }
    }
}
