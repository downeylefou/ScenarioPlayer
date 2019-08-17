using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GubGub.Scripts.Command;
using GubGub.Scripts.Enum;
using GubGub.Scripts.Lib;
using UniRx;
using UniRx.Async;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GubGub.Scripts.Main
{
    /// <summary>
    ///  シナリオ画面のプレゼンター
    /// </summary>
    public class ScenarioPresenter : MonoBehaviour
    {
        /// <summary>
        /// シナリオ終了を通知するストリーム
        /// </summary>
        public IObservable<Unit> IsEndScenario => _isEndScenario;

        private readonly Subject<Unit> _isEndScenario = new Subject<Unit>();

        /// <summary>
        /// モデルクラス
        /// </summary>
        private ScenarioModel _model = new ScenarioModel();

        /// <summary>
        ///  シナリオコマンド実行クラス
        /// </summary>
        private readonly ScenarioCommandExecutor _commandExecutor = new ScenarioCommandExecutor();

        /// <summary>
        ///  コマンドに対応した処理をビューに行わせるラップクラス
        /// </summary>
        private ScenarioViewMediator _viewMediator;

        /// <summary>
        /// メッセージ表示タイマーのDisposable
        /// </summary>
        private IDisposable _messageTimerDisposable;

        [SerializeField] public ScenarioView view;


        /// <summary>
        /// ビューやパラメータを初期化する
        /// </summary>
        /// <returns></returns>
        public async Task Initialize()
        {
            await view.Initialize();

            // 設定を取得してから初期化
            ConfigManager.Initialize();

            _viewMediator = new ScenarioViewMediator(view, ConfigManager.Config);
            SoundManager.Initialize(ConfigManager.Config);

            InitializeCommandActions();

            Bind();
            AddEventListeners();
        }

        /// <summary>
        /// シナリオプレイヤーを非表示にする
        /// </summary>
        public void Hide()
        {
            _viewMediator.Hide();
        }

        /// <summary>
        /// シナリオ、リソースの読み込みを行う
        /// </summary>
        /// <param name="loadScenarioPath"></param>
        /// <param name="isResourcePreload"></param>
        /// <returns></returns>
        public async Task LoadScenario(string loadScenarioPath, bool isResourcePreload)
        {
            // シナリオの読み込み
            var scenario = await ResourceManager.LoadText(
                ResourceLoadSetting.ScenarioResourcePrefix + loadScenarioPath);

            _model.ParseScenario(scenario);

            // リソースの事前読み込み
            if (isResourcePreload)
            {
                await ResourcePreload();
            }
        }

        /// <summary>
        /// アセットバンドルの事前読み込みを行う
        /// </summary>
        /// <returns></returns>
        private async UniTask ResourcePreload()
        {
            await ResourceManager.StartBulkLoad(_model.GetResourceList());
        }

        /// <summary>
        /// シナリオの再生を開始する
        /// </summary>
        /// <param name="label">再生の起点となるラベル名</param>
        /// <returns></returns>
        public async UniTask StartScenario(string label = null)
        {
            _viewMediator.ResetView();

            gameObject.SetActive(true);
            _viewMediator.Show();

            // ラベルが指定されていなければ、最初から再生する
            if (string.IsNullOrEmpty(label))
            {
                Forward();
            }
            else
            {
                JumpToLabelAndForward(label);
            }

            await Task.CompletedTask;
        }

        private void Bind()
        {
            _viewMediator.onAnyClick.Subscribe(OnAnyClick).AddTo(this);

            _viewMediator.onMouseWheel.Subscribe(OnMouseWheel).AddTo(this);

            // コマンドの終了を監視
            _commandExecutor.commandEnd.Subscribe(_ => { OnCommandEnd(); }).AddTo(this);

            // バックログからのボイス再生通知を監視
            _viewMediator.BackLogPresenter.PlayVoiceStream.Subscribe(PlayVoice).AddTo(this);

            // メッセージの表示完了を監視
            _viewMediator.MessagePresenter.IsEndMessage.Subscribe(_ => OnEndMessage()).AddTo(this);

            // 選択肢のクリックを監視
            _viewMediator.SelectionPresenter.onSelect.Subscribe(OnSelectionClick).AddTo(this);

            // Bgmのボリューム変更を監視
            _viewMediator.ConfigPresenter.changedBgmVolume.Subscribe(OnChangedBgmVolume).AddTo(this);

            // Seのボリューム変更を監視
            _viewMediator.ConfigPresenter.changedSeVolume.Subscribe(OnChangedSeVolume).AddTo(this);
        }

        private void AddEventListeners()
        {
            _viewMediator.MessagePresenter.View.OnConfigButton = OnConfigButton;
            _viewMediator.MessagePresenter.View.OnAutoButton = OnAutoButton;
            _viewMediator.MessagePresenter.View.OnLogButton = OnLogButton;
            _viewMediator.MessagePresenter.View.OnSkipButton = OnSkipButton;
            _viewMediator.MessagePresenter.View.OnCloseButton = OnCloseButton;

            _viewMediator.BackLogPresenter.onTouchDimmer = HideBackLog;
            _viewMediator.ConfigPresenter.onTouchDimmer = HideConfig;
        }

        /// <summary>
        /// 各コマンドに対応したメソッドをコマンド実行クラスに登録する
        /// </summary>
        private void InitializeCommandActions()
        {
            _commandExecutor.AddCommand(EScenarioCommandType.Message, OnMessageCommand);
            _commandExecutor.AddCommand(EScenarioCommandType.ShowWindow, OnShowWindowCommand);
            _commandExecutor.AddCommand(EScenarioCommandType.Stand, OnStandCommand);
            _commandExecutor.AddCommand(EScenarioCommandType.Image, OnImageCommand);
            _commandExecutor.AddCommand(EScenarioCommandType.Wait, OnWaitCommand);
            _commandExecutor.AddCommand(EScenarioCommandType.FadeOut, OnFadeOutCommand);
            _commandExecutor.AddCommand(EScenarioCommandType.FadeIn, OnFadeInCommand);
            _commandExecutor.AddCommand(EScenarioCommandType.Clear, OnClearCommand);
            _commandExecutor.AddCommand(EScenarioCommandType.Se, OnSeCommand);
            _commandExecutor.AddCommand(EScenarioCommandType.Bgm, OnBgmCommand);
            _commandExecutor.AddCommand(EScenarioCommandType.Jump, OnJumpCommand);
            _commandExecutor.AddCommand(EScenarioCommandType.Label, OnLabelCommand);
            _commandExecutor.AddCommand(EScenarioCommandType.Selection, OnSelectionCommand);
            _commandExecutor.AddCommand(EScenarioCommandType.StopScenario, OnStopScenarioCommand);
        }

        #region private method

        /// <summary>
        /// 行番号を進め、シナリオを進行させる
        /// </summary>
        /// <param name="jumpLine">ジャンプする際の行データ</param>
        private void ForwardNextLine(List<string> jumpLine = null)
        {
            _model.AdvanceLineNumber();
            Forward(jumpLine);
        }

        /// <summary>
        /// シナリオを進行させる
        /// </summary>
        /// <param name="jumpLine">ジャンプする際の行データ</param>
        private void Forward(List<string> jumpLine = null)
        {
            var line = _model.GetCurrentLine(jumpLine);

            if (line == null)
            {
                FinishScenario();
                return;
            }

            if (IsValidLine(line))
            {
                if (line[0] != null && line[0].Length > 0)
                {
                    ProcessCommand(line[0], line.Skip(1).ToList());
                }
                else
                {
                    ProcessMessage(line.Skip(1).ToList());
                }
            }
        }

        /// <summary>
        ///  シナリオが終了した
        /// </summary>
        private void FinishScenario()
        {
            SoundManager.StopSound();

            _isEndScenario.OnNext(Unit.Default);
        }

        /// <summary>
        ///  行をコマンドとして処理する
        /// </summary>
        /// <param name="commandName"></param>
        /// <param name="param"></param>
        private void ProcessCommand(string commandName, List<string> param)
        {
            _model.IsWaitProcess = true;
            _commandExecutor.ProcessCommand(commandName, param);
        }

        /// <summary>
        ///  メッセージコマンドを処理する
        /// </summary>
        /// <param name="param"></param>
        private void ProcessMessage(List<string> param)
        {
            _commandExecutor.ProcessCommand(EScenarioCommandType.Message.ToString(), param);
        }

        /// <summary>
        ///  ボイスを再生する
        /// </summary>
        /// <param name="voicePath"></param>
        private void PlayVoice(string voicePath)
        {
            Debug.Log(voicePath);
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
        /// メッセージ表示完了時、一定のディレイ後、オートやスキップ中なら次に進む
        /// </summary>
        private void OnEndMessage()
        {
            _messageTimerDisposable?.Dispose();

            _messageTimerDisposable = Observable
                .Timer(TimeSpan.FromMilliseconds(_model.GetMinMessageWaitTimeMilliSecond()))
                .Subscribe(_ =>
                {
                    _model.IsProcessingShowMessage = false;

                    if (_model.IsAutoForwardable())
                    {
                        ForwardNextLine();
                    }
                }).AddTo(this);
        }

        /// <summary>
        /// コマンド完了時
        /// </summary>
        private void OnCommandEnd()
        {
            if (_model.IsStop)
            {
                return;
            }

            // メッセージコマンドは即終了状態になるが、クリック待ちを行うため、次の行には進まない
            // 選択肢コマンドも同様
            if (!_model.IsProcessingShowMessage && !_model.IsProcessingShowSelection)
            {
                _model.IsWaitProcess = false;

                ForwardNextLine();
            }
        }

        /// <summary>
        /// バックログを非表示にする
        /// </summary>
        private void HideBackLog()
        {
            _viewMediator.HideScenarioLog();
        }

        /// <summary>
        /// コンフィグを非表示にする
        /// </summary>
        private void HideConfig()
        {
            _viewMediator.HideConfig();

            // コンフィグを保存
            ConfigManager.SaveConfig();
        }

        /// <summary>
        /// 指定のラベル名に遷移して、シナリオを進める
        /// </summary>
        /// <param name="labelName"></param>
        private void JumpToLabelAndForward(string labelName)
        {
            if (string.IsNullOrEmpty(labelName))
            {
                return;
            }

            var line = _model.GetLineForJumpToLabel(labelName);
            Forward(line);
        }

        /// <summary>
        /// 選択肢ビューを選択した
        /// </summary>
        private void OnSelectionClick(string labelName)
        {
            _model.IsProcessingShowSelection = false;

            _viewMediator.SelectionPresenter.Clear();
            JumpToLabelAndForward(labelName);
        }

        /// <summary>
        /// メッセージウィンドウと選択肢の表示状態を変更する
        /// </summary>
        private void ChangeViewCloseState(bool isCloseView)
        {
            _model.IsCloseView = isCloseView;
            _viewMediator.ChangeViewVisibleWithCloseState(_model.IsCloseView);
        }

        #endregion

        #region commandAction

        private async Task OnStopScenarioCommand(BaseScenarioCommand value)
        {
            var command = value as StopScenarioCommand;

            _model.StopScenario();
            Hide();

            await Task.CompletedTask;
        }

        private async Task OnLabelCommand(BaseScenarioCommand value)
        {
            // 何もしない
            await Task.CompletedTask;
        }

        private async Task OnSelectionCommand(BaseScenarioCommand value)
        {
            var command = value as SelectionCommand;

            // 次の行も選択肢コマンドでなければ、選択肢表示中フラグを有効にし、
            // 以降のコマンドに進まないようにする
            _model.CheckNextLineOnSelectionCommand(command?.CommandType.GetName());

            _viewMediator.SelectionPresenter.AddSelection(command);
            await Task.CompletedTask;
        }

        private async Task OnJumpCommand(BaseScenarioCommand value)
        {
            var command = value as JumpCommand;
            _model.IsWaitProcess = false;

            JumpToLabelAndForward(command?.LabelName);
            await Task.CompletedTask;
        }

        private async Task OnClearCommand(BaseScenarioCommand value)
        {
            var command = value as ClearCommand;
            await _viewMediator.Clear(command);
        }

        private async Task OnFadeInCommand(BaseScenarioCommand value)
        {
            var command = value as FadeInCommand;
            await _viewMediator.FadeIn(command);
        }

        private async Task OnFadeOutCommand(BaseScenarioCommand value)
        {
            var command = value as FadeOutCommand;
            await _viewMediator.FadeOut(command);
        }

        private async Task OnShowWindowCommand(BaseScenarioCommand value)
        {
            var command = value as ShowWindowCommand;
            _viewMediator.ShowWindow(command);
            await Task.CompletedTask;
        }

        private async Task OnBgmCommand(BaseScenarioCommand value)
        {
            var command = value as BgmCommand;
            SoundManager.PlayBgm(ResourceLoadSetting.BgmResourcePrefix + command?.FileName);
            await Task.CompletedTask;
        }

        private async Task OnSeCommand(BaseScenarioCommand value)
        {
            var command = value as SeCommand;
            SoundManager.PlaySe(ResourceLoadSetting.SeResourcePrefix + command?.FileName);
            await Task.CompletedTask;
        }

        private async Task OnWaitCommand(BaseScenarioCommand value)
        {
            await Task.Delay(
                (value is WaitCommand command) ? command.waitMilliSecond : WaitCommand.DefaultWaitMilliSecond);
        }

        private async Task OnImageCommand(BaseScenarioCommand value)
        {
            var command = value as ImageCommand;
            await _viewMediator.OnShowImage(command?.ImageName,
                command?.FadeTimeMilliSecond ?? ImageCommand.DefaultFadeTimeMilliSecond);
        }

        private async Task OnMessageCommand(BaseScenarioCommand value)
        {
            var command = value as MessageCommand;

            PlayVoice(command?.VoiceName);

            // メッセージ表示開始
            _viewMediator.OnShowMessage(
                _model.GetMessageData(command?.Message, command?.SpeakerName));

            _viewMediator.AddScenarioLog(command);

            _model.IsProcessingShowMessage = true;
            _messageTimerDisposable?.Dispose();

            await Task.CompletedTask;
        }

        private async Task OnStandCommand(BaseScenarioCommand value)
        {
            var command = value as StandCommand;
            await _viewMediator.ShowStand(command);
        }

        #endregion

        #region userInput method

        /// <summary>
        ///  画面中をクリックした
        /// </summary>
        private void OnAnyClick(PointerEventData eventData = null)
        {
            // 中クリックは処理しない
            if (eventData?.button == PointerEventData.InputButton.Middle)
            {
                return;
            }

            var tempIsSkip = _model.IsSkip;

            // ユーザー操作を止めている間も、オートとスキップ状態の解除は行う
            _model.IsAutoPlaying = false;
            _viewMediator.MessagePresenter.SetAutoButtonState(false);

            _model.IsSkip = false;
            _viewMediator.MessagePresenter.SetSkipButtonState(false);

            // クローズ中なら、クローズの解除だけ行う
            if (_model.IsCloseView)
            {
                ChangeViewCloseState(false);
                return;
            }

            // 右クリックはクローズボタンと同等の処理を行う
            if (eventData?.button == PointerEventData.InputButton.Right)
            {
                OnCloseButton();
                return;
            }

            if (_model.IsWaitProcess || _model.IsProcessingShowSelection)
            {
                return;
            }

            _model.IsProcessingShowMessage = false;
            _messageTimerDisposable?.Dispose();

            // スキップ中だったなら表示を止めるだけにして、次には進まない
            if (tempIsSkip)
            {
                return;
            }

            // メッセージ表示更新中なら、すぐに一括表示させる
            if (_viewMediator.MessagePresenter.IsMessageProcess)
            {
                _viewMediator.MessagePresenter.ShowMessageImmediate();
            }
            else
            {
                ForwardNextLine();
            }
        }

        /// <summary>
        /// マウスホイールを行った
        /// </summary>
        /// <param name="axis"></param>
        private void OnMouseWheel(float axis)
        {
            // 下スクロールでページ送り、上スクロールでバックログの表示
            if (axis < 0)
            {
                OnAnyClick();
            }
            else
            {
                OnLogButton();
            }
        }

        /// <summary>
        ///  バックログボタン
        /// </summary>
        private void OnLogButton()
        {
            _viewMediator.ShowScenarioLog();
        }

        /// <summary>
        /// コンフィグボタン
        /// </summary>
        private void OnConfigButton()
        {
            _viewMediator.ShowConfig();
        }

        /// <summary>
        /// オートプレイボタン
        /// </summary>
        /// <param name="isAuto"></param>
        private void OnAutoButton(bool isAuto)
        {
            _model.IsAutoPlaying = isAuto;

            // メッセージ表示タイマー終了後にオートプレイになった場合は、すぐに進める
            if (_model.IsAutoPlaying &&
                !_model.IsProcessingShowMessage &&
                !_model.IsProcessingShowSelection)
            {
                ForwardNextLine();
            }
        }

        /// <summary>
        /// スキップボタン
        /// </summary>
        /// <param name="isSkip"></param>
        private void OnSkipButton(bool isSkip)
        {
            _model.IsSkip = isSkip;

            if (isSkip && !_model.IsWaitProcess)
            {
                // メッセージ表示更新中なら、スキップ表示に変更させる
                if (_viewMediator.MessagePresenter.IsMessageProcess)
                {
                    _viewMediator.MessagePresenter.EnableMessageSkip();
                }
                else if (!_model.IsProcessingShowSelection)
                {
                    // メッセージを表示しきっている状態なので、すぐ次に進める
                    _model.IsProcessingShowMessage = false;
                    ForwardNextLine();
                }
            }
        }

        /// <summary>
        /// メッセージウィンドウのクローズボタン
        /// </summary>
        private void OnCloseButton()
        {
            _model.IsSkip = false;
            _model.IsAutoPlaying = false;

            ChangeViewCloseState(true);
        }

        /// <summary>
        /// Bgmのボリューム値が変更された
        /// </summary>
        /// <param name="volume"></param>
        private void OnChangedBgmVolume(float volume)
        {
            ConfigManager.SetParam(
                EScenarioConfigKey.BgmVolume, volume);
        }

        /// <summary>
        /// Seのボリューム値が変更された
        /// </summary>
        /// <param name="volume"></param>
        private void OnChangedSeVolume(float volume)
        {
            ConfigManager.SetParam(
                EScenarioConfigKey.SeVolume, volume);
        }

        #endregion
    }
}