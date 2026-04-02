using System.Collections;
using System.Collections.Generic;
using Framework.Core.Singleton;
using UnityEngine;

namespace Framework.Core.AudioManager
{
    /// <summary>
    /// BGM과 SFX를 통합 관리하는 싱글톤 오디오 매니저입니다.
    /// <br/><br/>
    /// - BGM: 단일 AudioSource로 재생, 페이드 인/아웃 지원
    /// - SFX: AudioSource 풀을 사용해 동시 재생 지원
    /// - 볼륨/뮤트 설정은 PlayerPrefs에 자동 저장/복원됩니다.
    /// </summary>
    public class AudioManager : PersistentMonoSingleton<AudioManager>
    {
        #region Fields

        [SerializeField] private int _sfxPoolSize = 8;
        [SerializeField] private float _fadeDuration = 1f;

        private AudioSource _bgmSource;
        private List<AudioSource> _sfxSources;
        private Coroutine _fadeCoroutine;

        private float _bgmVolume = 1f;
        private float _sfxVolume = 1f;
        private bool _bgmMuted;
        private bool _sfxMuted;

        private const string KeyBGMVolume = "AudioManager_BGMVolume";
        private const string KeySFXVolume = "AudioManager_SFXVolume";
        private const string KeyBGMMuted  = "AudioManager_BGMMuted";
        private const string KeySFXMuted  = "AudioManager_SFXMuted";

        #endregion

        #region Properties

        /// <summary>BGM 볼륨입니다. 0~1 범위입니다.</summary>
        public float BGMVolume => _bgmVolume;

        /// <summary>SFX 볼륨입니다. 0~1 범위입니다.</summary>
        public float SFXVolume => _sfxVolume;

        /// <summary>BGM 뮤트 상태입니다.</summary>
        public bool IsBGMMuted => _bgmMuted;

        /// <summary>SFX 뮤트 상태입니다.</summary>
        public bool IsSFXMuted => _sfxMuted;

        /// <summary>현재 BGM이 재생 중인지 여부입니다.</summary>
        public bool IsBGMPlaying => _bgmSource != null && _bgmSource.isPlaying;

        #endregion

        #region Unity Lifecycle

        /// <summary>
        /// AudioSource 초기화 및 볼륨 설정 복원을 수행합니다.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            InitializeAudioSources();
            LoadVolumeSettings();
        }

        #endregion

        #region Public Methods — BGM

        /// <summary>
        /// BGM 클립을 재생합니다. 동일 클립이 이미 재생 중이면 무시합니다.
        /// </summary>
        /// <param name="clip">재생할 오디오 클립</param>
        /// <param name="fade">페이드 인 여부</param>
        public void PlayBGM(AudioClip clip, bool fade = false)
        {
            if (clip == null)
            {
                Debug.LogWarning("[AudioManager] PlayBGM: clip is null.");
                return;
            }

            if (_bgmSource.clip == clip && _bgmSource.isPlaying)
            {
                return;
            }

            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
                _fadeCoroutine = null;
            }

            if (fade)
            {
                _fadeCoroutine = StartCoroutine(CrossFadeBGM(clip));
            }
            else
            {
                _bgmSource.clip = clip;
                _bgmSource.volume = _bgmMuted ? 0f : _bgmVolume;
                _bgmSource.Play();
            }
        }

        /// <summary>
        /// BGM 재생을 중지합니다.
        /// </summary>
        /// <param name="fade">페이드 아웃 여부</param>
        public void StopBGM(bool fade = false)
        {
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
                _fadeCoroutine = null;
            }

            if (fade)
            {
                _fadeCoroutine = StartCoroutine(FadeOutBGM(_fadeDuration, () =>
                {
                    _bgmSource.Stop();
                    _bgmSource.clip = null;
                }));
            }
            else
            {
                _bgmSource.Stop();
                _bgmSource.clip = null;
            }
        }

        /// <summary>
        /// BGM을 일시 정지합니다.
        /// </summary>
        public void PauseBGM()
        {
            _bgmSource.Pause();
        }

        /// <summary>
        /// 일시 정지된 BGM을 재개합니다.
        /// </summary>
        public void ResumeBGM()
        {
            _bgmSource.UnPause();
        }

        #endregion

        #region Public Methods — SFX

        /// <summary>
        /// SFX 클립을 재생합니다.
        /// </summary>
        /// <param name="clip">재생할 오디오 클립</param>
        /// <param name="volumeScale">SFX 기본 볼륨에 곱할 스케일 (0~1)</param>
        public void PlaySFX(AudioClip clip, float volumeScale = 1f)
        {
            if (clip == null)
            {
                Debug.LogWarning("[AudioManager] PlaySFX: clip is null.");
                return;
            }

            if (_sfxMuted)
            {
                return;
            }

            AudioSource source = GetAvailableSFXSource();
            source.volume = _sfxVolume * Mathf.Clamp01(volumeScale);
            source.PlayOneShot(clip);
        }

        /// <summary>
        /// 지정한 월드 좌표에서 SFX 클립을 재생합니다.
        /// </summary>
        /// <param name="clip">재생할 오디오 클립</param>
        /// <param name="position">재생할 월드 좌표</param>
        /// <param name="volumeScale">SFX 기본 볼륨에 곱할 스케일 (0~1)</param>
        public void PlaySFXAtPosition(AudioClip clip, Vector3 position, float volumeScale = 1f)
        {
            if (clip == null)
            {
                Debug.LogWarning("[AudioManager] PlaySFXAtPosition: clip is null.");
                return;
            }

            if (_sfxMuted)
            {
                return;
            }

            AudioSource.PlayClipAtPoint(clip, position, _sfxVolume * Mathf.Clamp01(volumeScale));
        }

        /// <summary>
        /// 현재 재생 중인 모든 SFX를 즉시 중지합니다.
        /// </summary>
        public void StopAllSFX()
        {
            foreach (AudioSource source in _sfxSources)
            {
                source.Stop();
            }
        }

        #endregion

        #region Public Methods — Volume / Mute

        /// <summary>
        /// BGM 볼륨을 설정하고 PlayerPrefs에 저장합니다.
        /// </summary>
        /// <param name="volume">설정할 볼륨 값 (0~1)</param>
        public void SetBGMVolume(float volume)
        {
            _bgmVolume = Mathf.Clamp01(volume);

            if (!_bgmMuted)
            {
                _bgmSource.volume = _bgmVolume;
            }

            PlayerPrefs.SetFloat(KeyBGMVolume, _bgmVolume);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// SFX 볼륨을 설정하고 PlayerPrefs에 저장합니다.
        /// </summary>
        /// <param name="volume">설정할 볼륨 값 (0~1)</param>
        public void SetSFXVolume(float volume)
        {
            _sfxVolume = Mathf.Clamp01(volume);

            PlayerPrefs.SetFloat(KeySFXVolume, _sfxVolume);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// BGM 뮤트 상태를 설정하고 PlayerPrefs에 저장합니다.
        /// </summary>
        /// <param name="mute">뮤트 여부</param>
        public void MuteBGM(bool mute)
        {
            _bgmMuted = mute;
            _bgmSource.volume = _bgmMuted ? 0f : _bgmVolume;

            PlayerPrefs.SetInt(KeyBGMMuted, _bgmMuted ? 1 : 0);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// SFX 뮤트 상태를 설정하고 PlayerPrefs에 저장합니다.
        /// </summary>
        /// <param name="mute">뮤트 여부</param>
        public void MuteSFX(bool mute)
        {
            _sfxMuted = mute;

            PlayerPrefs.SetInt(KeySFXMuted, _sfxMuted ? 1 : 0);
            PlayerPrefs.Save();
        }

        #endregion

        #region Private Methods

        private void InitializeAudioSources()
        {
            // BGM 전용 AudioSource
            _bgmSource = gameObject.AddComponent<AudioSource>();
            _bgmSource.loop = true;
            _bgmSource.playOnAwake = false;

            // SFX 풀 생성
            _sfxSources = new List<AudioSource>(_sfxPoolSize);
            for (int i = 0; i < _sfxPoolSize; i++)
            {
                AudioSource sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
                _sfxSources.Add(sfxSource);
            }
        }

        private void LoadVolumeSettings()
        {
            _bgmVolume = PlayerPrefs.GetFloat(KeyBGMVolume, 1f);
            _sfxVolume = PlayerPrefs.GetFloat(KeySFXVolume, 1f);
            _bgmMuted  = PlayerPrefs.GetInt(KeyBGMMuted, 0) == 1;
            _sfxMuted  = PlayerPrefs.GetInt(KeySFXMuted, 0) == 1;

            _bgmSource.volume = _bgmMuted ? 0f : _bgmVolume;
        }

        /// <summary>
        /// 현재 재생 중이지 않은 SFX AudioSource를 반환합니다.
        /// 모두 사용 중이면 새로운 AudioSource를 추가합니다.
        /// </summary>
        private AudioSource GetAvailableSFXSource()
        {
            foreach (AudioSource source in _sfxSources)
            {
                if (!source.isPlaying)
                {
                    return source;
                }
            }

            // 풀이 부족하면 동적으로 확장
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            newSource.loop = false;
            newSource.playOnAwake = false;
            _sfxSources.Add(newSource);

#if UNITY_EDITOR
            Debug.Log("[AudioManager] SFX 풀 확장: " + _sfxSources.Count + "개");
#endif

            return newSource;
        }

        private IEnumerator CrossFadeBGM(AudioClip nextClip)
        {
            // 현재 BGM이 재생 중이면 페이드 아웃
            if (_bgmSource.isPlaying)
            {
                float elapsed = 0f;
                float startVolume = _bgmSource.volume;

                while (elapsed < _fadeDuration)
                {
                    elapsed += Time.deltaTime;
                    _bgmSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / _fadeDuration);
                    yield return null;
                }

                _bgmSource.Stop();
            }

            // 새 클립으로 페이드 인
            _bgmSource.clip = nextClip;
            _bgmSource.volume = 0f;
            _bgmSource.Play();

            float targetVolume = _bgmMuted ? 0f : _bgmVolume;
            float fadeElapsed = 0f;

            while (fadeElapsed < _fadeDuration)
            {
                fadeElapsed += Time.deltaTime;
                _bgmSource.volume = Mathf.Lerp(0f, targetVolume, fadeElapsed / _fadeDuration);
                yield return null;
            }

            _bgmSource.volume = targetVolume;
            _fadeCoroutine = null;
        }

        private IEnumerator FadeOutBGM(float duration, System.Action onComplete)
        {
            float elapsed = 0f;
            float startVolume = _bgmSource.volume;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                _bgmSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
                yield return null;
            }

            _bgmSource.volume = 0f;
            onComplete?.Invoke();
            _fadeCoroutine = null;
        }

        #endregion
    }
}
