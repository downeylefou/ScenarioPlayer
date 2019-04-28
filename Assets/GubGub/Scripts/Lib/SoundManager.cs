using UnityEngine;

namespace GubGub.Scripts.Lib
{
    /// <summary>
    ///  サウンドマネージャー
    /// </summary>
    public class SoundManager : MonoBehaviour
    {
        #region singleton
    
        private static SoundManager _instance = null;
    
        private static SoundManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType(typeof(SoundManager)) as SoundManager;
                }
    
                return _instance;
            }
        }
    
        #endregion
    
        [SerializeField] private AudioSource bgmSource;
    
        [SerializeField] private AudioSource seSource;
    
    
        /// <summary>
        /// 全てのサウンドを停止する
        /// </summary>
        public static void StopSound()
        {
            StopBgm();
            StopSe();
        }
        
        /// <summary>
        /// BGMを停止する
        /// </summary>
        public static void StopBgm()
        {
            Instance.bgmSource.Stop();
        }

        /// <summary>
        /// SEを停止する
        /// </summary>
        public static void StopSe()
        {
            Instance.seSource.Stop();
        }
    
        /// <summary>
        ///  BGMを再生する
        /// </summary>
        /// <param name="fileName"></param>
        public static async void PlayBgm(string fileName)
        {
            var clip = await ResourceManager.LoadSound(fileName);
    
            if (clip != null)
            {
                Instance.bgmSource.clip = clip;
                Instance.bgmSource.Play();
            }
        }
    
        /// <summary>
        ///  SEを再生する
        /// </summary>
        /// <param name="fileName"></param>
        public static async void PlaySe(string fileName)
        {
            var clip = await ResourceManager.LoadSound(fileName);
    
            if (clip != null)
            {
                Instance.seSource.PlayOneShot(clip);
            }
        }
    }
}
