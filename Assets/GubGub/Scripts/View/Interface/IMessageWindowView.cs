using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GubGub.Scripts.View.Interface
{
    /// <summary>
    /// シナリオのメッセージウィンドウクラスのインターフェイス
    /// </summary>
    public interface IMessageWindowView
    {
        UnityAction OnOptionButton { set; }
        UnityAction OnCloseButton { set; }
        UnityAction OnLogButton { set; }
        UnityAction<bool> OnAutoButton { set; }
        UnityAction OnSkipButton { set; }


        string MessageText { get; set; }
        string NameText { get; set; }
        GameObject gameObject { get; }

        void SetParent(Transform parent, bool worldPositionStays);
      
        Image NextIcon { get; }
        void SetAutoButtonState(bool isAuto);
    }
}
