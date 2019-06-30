using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GubGub.Scripts.Enum
{

    /// <summary>
    /// シナリオ設定データの定数
    /// </summary>
    public enum EScenarioConfigKey
    {
        BgmVolume,
        SeVolume,
    }

    /// <summary>
    ///  EScenarioConfigKeyの拡張クラス
    /// </summary>
    internal static class EScenarioConfigKeyExtension
    {
        public static Dictionary<EScenarioConfigKey, string> NameList { get;} =
            new Dictionary<EScenarioConfigKey, string>()
            {
                {EScenarioConfigKey.BgmVolume, "bgm_volume"},
                {EScenarioConfigKey.SeVolume, "se_volume"},
            };
        
        /// <summary>
        ///  Enumに対応した文字列を取得する
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetName(this EScenarioConfigKey value)
        {
            return NameList[value];
        }
        
    }
}