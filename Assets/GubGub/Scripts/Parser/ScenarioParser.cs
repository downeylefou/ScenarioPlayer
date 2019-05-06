using System;
using System.Collections.Generic;
using System.Linq;
using GubGub.Scripts.Enum;

namespace GubGub.Scripts.Parser
{
    /// <inheritdoc />
    /// <summary>
    /// TSVファイルスクリプトのパーサー
    /// スクリプト中のリソースファイル名のリストを持つ
    /// </summary>
    public class ScenarioParser : IScriptParser
    {
        private static readonly char[] LineBreak = "\r\n".ToCharArray();
        private static readonly char[] CommandDelimiter = "\t".ToCharArray();

        private readonly Dictionary<string, EResourceType> _resourceList =
            new Dictionary<string, EResourceType>();


        /// <summary>
        ///  リソースリストを取得する
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, EResourceType> GetResourceList()
        {
            return _resourceList;
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
            PushResourceName(values);

            return values;
        }

        /// <summary>
        /// 行を分割した配列からリソース名を取得し、リストに追加する
        /// </summary>
        /// <param name="values"></param>
        private void PushResourceName(List<string> values)
        {
            var command = EScenarioCommandTypeExtension.GetEnum(values[0]);

            switch (command)
            {
                case EScenarioCommandType.Image:
                    AddResource(values[1], EResourceType.Background);
                    break;
                case EScenarioCommandType.Bgm:
                    AddResource(values[1], EResourceType.Bgm);
                    break;
                case EScenarioCommandType.Se:
                    AddResource(values[1], EResourceType.Se);
                    break;
                case EScenarioCommandType.Stand:
                    AddResource(values[2], EResourceType.Stand);
                    break;
                case EScenarioCommandType.Unknown:
                    if (values[0] == "")
                    {
                        // 会話メッセージの場合    ボイスパスを取得
                        if (values.Count > 1 && values[1] != null && values[1].Length > 0)
                        {
                            AddResource(values[1], EResourceType.Voice);
                        }
                    }

                    break;
            }
        }

        /// <summary>
        /// リソースリストに追加する
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private void AddResource(string resourceName, EResourceType type)
        {
            if (!string.IsNullOrEmpty(resourceName) && !_resourceList.ContainsKey(resourceName))
            {
                _resourceList.Add(resourceName, type);
            }
        }
    }
}
