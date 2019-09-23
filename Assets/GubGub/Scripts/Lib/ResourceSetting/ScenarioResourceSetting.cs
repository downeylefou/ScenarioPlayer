using System.Collections.Generic;
using UnityEngine;

namespace GubGub.Scripts.Lib.ResourceSetting
{
    /// <summary>
    /// インポータから出力される設定データクラス
    /// </summary>
    [ExcelAsset]
    public class ScenarioResourceSetting : ScriptableObject
    {
        public List<CharacterSheetEntity> character;
        public List<BgmSheetEntity> bgm;
    }
}