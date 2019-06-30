using UnityEngine;

namespace GubGub.Scripts.Lib
{
    /// <summary>
    /// プレイヤーデータの管理クラス
    /// </summary>
    public static class PlayerDataManager
    {
        /// <summary>
        /// Floatのパラメータをロードする
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static float LoadFloat(string key, float defaultValue)
        {
            return PlayerPrefs.GetFloat(key, defaultValue);
        }
        
        /// <summary>
        /// Floatのパラメータをセーブする
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static void SetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }
        
        /// <summary>
        /// プレイヤーデータを保存する
        /// </summary>
        public static void Save()
        {
            PlayerPrefs.Save();
        }
    }
}