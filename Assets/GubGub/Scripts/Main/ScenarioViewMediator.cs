using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using GubGub.Scripts.Command;
using GubGub.Scripts.Data;
using GubGub.Scripts.Enum;
using GubGub.Scripts.Lib;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GubGub.Scripts.Main
{
    /// <summary>
    ///  シナリオプレゼンターとビューの仲介クラス
    /// </summary>
    public class ScenarioViewMediator
    {
        #region field

        private const int StandOffsetY = 100;
        private const int StandSideOffset = 300; // 左右の立ち位置の中心を画面中心から振る距離


        /// <summary>
        /// メッセージビューの管理クラス
        /// </summary>
        public ScenarioMessagePresenter MessagePresenter => _view.MessagePresenter;

        /// <summary>
        /// バックログビューの管理クラス
        /// </summary>
        public BackLogPresenter BackLogPresenter => _view.BackLogPresenter;

        /// <summary>
        /// 選択肢ビューの管理クラス
        /// </summary>
        public ScenarioSelectionPresenter SelectionPresenter => _view.SelectionPresenter;

        /// <summary>
        /// 画面をクリックしたことを通知する
        /// </summary>
        public readonly Subject<PointerEventData> onAnyClick = new Subject<PointerEventData>();

        /// <summary>
        ///  立ち位置と、そこに表示されている立ち絵の名前のリスト
        /// </summary>
        private readonly Dictionary<EScenarioStandPosition, string> _currentStandNames =
            new Dictionary<EScenarioStandPosition, string>()
            {
                {EScenarioStandPosition.Left, ""}, {EScenarioStandPosition.Center, ""},
                {EScenarioStandPosition.Right, ""}
            };

        /// <summary>
        ///  現在の背景の名前
        /// </summary>
        private string _currentImageName = "";

        private readonly ScenarioView _view;

        private ScenarioConfigData _configData;

        #endregion

        ///  <summary>
        ///
        ///  </summary>
        ///  <param name="view"></param>
        /// <param name="configData"></param>
        public ScenarioViewMediator(ScenarioView view, ScenarioConfigData configData)
        {
            _view = view;
            SetConfig(configData);

            _view.onAnyClick.Subscribe(onAnyClick);
        }

        #region public command method
                
        /// <summary>
        /// シナリオプレイヤーを表示する
        /// </summary>
        public void Show()
        {
            _view.Show();
        }
        
        /// <summary>
        /// シナリオプレイヤーを非表示にする
        /// </summary>
        public void Hide()
        {
            _view.Hide();
        }
        
        /// <summary>
        /// ビューの表示を初期化する
        /// </summary>
        public void ResetView()
        {
            _view.ResetView();

            // _currentStandNamesを直接foreachで回すと、値が変更できないため、対応する定数リストで回す
            foreach (var position in EScenarioStandPositionExtension.NameList.Keys)
            {
                if (_currentStandNames.ContainsKey(position))
                {
                    _currentStandNames[position] = "";
                }
            }

            _currentImageName = ""; 
        }
        
        /// <summary>
        ///  メッセージウィンドウを表示する
        /// </summary>
        /// <param name="command"></param>
        public void ShowWindow(ShowWindowCommand command)
        {
            _view.ChangeMessageViewPosition(command.Position);
            _view.MessagePresenter.ShowWindow(command.ViewType, command.MarginX, command.MarginY);
        }

        /// <summary>
        ///  メッセージを表示する
        /// </summary>
        /// <param name="messageData"></param>
        /// <returns></returns>
        public void OnShowMessage(ScenarioMessageData messageData)
        {
            _view.ShowMessage(messageData);
        }

        /// <summary>
        ///  背景画像を表示する
        /// </summary>
        /// <param name="commandImageName"></param>
        /// <param name="commandFadeTime"></param>
        /// <returns></returns>
        public async Task OnShowImage(string commandImageName, int commandFadeTime)
        {
            // 同じ画像の場合は何もしない
            if (_currentImageName == commandImageName)
            {
                return;
            }

            _currentImageName = commandImageName;

            // HideImageAndMovie(commandFadeTime);

            var sprite = await GetBackgroundSprite(commandImageName);

            if (sprite)
            {
                var imageObj = new GameObject("BackgroundImage");
                var image = imageObj.AddComponent<Image>();
                image.sprite = sprite;
                image.SetAlpha(0f);

                _view.AddImage(imageObj);
                AdjustImage(imageObj);
                await Fade(image, image.color, commandFadeTime, 1f);
            }
            else
            {
                // displayWarningMessage("指定した背景が見つかりません。 " + cmd.imageName);
            }

            await Task.CompletedTask;
        }

        /// <summary>
        ///  立ち絵を表示する
        /// </summary>
        public async Task ShowStand(StandCommand command)
        {
            if (!EScenarioStandPositionExtension.IsContain(command.Position.ToLower()))
            {
                // displayWarningMessage('無効な立ち位置です。 "' + cmd.position + '"');
                return;
            }

            var position = EScenarioStandPositionExtension.GetEnum(command.Position.ToLower());

            // 同じ場所に同一人物が既に表示されている場合は、表示順を最前面にするのみ
            if (IsSameStandImageAtPosition(command.StandName, position))
            {
                return;
            }

            RemoveStand(position, ClearCommand.DefaultFadeTimeMilliSecond / 2, true);

            var sprite = await GetStandImageSprite(command.StandName);

            if (sprite)
            {
                var imageObj = new GameObject("StandImage");
                var image = imageObj.AddComponent<Image>();
                image.sprite = sprite;
                image.SetAlpha(0f);

                _view.AddStand(imageObj, position);
                AdjustStand(imageObj, position, command);

                await Fade(image, image.color, command.FadeTimeMilliSecond, 1f, command.IsWait);

                _currentStandNames[position] = command.StandName;
            }
            else
            {
                // displayWarningMessage("指定した立ち絵が見つかりません。 " + standName);
            }
        }

        /// <summary>
        ///  フェードアウト処理を行う
        /// </summary>
        /// <param name="command"></param>
        public async Task FadeOut(FadeOutCommand command)
        {
            MessagePresenter.SetInteractable(false);
            await FadeScreen(GetColorByString(command.colorString), command.fadeMilliSecond, command.alpha);
            MessagePresenter.SetInteractable(true);
        }

        /// <summary>
        ///  フェードイン処理を行う
        /// </summary>
        /// <param name="command"></param>
        public async Task FadeIn(FadeInCommand command)
        {
            MessagePresenter.SetInteractable(false);
            await FadeScreen(GetColorByString(command.colorString), command.fadeMilliSecond, command.alpha);
            MessagePresenter.SetInteractable(true);
        }

        /// <summary>
        ///  表示物の消去を行う
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task Clear(ClearCommand command)
        {
            switch (command.ClearTarget)
            {
                case EScenarioClearTargetType.All:
                    // RemoveAllEmotion(command.FadeTimeMilliSecond);
                    await RemoveAllStand(command.FadeTimeMilliSecond);
                    _view.MessagePresenter.ClearText();
                    break;
                case EScenarioClearTargetType.Text:
                    _view.MessagePresenter.ClearText();
                    break;
                case EScenarioClearTargetType.Left:
                case EScenarioClearTargetType.Center:
                case EScenarioClearTargetType.Right:
                    // RemoveEmotion(EScenarioStandPositionExtension.GetEnum(
                    //     command.ClearTarget.GetName(), command.FadeTimeMilliSecond));
                    await RemoveStand(EScenarioStandPositionExtension.GetEnum(
                        command.ClearTarget.GetName()), command.FadeTimeMilliSecond, true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// シナリオバックログにログデータを追加する
        /// </summary>
        /// <param name="command"></param>
        public void AddScenarioLog(MessageCommand command)
        {
            BackLogPresenter.AddScenarioLog(command);
        }
        
        /// <summary>
        /// シナリオバックログを表示する
        /// </summary>
        public void ShowScenarioLog()
        {
            BackLogPresenter.Show();
        }
        
        /// <summary>
        /// シナリオバックログを非表示にする
        /// </summary>
        public void HideScenarioLog()
        {
            BackLogPresenter.Hide();
        }
        
        /// <summary>
        /// クローズ状態かによってビューの表示状態を変更する
        /// </summary>
        /// <param name="isCloseView"></param>
        public void ChangeViewVisibleWithCloseState(bool isCloseView)
        {
            MessagePresenter.ChangeMessageWindowVisible(!isCloseView);
            SelectionPresenter.ChangeVisible(!isCloseView);
        }
        
        #endregion

        #region private method

        /// <summary>
        ///  コンフィグデータを設定
        /// </summary>
        /// <param name="configData"></param>
        private void SetConfig(ScenarioConfigData configData)
        {
            _configData = configData;
            _view.MessageViewType = _configData.MessageViewType;
        }

        /// <summary>
        ///  全ての感情アイコンを消去する
        /// </summary>
        /// <param name="commandFadeTimeMilliSecond"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void RemoveAllEmotion(int commandFadeTimeMilliSecond)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 全ての立ち絵を消去する
        /// </summary>
        /// <param name="fadeTimeMilliSecond"></param>
        /// <returns></returns>
        private async Task RemoveAllStand(int fadeTimeMilliSecond)
        {
            // 全て同時に消したいので、個別に awaitはしない
            RemoveStand(EScenarioStandPosition.Left, fadeTimeMilliSecond, true);
            RemoveStand(EScenarioStandPosition.Center, fadeTimeMilliSecond, true);
            RemoveStand(EScenarioStandPosition.Right, fadeTimeMilliSecond, true);

            await Task.Delay(fadeTimeMilliSecond);
        }

        /// <summary>
        ///  指定位置の立ち絵を消去する
        /// </summary>
        /// <param name="position"></param>
        /// <param name="fadeTimeMilliSecond"></param>
        /// <param name="isWait"></param>
        private async Task RemoveStand(EScenarioStandPosition position,
                                       int fadeTimeMilliSecond = 0,
                                       bool isWait = false)
        {
            var standObj = _view.GetStandObj(position);
            if (standObj)
            {
                var image = standObj.GetComponent<Image>();
                await Fade(image, image.color, fadeTimeMilliSecond, 0f, isWait);
                _view.RemoveStand(position);
            }

            _currentStandNames[position] = "";
        }

        /// <summary>
        /// 指定位置の感情アイコンを消去する
        /// </summary>
        /// <returns></returns>
        private async Task RemoveEmotion(EScenarioStandPosition position,
                                         int fadeTimeMilliSecond = 0,
                                         bool isWait = false)
        {
        }

        /// <summary>
        ///  同じ立ち位置に同じ名前の画像がすでにあるか
        /// </summary>
        /// <param name="standName"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private bool IsSameStandImageAtPosition(string standName, EScenarioStandPosition position)
        {
            return _currentStandNames[position] == standName;
        }

        private void AdjustStand(GameObject standObj, EScenarioStandPosition position, StandCommand command)
        {
            var image = standObj.GetComponent<Image>();

            // サイズ調整
            image.SetNativeSize();

            var rect = standObj.GetComponent<RectTransform>();
            rect.SetScale(Vector3.one);

            // 座標調整
            var xCenter = 0;

            if (position == EScenarioStandPosition.Left)
            {
                xCenter -= StandSideOffset;
            }
            else if (position == EScenarioStandPosition.Right)
            {
                xCenter += StandSideOffset;
            }

            var pos = rect.anchoredPosition3D;
            pos.x = xCenter + command.OffsetX;
            pos.y = StandOffsetY + command.OffsetY;
            pos.z = 0;
            rect.anchoredPosition3D = pos;
        }

        /// <summary>
        ///  画像をストレッチする
        /// </summary>
        /// <param name="imageObj"></param>
        private void AdjustImage(GameObject imageObj)
        {
            var viewRt = imageObj.GetComponent<RectTransform>();
            viewRt.anchorMax = Vector2.one;
            viewRt.anchorMin = Vector2.zero;
            viewRt.anchoredPosition3D = Vector3.zero;
            viewRt.sizeDelta = Vector2.zero;
            viewRt.SetScale(Vector3.one);
        }

        /// <summary>
        ///  背景画像を取得する
        /// </summary>
        /// <param name="imageName"></param>
        /// <returns></returns>
        private async Task<Sprite> GetBackgroundSprite(string imageName)
        {
            return await ResourceManager.LoadSprite(
                ResourceLoadSetting.BackgroundResourcePrefix + imageName);
        }

        /// <summary>
        ///  立ち絵画像を取得する
        /// </summary>
        /// <param name="imageName"></param>
        /// <returns></returns>
        private async Task<Sprite> GetStandImageSprite(string imageName)
        {
            return await ResourceManager.LoadSprite(
                ResourceLoadSetting.StandResourcePrefix + imageName);
        }

        /// <summary>
        ///  16進数の文字列から Colorクラスを取得する
        /// </summary>
        /// <param name="colorString"></param>
        /// <returns></returns>
        private static Color GetColorByString(string colorString)
        {
            var color = default(Color);
            ColorUtility.TryParseHtmlString(colorString, out color);

            return color;
        }

        /// <summary>
        ///  画面全体を覆うフェード処理を行う
        /// </summary>
        /// <param name="fadeColor"></param>
        /// <param name="fadeMilliSecond"></param>
        /// <param name="fadeAlpha"></param>
        /// <returns></returns>
        private async Task FadeScreen(Color fadeColor, int fadeMilliSecond, float fadeAlpha)
        {
            var image = _view.GetFadeImage();
            await Fade(image, fadeColor, fadeMilliSecond, fadeAlpha, true);
        }

        /// <summary>
        ///  対象画像のフェード処理を行う
        /// </summary>
        /// <param name="target"></param>
        /// <param name="fadeColor"></param>
        /// <param name="fadeMilliSecond"></param>
        /// <param name="fadeAlpha"></param>
        /// <param name="isWait"></param>
        /// <returns></returns>
        private async Task Fade(Image target, Color fadeColor, int fadeMilliSecond, float fadeAlpha,
                                bool isWait = false)
        {
            var fadeTime = (float) fadeMilliSecond / 1000;
            fadeColor.a = target.color.a;
            target.color = fadeColor;

            if (isWait)
            {
                await DOTween.ToAlpha(
                    () => target.color,
                    color => target.color = color,
                    fadeAlpha,
                    fadeTime
                );
            }
            else
            {
                DOTween.ToAlpha(
                    () => target.color,
                    color => target.color = color,
                    fadeAlpha,
                    fadeTime
                );
            }
        }

        #endregion


    }
}
