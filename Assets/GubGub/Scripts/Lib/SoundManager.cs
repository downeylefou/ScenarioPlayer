using GubGub.Scripts.Data;
using UniRx;
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

        private ScenarioConfigData _config;

        /// <summary>
        /// インスペクタから操作するためのBGMボリューム
        /// </summary>
        [RangeReactivePropertyAttribute(0, 1)]
        [SerializeField] private FloatReactiveProperty bgmVolume = new FloatReactiveProperty(1f); 
        
        /// <summary>
        /// インスペクタから操作するためのSEボリューム
        /// </summary>
        [RangeReactivePropertyAttribute(0, 1)]
        [SerializeField] private FloatReactiveProperty seVolume = new FloatReactiveProperty(1f); 

        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <param name="config"></param>
        public static void Initialize(ScenarioConfigData config)
        {
            Instance._config = config;
            Instance.Bind();
            
            ChangeBgmVolume(Instance._config.bgmVolume.Value);
            ChangeSeVolume(Instance._config.seVolume.Value);
        }
        
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
                Instance.bgmSource.loop = true;
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
        
        /// <summary>
        /// BGMのボリュームを変更する
        /// </summary>
        /// <param name="volume"></param>
        private static void ChangeBgmVolume(float volume)
        {
            Instance.bgmSource.volume = volume;
        }
        
        /// <summary>
        /// SEのボリュームを変更する
        /// </summary>
        /// <param name="volume"></param>
        private static void ChangeSeVolume(float volume)
        {
            Instance.seSource.volume = volume;
        }
        
        private void Bind()
        {
            _config.bgmVolume.Subscribe(ChangeBgmVolume).AddTo(this);
            _config.seVolume.Subscribe(ChangeSeVolume).AddTo(this);
            
            #if UNITY_EDITOR
            bgmVolume.Value = _config.bgmVolume.Value;
            seVolume.Value = _config.seVolume.Value;
            
            // インスペクタからコンフィグの値を操作する
            bgmVolume.Subscribe(volume => _config.bgmVolume.Value = volume).AddTo(this);
            seVolume.Subscribe(volume => _config.seVolume.Value = volume).AddTo(this);
            #endif
        }
    }
}
