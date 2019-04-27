using System.Collections.Generic;
using System.Linq;

namespace GubGub.Scripts.Enum
{
    /// <summary>
    ///  シナリオスクリプトの指定色タイプ
    /// </summary>
    public enum EScenarioColorType
    {
        Black,
        White,
        Red,
        Green,
        Blue,
        Yellow,
        Purple
    }

    /// <summary>
    ///  EScenarioColorTypeの拡張クラス
    /// </summary>
    internal static class EScenarioColorTypeExtension
    {
        private static readonly Dictionary<EScenarioColorType, string> NameList =
            new Dictionary<EScenarioColorType, string>()
            {
                {EScenarioColorType.Black, "black"},
                {EScenarioColorType.White, "white"},
                {EScenarioColorType.Red, "red"},
                {EScenarioColorType.Green, "green"},
                {EScenarioColorType.Blue, "blue"},
                {EScenarioColorType.Yellow, "yellow"},
                {EScenarioColorType.Purple, "purple"},
            };

        private static readonly Dictionary<EScenarioColorType, string> ColorList =
            new Dictionary<EScenarioColorType, string>()
            {
                {EScenarioColorType.Black, "#000000"},
                {EScenarioColorType.White, "#ffffff"},
                {EScenarioColorType.Red, "#ff0000"},
                {EScenarioColorType.Green, "#00ff00"},
                {EScenarioColorType.Blue, "#0000ff"},
                {EScenarioColorType.Yellow, "#ffff00"},
                {EScenarioColorType.Purple, "#ff00ff"},
            };

        /// <summary>
        ///  Enumに対応した文字列を取得する
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetName(this EScenarioColorType value)
        {
            return NameList[value];
        }

        /// <summary>
        ///  Enumに対応した色番号文字列を取得する
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetColorString(this EScenarioColorType value)
        {
            return ColorList[value];
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
        public static EScenarioColorType GetEnum(string value)
        {
            return NameList.FirstOrDefault(x => x.Value == value).Key;
        }

        /// <summary>
        ///  指定文字列がタイプ（文字列）と等しいか
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsEqual(this EScenarioColorType type, string value)
        {
            return (GetName(type) == value);
        }

        /// <summary>
        ///  指定文字列がタイプ（文字列）のどれかと等しいか
        /// </summary>
        /// <param name="types"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsEqual(this EScenarioColorType[] types, string value)
        {
            return types.Any(type => IsEqual(type, value));
        }
    }
}
