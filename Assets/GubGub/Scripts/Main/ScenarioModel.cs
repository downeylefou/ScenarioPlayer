using System.Collections.Generic;
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
        /// メッセージ表示用パラメータ
        /// </summary>
        private ScenarioMessageData _messageData = new ScenarioMessageData();

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
            _messageData.SetParam(
                message, speakerName, GetMessageSpeedMilliSecond(), IsSkip);

            return _messageData;
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
    }
}