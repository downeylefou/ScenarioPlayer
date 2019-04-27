﻿using System;
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

        // 仮のアセットバンドルのパス
        private const string StandFilePrefix = "characters/";
        private const string StandPassSuffix = "/stand.png";

        private const string ImageFilePrefix = "backgrounds/";
        // private const string ImagePassSuffix = "/stand.png";

        private const int StandOffsetY = 100;
        private const int StandSideOffset = 300; // 左右の立ち位置の中心を画面中心から振る距離


        public ScenarioMessagePresenter MessagePresenter => _view.MessagePresenter;

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
        /// <param name="message"></param>
        /// <param name="speakerName"></param>
        /// <param name="messageSpeed"></param>
        /// <returns></returns>
        public async Task OnShowMessage(string message, string speakerName, int messageSpeed)
        {
            _view.ShowMessage(message, speakerName, messageSpeed);

            await Task.CompletedTask;
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
            await FadeScreen(GetColorByString(command.colorString), command.fadeMilliSecond, command.alpha);
        }

        /// <summary>
        ///  フェードイン処理を行う
        /// </summary>
        /// <param name="command"></param>
        public async Task FadeIn(FadeInCommand command)
        {
            await FadeScreen(GetColorByString(command.colorString), command.fadeMilliSecond, command.alpha);
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

//            float sizeAdjustPer = 0.6f;
//            float spriteWidth = image.sprite.rect.width * sizeAdjustPer;
//            float spriteHeight = image.sprite.rect.height * sizeAdjustPer;
//
            var rect = standObj.GetComponent<RectTransform>();
//            rect.anchorMax = rect.anchorMin;
//            rect.sizeDelta = new Vector2(spriteWidth, spriteHeight);
//            rect.SetScale(Vector3.one);


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

            var pos = rect.anchoredPosition;
            pos.x = xCenter + command.OffsetX;
            pos.y = StandOffsetY + command.OffsetY;
            rect.anchoredPosition = pos;
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
//            var resObj = await ResourceManager.LoadResource(ImageFilePrefix + imageName).ToTask();

//            return resObj.GetResource<Sprite>(ImageFilePrefix + imageName);
//            return null;

            return await ResourceManager.LoadSprite(ImageFilePrefix + imageName);

        }

        /// <summary>
        ///  立ち絵画像を取得する
        /// </summary>
        /// <param name="imageName"></param>
        /// <returns></returns>
        private async Task<Sprite> GetStandImageSprite(string imageName)
        {
            // TODO:ResourceManager

//            var resObj = await ResourceManager.Instance
//                .LoadResource(StandFilePrefix + imageName + StandPassSuffix).ToTask();

//            return resObj.GetResource<Sprite>(StandFilePrefix + imageName + StandPassSuffix);
            return await ResourceManager.LoadSprite(StandFilePrefix + imageName);
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