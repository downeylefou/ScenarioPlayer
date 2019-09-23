using System.Collections.Generic;
using GubGub.Scripts.Enum;
using GubGub.Scripts.Lib.ResourceSetting;

namespace GubGub.Scripts.Parser
{
    /// <summary>
    /// シナリオデータをパースするためのインタフェース
    /// </summary>
    public interface IScriptParser
    {
        List<List<string>> ParseScript(string rawScript, ScenarioResourceSettingModel setting);
        Dictionary<string, EResourceType> GetResourceList();
    }
}