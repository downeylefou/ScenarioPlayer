using System.Collections.Generic;
using GubGub.Scripts.Data;

namespace GubGub.Scripts.View.Interface
{
    /// <summary>
    /// バックログのスクロールビューのインターフェイス
    /// </summary>
    public interface ILogScrollView
    {
        void UpdateData(IList<ScenarioLogData> items);
    }
}