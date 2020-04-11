using System.Collections.Generic;
using System.Linq;
using Sample._1_Adventure.Scripts.Data;
using Sample._1_Adventure.Scripts.Util;
using UniRx;
using UniRx.Async;
using UnityEngine;

namespace Sample._1_Adventure.Scripts.Map
{
    /// <summary>
    /// 各マップの表示系を担当する
    /// </summary>
    public class AdvMapPresenter : MonoBehaviour
    {
        [SerializeField] private BackgroundPresenter backgroundPresenter;

        /// <summary>
        /// マップ番号の変更を通知する
        /// </summary>
        private readonly Subject<int> onChangeMapSubject = new Subject<int>();

        private IEnumerable<IAdvMap> maps;

        public void Initialize()
        {
            maps = GetComponentsInChildren<IAdvMap>(true);
            if (maps == null)
            {
                Debug.LogError("マップが見つかりません");
                return;
            }

            foreach (var advMap in maps)
            {
                advMap.ShowMap();
                advMap.Initialize(onChangeMapSubject);
                advMap.HideMap();
            }

            onChangeMapSubject.Subscribe(async _ => await OnChangeMap(_)).AddTo(this);
        }

        /// <summary>
        /// マップ表示を開始する
        /// </summary>
        /// <returns></returns>
        public async UniTask StartMap()
        {
            backgroundPresenter.ChangeBackground(AdvParameter.CurrentMapNumber);

            GetCurrentMap().ShowMap();
            GetCurrentMap().OnBeforeStart();
            await LoadingUtil.Hide();
            GetCurrentMap().OnStart();
        }

        /// <summary>
        /// マップ番号が変更された
        /// </summary>
        /// <param name="mapNumber"></param>
        /// <returns></returns>
        private async UniTask OnChangeMap(int mapNumber)
        {
            await LoadingUtil.Show();
            GetCurrentMap().HideMap();

            AdvParameter.CurrentMapNumber = mapNumber;
            await StartMap();
        }

        /// <summary>
        /// 現在のマップ番号のマップを取得する
        /// </summary>
        /// <returns></returns>
        private IAdvMap GetCurrentMap()
        {
            return maps.FirstOrDefault(_ => _.GetMapNumber() == AdvParameter.CurrentMapNumber);
        }
    }
}