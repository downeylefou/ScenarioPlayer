using System;
using System.Collections.Generic;
using System.Linq;
using GubGub.Scripts.Enum;

namespace GubGub.Scripts.Parser
{
    /// <inheritdoc />
    /// <summary>
    ///  TSVファイルスクリプトのパーサー
    /// </summary>
    public class ScenarioParser : IScriptParser
    {
        private static readonly char[] LineBreak = "\r\n".ToCharArray();
        private static readonly char[] CommandDelimiter = "\t".ToCharArray();

        private readonly List<string> _resourceNames = new List<string>();
        // private string _resourceDirectory;


        /// <summary>
        ///  リソース名リストを取得する
        /// </summary>
        /// <returns></returns>
        public List<string> GetResourceNames()
        {
            return _resourceNames;
        }

        /// <summary>
        ///  スクリプトデータをパースする
        /// </summary>
        /// <param name="scriptUrl"></param>
        /// <param name="rawScript"></param>
        /// <returns></returns>
        public List<List<string>> ParseScript(string scriptUrl, string rawScript)
        {
            var lineList = rawScript.Split(LineBreak, StringSplitOptions.RemoveEmptyEntries);
            var commandList = new List<List<string>>();

            foreach (var line in lineList)
            {
                commandList.Add(ParseLine(line));
            }

            return commandList;
        }

        private List<string> ParseLine(string line)
        {
            var values = line.Split(CommandDelimiter).ToList();
            PushResources(values);

            return values;
        }

        /// <summary>
        /// 行を分割した配列からリソースを取得しresourceNamesに追加する
        /// </summary>
        /// <param name="values"></param>
        private void PushResources(List<string> values)
        {
            var command = EScenarioCommandTypeExtension.GetEnum(values[0]);
            var urlList = new List<string>();

            switch (command)
            {
                case EScenarioCommandType.Image:
                    // case EScenarioCommandType.MOVIE:
                    // case EScenarioCommandType.BGM:
                    // case EScenarioCommandType.SE:
                    urlList.Add(values[1]);
                    break;
                case EScenarioCommandType.Stand:
                    urlList.Add(values[2]);
                    break;
                // case EScenarioCommandType.START:
                //     urlList.push(values[2]);
                //     urlList.push(values[3]);
                //     break;
                case EScenarioCommandType.Unknown:
                    if (values[0] == "")
                    {
                        // 会話メッセージの場合
                        if (values.Count > 1 && values[1] != null && values[1].Length > 0)
                        {
                            urlList.Add(values[1]);
                        }
                    }

                    break;
            }

            _resourceNames.Concat(urlList.Where(ValidateResourceUrl).Select(x => x).ToList());
        }

        /// <summary>
        /// 正常なリソースパスか判定する
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool ValidateResourceUrl(string value)
        {
          
            return true;
        }
    }
}
