﻿using System.Collections.Generic;
using System.Linq;

namespace GubGub.Scripts.Enum
{
    /// <summary>
    ///  シナリオスクリプトのコマンドタイプ
    /// </summary>
    public enum EScenarioCommandType
    {
        Unknown,
        ShowWindow,
        Message,
        Image,
        Stand,
        Face,
        FaceOff,
        Label,
        Wait,
        FadeIn,
        FadeOut,
        Clear,
        Se,
        Bgm,
        Jump,
        Selection,
        StopScenario,
    }

    /// <summary>
    ///  EScenarioCommandTypeの拡張クラス
    /// </summary>
    internal static class EScenarioCommandTypeExtension
    {
        private static readonly Dictionary<EScenarioCommandType, string> NameList =
            new Dictionary<EScenarioCommandType, string>()
            {
                {EScenarioCommandType.Unknown, "unknown"},
                {EScenarioCommandType.ShowWindow, "showWindow"},
                {EScenarioCommandType.Message, "message"},
                {EScenarioCommandType.Image, "image"},
                {EScenarioCommandType.Stand, "stand"},
                {EScenarioCommandType.Face, "face"},
                {EScenarioCommandType.Label, "label"},
                {EScenarioCommandType.Wait, "wait"},
                {EScenarioCommandType.FadeIn, "fadeIn"},
                {EScenarioCommandType.FadeOut, "fadeOut"},
                {EScenarioCommandType.Clear, "clear"},
                {EScenarioCommandType.Se, "se"},
                {EScenarioCommandType.Bgm, "bgm"},
                {EScenarioCommandType.Jump, "jump"},
                {EScenarioCommandType.Selection, "selection"},
            };

        /// <summary>
        ///  Enumに対応した文字列を取得する
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetName(this EScenarioCommandType value)
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
        public static EScenarioCommandType GetEnum(string value)
        {
            return NameList.FirstOrDefault(x => x.Value == value).Key;
        }

        /// <summary>
        ///  指定文字列がタイプ（文字列）と等しいか
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsEqual(this EScenarioCommandType type, string value)
        {
            return (type.GetName() == value);
        }

        /// <summary>
        ///  指定文字列がタイプ（文字列）のどれかと等しいか
        /// </summary>
        /// <param name="types"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsEqual(this IEnumerable<EScenarioCommandType> types, string value)
        {
            return types.Any(type => IsEqual(type, value));
        }
    }
}
