using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GubGub.Scripts.Command;
using GubGub.Scripts.Data;
using GubGub.Scripts.Enum;
using GubGub.Scripts.Lib;
using GubGub.Scripts.Parser;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

namespace GubGub.Scripts.Main
{
    /// <summary>
    ///  シナリオ画面のプレゼンター
    /// </summary>
    public class ScenarioPresenter : MonoBehaviour
    {

        /// <summary>
        /// TSVファイルスクリプトのパーサー
        /// </summary>
        private readonly ScenarioParser _parser = new ScenarioParser();

        /// <summary>
        ///  コマンド処理の終了が通知されるストリーム
        /// </summary>
        private readonly Subject<Unit> _commandEnd = new Subject<Unit>();

        /// <summary>
        ///  コマンドタイプに応じたシナリオコマンドを返すクラス
        /// </summary>
        private readonly CommandPalette _commandPalette = new CommandPalette();

        /// <summary>
        ///  バックログからボイス再生パスを通知されるストリーム
        /// </summary>
        private readonly Subject<string> _playVoicePathStream = new Subject<string>();

        /// <summary>
        ///  コマンドに対応した処理をビューに行わせるラップクラス
        /// </summary>
        private ScenarioViewMediator _viewMediator;

        /// <summary>
        ///  コマンドに対応した関数リスト
        /// </summary>
        private Dictionary<EScenarioCommandType, UnityAction<BaseScenarioCommand>> _commandActions;

        /// <summary>
        ///  パース済みのテキスト配列
        /// </summary>
        private ScenarioParseData _parseData;

        /// <summary>
        ///  シナリオ再生時の各種設定
        /// </summary>
        private ScenarioConfigData _configData;

        /// <summary>
        ///  現在実行中のコマンド
        /// </summary>
        private BaseScenarioCommand _currentCommand;

        /// <summary>
        ///  現在参照中のスクリプト行
        /// </summary>
        private List<string> _currentLine;

        /// <summary>
        ///  オートプレイ中か
        /// </summary>
        private bool _isAutoPlaying;

        /// <summary>
        ///  コマンド処理中にユーザー入力を止めるためのフラグ
        /// </summary>
        private bool _isWaitProcess;


        [SerializeField]
        public ScenarioView view;

        private async void Awake()
        {
            await Initialize();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public async Task Initialize()
        {
            await view.Initialize();
            
            _configData = new ScenarioConfigData();
            _viewMediator = new ScenarioViewMediator(view, _configData);

            await InitializeLogDialog();
            InitializeCommandActions();
            
            await InitializeScenarioParseData();

            // Bind();
            AddEventListeners();

            Forward();
        }

        private void AddEventListeners()
        {
            _viewMediator.MessagePresenter.View.OnOptionButton = OnOptionButton;
            _viewMediator.MessagePresenter.View.OnAutoButton = OnAutoButton;
            _viewMediator.MessagePresenter.View.OnLogButton = OnLogButton;

            _viewMediator.onAnyClick.Subscribe(_ => OnAnyClick()).AddTo(this);

            // コマンドの終了を監視
            _commandEnd.Subscribe(_ => { Forward(); }).AddTo(this);

            // バックログからのボイス再生通知を監視
            _playVoicePathStream.Subscribe(_ => { Debug.Log(_.ToString()); }).AddTo(this);

//            _logDataStream.Subscribe(data => _logDialog.UpdateLog(data)).AddTo(this);
        }

        /// <summary>
        ///  バックログダイアログを初期化する
        /// </summary>
        private async Task InitializeLogDialog()
        {
//            _logDialog = await ScenarioLogDialogView.Create();
//            await _logDialog.Initialize(View.transform);
        }

        /// <summary>
        ///  各コマンドに対応した関数リストを生成
        /// </summary>
        private void InitializeCommandActions()
        {
            _commandActions = new Dictionary<EScenarioCommandType, UnityAction<BaseScenarioCommand>>()
            {
                {EScenarioCommandType.Message, OnMessageCommand},
                {EScenarioCommandType.ShowWindow, OnShowWindowCommand},
                {EScenarioCommandType.Stand, OnStandCommand},
                {EScenarioCommandType.Image, OnImageCommand},
                {EScenarioCommandType.Wait, OnWaitCommand},
                {EScenarioCommandType.FadeOut, OnFadeOutCommand},
                {EScenarioCommandType.FadeIn, OnFadeInCommand},
                {EScenarioCommandType.Clear, OnClearCommand},
            };
        }

        /// <summary>
        ///  テキストをパースしてリスト化する
        /// </summary>
        private async Task InitializeScenarioParseData()
        {
            var scenario = await ResourceManager.LoadText("test_scenario");
            List<List<string>> list = _parser.ParseScript("", scenario.text);

            _parseData = new ScenarioParseData(list);
        }

        #region private method

        /// <summary>
        ///  シナリオを進行させる
        /// </summary>
        private void Forward()
        {
            _currentLine = _parseData.GetCurrentLineAndAdvanceNumber();

            if (_currentLine == null || _currentLine.Count <= 0)
            {
                FinishScenario();
                return;
            }

            if (IsValidLine(_currentLine))
            {
                if (_currentLine[0] != null && _currentLine[0].Length > 0)
                {
                    ProcessCommand(_currentLine[0], _currentLine.Skip(1).ToList());
                }
                else
                {
                    ProcessMessage(_currentLine.Skip(1).ToList());
                }
            }
        }

        /// <summary>
        ///  シナリオを自動で進行させる
        /// </summary>
        private void AutoForward()
        {
            Forward();
        }

        /// <summary>
        ///  シナリオが終了した
        /// </summary>
        private void FinishScenario()
        {
        }

        /// <summary>
        ///  行をコマンドとして処理する
        /// </summary>
        /// <param name="commandName"></param>
        /// <param name="param"></param>
        private void ProcessCommand(string commandName, List<string> param)
        {
            _currentCommand = _commandPalette.GetCommand(commandName);
            _currentCommand.Initialize(param);

            SetIsWaitProcessState(true);
            _commandActions[_currentCommand.CommandType].Invoke(_currentCommand);
        }

        /// <summary>
        ///  メッセージコマンドを処理する
        /// </summary>
        /// <param name="param"></param>
        private void ProcessMessage(List<string> param)
        {
            // ProcessCommand(EScenarioCommandType.Message.GetName(), param);
            _currentCommand = _commandPalette.GetCommand(EScenarioCommandType.Message.ToString());
            _currentCommand.Initialize(param);
            _commandActions[EScenarioCommandType.Message].Invoke(_currentCommand);
        }

        /// <summary>
        ///  ボイスを再生する
        /// </summary>
        /// <param name="voiceName"></param>
        /// <param name="speakerName"></param>
        private void PlayVoice(string voiceName, string speakerName)
        {
        }

        /// <summary>
        /// 引数のテキストリストが有効なコマンド行かどうか調べる
        /// 中身に空の文字列しかない場合、無効なコマンドとする
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private static bool IsValidLine(IEnumerable<string> line)
        {
            return line.Any(value => !string.IsNullOrEmpty(value));
        }

        /// <summary>
        ///  ユーザー入力を止めるための処理待ちフラグを設定する
        /// </summary>
        /// <param name="value"></param>
        private void SetIsWaitProcessState(bool value)
        {
            _isWaitProcess = value;
        }

        #endregion

        #region commandAction

        private void OnCommandEnd()
        {
            SetIsWaitProcessState(false);
            _commandEnd.OnNext(Unit.Default);
        }


        private async void OnClearCommand(BaseScenarioCommand value)
        {
            var command = value as ClearCommand;
            await _viewMediator.Clear(command);
            OnCommandEnd();
        }

        private async void OnFadeInCommand(BaseScenarioCommand value)
        {
            var command = value as FadeInCommand;
            await _viewMediator.FadeIn(command);
            OnCommandEnd();
        }

        private async void OnFadeOutCommand(BaseScenarioCommand value)
        {
            var command = value as FadeOutCommand;
            await _viewMediator.FadeOut(command);
            OnCommandEnd();
        }

        private async void OnShowWindowCommand(BaseScenarioCommand value)
        {
            var command = value as ShowWindowCommand;
            _viewMediator.ShowWindow(command);
            OnCommandEnd();
        }

        private async void OnWaitCommand(BaseScenarioCommand value)
        {
            var command = value as WaitCommand;
            await Task.Delay(command.waitMilliSecond);
            OnCommandEnd();
        }

        private async void OnImageCommand(BaseScenarioCommand value)
        {
            var command = value as ImageCommand;
            await _viewMediator.OnShowImage(command?.ImageName, command.FadeTimeMilliSecond);
            OnCommandEnd();
        }

        private async void OnMessageCommand(BaseScenarioCommand value)
        {
            var command = value as MessageCommand;
            PlayVoice(command?.VoiceName, command?.SpeakerName);

            // オートプレイ処理
            // TODO: ボイスの再生待ちも条件に加える
            if (_isAutoPlaying && command != null)
            {
                var waitTime = Mathf.Max(
                    _configData.AutoMessageSpeedMilliSecond * command.Message.Length,
                    _configData.MinAutoWaitTimeMilliSecond);
                Observable.Timer(TimeSpan.FromSeconds(waitTime / 1000f)).Subscribe(_ =>
                {
                    if (_isAutoPlaying)
                    {
                        OnCommandEnd();
                    }
                });
            }

            await _viewMediator.OnShowMessage(
                command?.Message, command?.SpeakerName, _configData.MessageSpeedMilliSecond);
        }

        private async void OnStandCommand(BaseScenarioCommand value)
        {
            var command = value as StandCommand;
            await _viewMediator.ShowStand(command);
            OnCommandEnd();
        }

        #endregion

        #region userInput method

        /// <summary>
        ///  画面中をクリックした
        /// </summary>
        private void OnAnyClick()
        {
            if (_isWaitProcess)
            {
                return;
            }

            _isAutoPlaying = false;

            // メッセージ表示更新中なら、すぐに一括表示させる
            if (_viewMediator.MessagePresenter.IsMessageProcess)
            {
                _viewMediator.MessagePresenter.ShowMessageImmediate();
            }
            else
            {
                Forward();
                Debug.Log("OnAnyClick");
            }
        }

        /// <summary>
        ///  バックログボタン
        /// </summary>
        private async void OnLogButton()
        {
//            ShowBackLogDialog();
        }

        private void OnOptionButton()
        {
            Debug.Log("OnOptionButton");
        }

        private void OnAutoButton()
        {
            _isAutoPlaying = !_isAutoPlaying;
            if (_isAutoPlaying)
            {
                AutoForward();
            }
        }

        #endregion
    }
}
