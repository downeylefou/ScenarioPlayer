using System;
using System.Collections.Generic;
using GubGub.Scripts.Enum;
using UnityEngine.UI;

namespace GubGub.Scripts.Data
{
    /// <summary>
    ///  シナリオデータ
    ///  パースした後のデータが格納されていて、再生位置も保持する
    /// </summary>
    public class ScenarioParseData
    {
        /// <summary>
        /// コマンド名が入るカラム番号
        /// </summary>
        private const int CommandNameColumnIndex = 0;
        
        /// <summary>
        /// ラベル名を指定するコマンドで、ラベル名が入るカラム番号
        /// </summary>
        private const int LabelNameColumnIndex = 1;
        
        private int _lineIndex;
        private readonly List<List<string>> _lineList;

        private List<string> _resourceList = new List<string>();


        public ScenarioParseData(List<List<string>> lineList)
        {
            _lineList = lineList;
        }

        /// <summary>
        ///  現在の行を取得する
        /// </summary>
        /// <returns></returns>
        public List<string> GetCurrentLine()
        {
            List<string> line = null;

            if (_lineList.Count > _lineIndex)
            {
                line = _lineList[_lineIndex];
            }

            return line;
        }

        /// <summary>
        /// 行番号を進める
        /// </summary>
        public void AdvanceLineNumber()
        {
            _lineIndex++;
        }

        /// <summary>
        /// 指定ラベルにジャンプし、行の情報を取得する
        /// </summary>
        /// <param name="labelName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public List<string> GetLineForJumpToLabel(string labelName)
        {
            var enumLabelName = EScenarioCommandType.Label.GetName();

            for (var i = 0; i < _lineList.Count; i++)
            {
                // "label ラベル名"となっている行を探す
                if (_lineList[i][CommandNameColumnIndex] == 
                    enumLabelName && _lineList[i][LabelNameColumnIndex] == labelName)
                {
                    _lineIndex = i;
                    return _lineList[_lineIndex];
                }
            }

            throw new ArgumentException("[ScenarioParseData] ジャンプ先ラベル名 '" + labelName + "' が見つかりません。");
        }

        /// <summary>
        /// 次の行のコマンド名が、指定する名前と一致するか
        /// </summary>
        /// <param name="commandName"></param>
        /// <returns></returns>
        public bool GetIsMatchNextLineCommandName(string commandName)
        {
            if (_lineIndex + 1 > _lineList.Count)
            {
                return false;
            }

            var nextCommandName = _lineList[_lineIndex + 1][CommandNameColumnIndex];
            // 大文字・小文字は区別しない
            if (String.Compare(
                    nextCommandName, 
                    commandName, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return true;
            }

            return false;
        }
    }
}
