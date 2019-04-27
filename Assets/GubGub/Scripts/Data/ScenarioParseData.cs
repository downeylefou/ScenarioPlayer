using System;
using System.Collections.Generic;
using GubGub.Scripts.Enum;

namespace GubGub.Scripts.Data
{
    /// <summary>
    ///  シナリオデータ
    ///  パースした後のデータが格納されていて、再生位置も保持する
    /// </summary>
    public class ScenarioParseData
    {
        private int _lineIndex;
        private readonly List<List<string>> _lineList;

        private List<string> _resourceList = new List<string>();


        public ScenarioParseData(List<List<string>> lineList)
        {
            _lineList = lineList;
        }

        /// <summary>
        ///  現在の行を取得して、行番号を進める
        /// </summary>
        /// <returns></returns>
        public List<string> GetCurrentLineAndAdvanceNumber()
        {
            List<string> line = null;

            if (_lineList.Count > _lineIndex)
            {
                line = _lineList[_lineIndex];
                _lineIndex++;
            }

            return line;
        }

        /// <summary>
        ///  指定ラベルにジャンプする
        /// </summary>
        /// <param name="labelName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public List<string> GotoLabel(string labelName)
        {
            var enumLabelName = EScenarioCommandType.Label.GetName();

            for (var i = 0; i < _lineList.Count; i++)
            {
                if (_lineList[0][0] == enumLabelName && _lineList[0][1] == labelName)
                {
                    _lineIndex = i;
                    return _lineList[_lineIndex];
                }
            }

            throw new ArgumentException("[ScenarioParseData] ジャンプ先ラベル名 '" + labelName + "' が見つかりません。");
        }
    }
}
