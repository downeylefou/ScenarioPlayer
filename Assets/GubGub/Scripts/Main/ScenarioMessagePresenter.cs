using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GubGub.Scripts.Enum;
using GubGub.Scripts.Lib;
using GubGub.Scripts.View;
using GubGub.Scripts.View.Interface;
using UniRx;
using UnityEngine;

namespace GubGub.Scripts.Main
{
    /// <summary>
    ///  シナリオのメッセージビューを管理する
    /// </summary>
    public class ScenarioMessagePresenter : MonoBehaviour
    {
        #region field

        /// <summary>
        /// メッセージの表示完了を通知する
        /// </summary>
        public Subject<Unit> IsEndMessage => _isEndMessage;

        private readonly Subject<Unit> _isEndMessage = new Subject<Unit>();


        /// <summary>
        ///  メッセージ送り中か
        /// </summary>
        public bool IsMessageProcess { get; private set; }

        /// <summary>
        /// 実際に表示されるメッセージウィンドウ
        /// </summary>
        public IMessageWindowView View => CurrentView;

        /// <summary>
        /// デフォルトのメッセージウィンドウ
        /// </summary>
        [SerializeField] private DefaultMessageWindowView defaultMessageWindowView;

        /// <summary>
        /// 入力されたテキストのビルダー
        /// </summary>
        private readonly StringBuilder _originalMessage = new StringBuilder(200);
        
        /// <summary>
        /// 出力するテキストのビルダー
        /// </summary>
        private readonly StringBuilder _viewMessage = new StringBuilder(200);

        private IDisposable _messageTimer;

        private EScenarioMessageViewType _type = EScenarioMessageViewType.Default;

        private int _currentCharIndex;

        private IMessageWindowView CurrentView { get; set; }

        /// <summary>
        ///  メッセージ中のタグに対応する終了タグ
        /// </summary>
        private readonly Dictionary<char, string> _endTags = new Dictionary<char, string>()
        {
            //  4種のタグに対応：<b>, <i>, <color=#FFFFFF, white>, <size=24>,
            {'b', "</b>"}, {'i', "</i>"}, {'c', "</color>"}, {'s', "</size>"}
        };

        /// <summary>
        ///  メッセージの末尾に挿入する終了タグリスト
        /// </summary>
        private readonly List<string> _tmpCloseTags = new List<string>();

        #endregion

        /// <summary>
        /// メッセージウィンドウを初期化
        /// </summary>
        /// <returns></returns>
        public async Task Initialize()
        {
            CurrentView = defaultMessageWindowView;

            ClearText();

            await Task.CompletedTask;
        }

        /// <summary>
        /// オートボタンのトグル状態を設定する
        /// </summary>
        /// <param name="isAuto"></param>
        public void SetAutoButtonState(bool isAuto)
        {
            CurrentView.SetAutoButtonState(isAuto);
        }
        
        /// <summary>
        /// スキップボタンのトグル状態を設定する
        /// </summary>
        /// <param name="isAuto"></param>
        public void SetSkipButtonState(bool isAuto)
        {
            CurrentView.SetSkipButtonState(isAuto);
        }
        
        /// <summary>
        ///  マージン座標を指定してウィンドウを表示させる
        /// </summary>
        /// <param name="type"></param>
        /// <param name="marginX"></param>
        /// <param name="marginY"></param>
        public void ShowWindow(EScenarioMessageViewType type, int marginX, int marginY)
        {
            ChangeWindow(type);
            SetMessageViewPosition(CurrentView, marginX, marginY);
        }

        /// <summary>
        ///  メッセージビューの親オブジェクトを設定する
        /// </summary>
        /// <param name="parent"></param>
        public void SetViewParent(Transform parent)
        {
            // gameObject.transform.SetParent(parent, true);
            CurrentView.SetParent(parent, true);
            CurrentView.gameObject.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
        }

        /// <summary>
        /// メッセージと話者名を初期化する
        /// </summary>
        public void ClearText()
        {
            CurrentView.MessageText = "";
            CurrentView.NameText = "";
        }

        /// <summary>
        ///  メッセージを表示する
        /// </summary>
        /// <param name="message"></param>
        /// <param name="speakerName"></param>
        /// <param name="messageSpeed"></param>
        public void ShowMessage(string message, string speakerName = null, int messageSpeed = 30)
        {
            CurrentView.NameText = speakerName ?? "";

            InitializeParameter();
            _originalMessage.Append(message);
            CurrentView.MessageText = "";

            SetPageBreakIconVisible(false);

            IsMessageProcess = true;
            _messageTimer = Observable.Interval(TimeSpan.FromMilliseconds(messageSpeed))
                .TakeWhile(_ => IsMessageProcess)
                .Subscribe(_ => OnMessageTimer()).AddTo(this);
        }

        /// <summary>
        ///  文字送りを省き、メッセージを一気に表示させる
        /// </summary>
        public void ShowMessageImmediate()
        {
            OnEndMessageProcess();
        }

        /// <summary>
        /// ウィンドウタイプよってウィンドウを切り替える
        /// </summary>
        /// <param name="type"></param>
        private void ChangeWindow(EScenarioMessageViewType type)
        {
            if (_type != type)
            {
                // 複数のウィンドウを使用したい場合、ここでCurrentViewを変更する
                
                // ウィンドウの表示を初期化す
                ClearText();
                CurrentView.gameObject.SetActive(false);
                SetMessageViewPosition(CurrentView);
                _type = type;
            }

            CurrentView.gameObject.SetActive(true);
        }
        
        /// <summary>
        ///  タイマーのカウントごとにメッセージを1文字ずつ表示する
        ///  タグ文字に対しては、1文字ずつ、対応する終了タグを表示用に挿入する
        /// </summary>
        private void OnMessageTimer()
        {
            if (IsMessageProcess)
            {
                DecideCurrentCharIndex();

                // 0 ~ 現在のインデックスまでの文字 + 終了タグをメッセージとして表示する
                _viewMessage.Append(_originalMessage.ToString().Substring(0, _currentCharIndex + 1));
                _viewMessage.Append(string.Join("", _tmpCloseTags));
                CurrentView.MessageText = _viewMessage.ToString();

                _viewMessage.Clear();
                _currentCharIndex++;

                // 全ての文字を表示しきった
                if (_currentCharIndex >= _originalMessage.Length)
                {
                    OnEndMessageProcess();
                }
            }
        }

        /// <summary>
        /// タグをメッセージ1文字分として、次のタイミングのメッセージ表示indexを更新する
        /// </summary>
        private void DecideCurrentCharIndex()
        {
            if (_currentCharIndex >= _originalMessage.Length || _originalMessage[_currentCharIndex] != '<')
            {
                return;
            }

            var nextChar = (_originalMessage.Length >= _currentCharIndex + 1)
                ? _originalMessage[_currentCharIndex + 1]
                : '_';

            if (nextChar == '/')
            {
                // 対応する終了タグを削除する
                _tmpCloseTags.Remove(GetCloseTag(_originalMessage[_currentCharIndex + 2]));
            }
            else
            {
                // 対応する終了タグを先頭に追加する
                _tmpCloseTags.Insert(0, GetCloseTag(_originalMessage[_currentCharIndex + 1]));
            }

            var tagEndIndex = _originalMessage.ToString().IndexOf('>', _currentCharIndex);
            _currentCharIndex = tagEndIndex;
        }

        /// <summary>
        ///  開始タグの1文字目に対応する終了タグを取得する
        /// </summary>
        /// <param name="tagFirstChar"></param>
        /// <returns></returns>
        private string GetCloseTag(char tagFirstChar)
        {
            if (_endTags.ContainsKey(tagFirstChar))
            {
                return _endTags[tagFirstChar];
            }

            return "";
        }

        /// <summary>
        ///  全ての文字を表示したので、一時パラメータを初期化し、改行アイコンを表示させる
        /// </summary>
        private void OnEndMessageProcess()
        {
            CurrentView.MessageText = _originalMessage.ToString();
            InitializeParameter();

            SetPageBreakIconVisible(true);
            _isEndMessage.OnNext(Unit.Default);
        }

        /// <summary>
        ///  パラメータの初期化
        /// </summary>
        private void InitializeParameter()
        {
            IsMessageProcess = false;

            _messageTimer?.Dispose();
            _currentCharIndex = 0;
            _tmpCloseTags.Clear();
            _viewMessage.Clear();
            _originalMessage.Clear();
        }

        /// <summary>
        ///  改ページアイコンの表示状態を設定する
        /// </summary>
        /// <param name="value"></param>
        private void SetPageBreakIconVisible(bool value)
        {
            CurrentView.NextIcon.enabled = value;
        }

        /// <summary>
        ///  メッセージビューの座標を変更する
        /// </summary>
        /// <param name="view"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        private void SetMessageViewPosition(IMessageWindowView view, float posX = 0, float posY = 0)
        {
            view.gameObject.GetComponent<RectTransform>().SetXy(posX, posY);
        }
    }
}
