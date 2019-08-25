using System.Collections.Generic;
using System.Text.RegularExpressions;
using GubGub.Scripts.Data;
using GubGub.Scripts.Enum;
using GubGub.Scripts.Lib;
using GubGub.Scripts.Parser;
using UnityEngine;

namespace GubGub.Scripts.Main
{
    /// <summary>
    ///  シナリオのモデルデータ
    /// </summary>
    public class ScenarioModel
    {
        /// <summary>
        ///  オートプレイ中か
        /// </summary>
        public bool IsAutoPlaying { get; set; }

        /// <summary>
        /// スキップ中か
        /// </summary>
        public bool IsSkip { get; set; }

        /// <summary>
        /// メッセージウィンドウや選択肢を非表示にした状態か
        /// </summary>
        public bool IsCloseView { get; set; }

        /// <summary>
        /// 停止中か
        /// </summary>
        public bool IsStop { get; set; }

        /// <summary>
        ///  コマンド処理中にユーザー入力を止めるためのフラグ
        /// </summary>
        public bool IsWaitProcess { get; set; }

        /// <summary>
        /// メッセージ表示中フラグ
        /// コマンド処理中フラグとは別に、シナリオの進行を止める
        /// </summary>
        public bool IsProcessingShowMessage { get; set; }

        /// <summary>
        /// メッセージ表示中フラグ
        /// コマンド処理中フラグとは別に、シナリオの進行と入力処理を止める
        /// </summary>
        public bool IsProcessingShowSelection { get; set; }

        /// <summary>
        /// TSVファイルスクリプトのパーサー
        /// </summary>
        private readonly ScenarioParser _parser = new ScenarioParser();

        /// <summary>
        ///  パース済みのテキスト配列
        /// </summary>
        private ScenarioParseData _parseData;

        /// <summary>
        /// シナリオ再生用のコンフィグ
        /// </summary>
        private ScenarioConfigData Config => ConfigManager.Config;

        /// <summary>
        ///  現在参照中のスクリプト行
        /// </summary>
        private List<string> _currentLine;

        /// <summary>
        /// ゲーム側と受け渡しを行うパラメータリスト
        /// </summary>
        private readonly Dictionary<string, ScenarioParameter> _params = new Dictionary<string, ScenarioParameter>();

        /// <summary>
        /// メッセージ表示用パラメータ
        /// </summary>
        private readonly ScenarioMessageData _messageData = new ScenarioMessageData();


        /// <summary>
        /// シナリオ内に含まれるリソースの、パス・タイプのリストを取得する
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, EResourceType> GetResourceList()
        {
            return _parser.GetResourceList();
        }

        /// <summary>
        /// 読み込んだテキストアセットをシナリオデータにパースする
        /// </summary>
        /// <param name="scenario"></param>
        public void ParseScenario(TextAsset scenario)
        {
            List<List<string>> list = _parser.ParseScript("", scenario.text);

            _parseData = new ScenarioParseData(list);
        }

        /// <summary>
        /// シナリオの行番号を進める
        /// </summary>
        public void AdvanceLineNumber()
        {
            _parseData.AdvanceLineNumber();
        }

        /// <summary>
        /// 現在の行を取得する
        /// </summary>
        /// <param name="jumpLine"></param>
        /// <returns></returns>
        public List<string> GetCurrentLine(List<string> jumpLine = null)
        {
            _currentLine = jumpLine ?? _parseData.GetCurrentLine();

            if (_currentLine == null || _currentLine.Count <= 0)
            {
                return null;
            }

            return _currentLine;
        }

        /// <summary>
        /// 指定ラベルにジャンプし、行の情報を取得する
        /// </summary>
        /// <param name="labelName"></param>
        /// <returns></returns>
        public List<string> GetLineForJumpToLabel(string labelName)
        {
            return _parseData.GetLineForJumpToLabel(labelName);
        }

        /// <summary>
        /// メッセージ表示後、自動的に次に進める状態かを取得する
        /// </summary>
        /// <returns></returns>
        public bool IsAutoForwardable()
        {
            return (IsAutoPlaying || IsSkip) && !IsProcessingShowSelection;
        }

        /// <summary>
        /// メッセージ表示完了タイマーのウェイト時間を取得する
        /// </summary>
        /// <returns></returns>
        public int GetMinMessageWaitTimeMilliSecond()
        {
            return IsSkip ? Config.MinSkipWaitTimeMilliSecond : Config.MinAutoWaitTimeMilliSecond;
        }

        /// <summary>
        /// 次の行が選択肢コマンドでなければ、選択肢表示中フラグを有効にする
        /// </summary>
        /// <param name="commandName"></param>
        public void CheckNextLineOnSelectionCommand(string commandName)
        {
            if (!_parseData.GetIsMatchNextLineCommandName(commandName))
            {
                IsProcessingShowSelection = true;
            }
        }

        /// <summary>
        /// メッセージ表示モデルにパラメータを設定し、取得する
        /// </summary>
        /// <param name="message"></param>
        /// <param name="speakerName"></param>
        public ScenarioMessageData GetMessageData(string message, string speakerName)
        {
            message = GetParameterReplacedMessage(message);

            _messageData.SetParam(
                message, speakerName, GetMessageSpeedMilliSecond(), IsSkip);

            return _messageData;
        }

        /// <summary>
        /// 元の文字列から、パラメータを置換した文字列を取得する
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private string GetParameterReplacedMessage(string message)
        {
            const string parameterPattern = @"(?<=\@).*(?=\/@)";

            var matches = Regex.Matches(message, parameterPattern);
            foreach (Match match in matches)
            {
                var param = GetParameter<object>(match.Value) ?? "";
                var replacePattern = @"@" + match.Value + @"/@";

                message = message.Replace(replacePattern, param.ToString());
            }

            return message;
        }

        /// <summary>
        /// パラメータを設定する
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        public void SetParameter<T>(string key, T value)
        {
            ScenarioParameter param;

            if (!_params.ContainsKey(key))
            {
                param = new ScenarioParameter();
                _params.Add(key, param);
            }

            param = _params[key];
            param.SetValue(value);
        }

        /// <summary>
        /// パラメータを取得する
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetParameter<T>(string key)
        {
            if (_params.ContainsKey(key))
            {
                return _params[key].GetParameter<T>();
            }

            Debug.LogWarning("Undefined parameter : " + key);
            
            if (typeof(T) == typeof(string))
            {
                return (T) (object) "";
            }

            return default;
        }

        /// <summary>
        /// メッセージの一文字あたりの表示速度を取得する
        /// </summary>
        /// <returns></returns>
        private int GetMessageSpeedMilliSecond()
        {
            if (IsSkip)
            {
                return Config.SkipMessageSpeedMilliSecond;
            }

            if (IsAutoPlaying)
            {
                return Config.AutoMessageSpeedMilliSecond;
            }

            return Config.MessageSpeedMilliSecond;
        }

        /// <summary>
        /// 停止状態にする
        /// </summary>
        public void StopScenario()
        {
            IsStop = true;
            IsSkip = false;
            IsCloseView = false;
            IsAutoPlaying = false;
            IsWaitProcess = false;
            IsProcessingShowMessage = false;
            IsProcessingShowSelection = false;
        }

        /// <summary>
        /// シナリオの停止状態を解除する
        /// </summary>
        public void ReleaseStopState()
        {
            IsStop = false;
        }
    }
}