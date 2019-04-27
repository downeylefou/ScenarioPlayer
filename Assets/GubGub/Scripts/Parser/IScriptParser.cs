using System.Collections.Generic;

namespace GubGub.Scripts.Parser
{
    /// <summary>
    /// シナリオデータをパースするためのインタフェース
    /// </summary>
    public interface IScriptParser
    {
        List<List<string>> ParseScript(string scriptUrl, string rawScript);
        List<string> GetResourceNames();
    }
}
