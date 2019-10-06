using System.Collections.Generic;
using System.Linq;

namespace GubGub.Scripts.Enum
{
    /// <summary>
    ///  シナリオの表示消去コマンドの対象タイプ
    /// </summary>
    public enum EScenarioClearTargetType
    {
        All,
        Left,
        Center,
        Right,
        Text,
        Face,
    }

    /// <summary>
    ///  EScenarioClearTargetTypeの拡張クラス
    /// </summary>
    internal static class EScenarioClearTargetTypeExtension
    {
        private static readonly Dictionary<EScenarioClearTargetType, string> NameList =
            new Dictionary<EScenarioClearTargetType, string>()
            {
                {EScenarioClearTargetType.All, "all"},
                {EScenarioClearTargetType.Left, "left"},
                {EScenarioClearTargetType.Center, "center"},
                {EScenarioClearTargetType.Right, "right"},
                {EScenarioClearTargetType.Text, "text"},
                {EScenarioClearTargetType.Face, "face"},
            };

        /// <summary>
        ///  文字列に対応したEnumを取得する
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static EScenarioClearTargetType GetEnum(string value)
        {
            return NameList.FirstOrDefault(x => x.Value == value).Key;
        }

        /// <summary>
        ///  Enumに対応した文字列を取得する
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetName(this EScenarioClearTargetType value)
        {
            return NameList[value];
        }
    }
}
