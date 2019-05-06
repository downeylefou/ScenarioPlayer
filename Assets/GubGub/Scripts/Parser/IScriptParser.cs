using System.Collections.Generic;
using GubGub.Scripts.Enum;

namespace GubGub.Scripts.Parser
{
    /// <summary>
    /// シナリオデータをパースするためのインタフェース
    /// </summary>
    public interface IScriptParser
    {
        List<List<string>> ParseScript(string scriptUrl, string rawScript);
        Dictionary<string, EResourceType> GetResourceList();
    }
}
