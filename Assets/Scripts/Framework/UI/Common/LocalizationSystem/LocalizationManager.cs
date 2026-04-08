using System;
using System.Collections.Generic;
using Framework.Core.Singleton;
using Newtonsoft.Json;
using UnityEngine;

namespace Framework.UI
{
    /// <summary>
    /// 다국어 텍스트를 런타임에 교체하는 싱글톤 매니저입니다.
    /// <para>
    /// 언어 데이터는 <c>Resources/Localization/</c> 경로에 언어 코드 이름의 JSON 파일로 저장해야 합니다.<br/>
    /// 예: <c>Resources/Localization/ko.json</c>, <c>Resources/Localization/en.json</c>
    /// </para>
    /// </summary>
    public class LocalizationManager : PersistentMonoSingleton<LocalizationManager>
    {
        #region Constants

        private const string ResourcePath = "Localization";
        private const string FallbackLanguage = "en";

        #endregion

        #region Events

        /// <summary>언어가 변경될 때 발생합니다. LocalizedText가 이 이벤트를 구독하여 자동 갱신합니다.</summary>
        public event Action OnLanguageChanged;

        #endregion

        #region Fields

        private readonly Dictionary<string, string> _table = new();
        private string _currentLanguage;

        #endregion

        #region Properties

        /// <summary>현재 적용된 언어 코드입니다. (예: "ko", "en")</summary>
        public string CurrentLanguage => _currentLanguage;

        #endregion

        #region Public Methods

        /// <summary>
        /// 언어를 변경하고 <see cref="OnLanguageChanged"/> 이벤트를 발생시킵니다.
        /// <c>Resources/Localization/{languageCode}.json</c> 파일을 로드합니다.
        /// </summary>
        /// <param name="languageCode">적용할 언어 코드 (예: "ko", "en")</param>
        /// <exception cref="InvalidOperationException">언어 파일을 찾을 수 없는 경우</exception>
        public void SetLanguage(string languageCode)
        {
            if (_currentLanguage == languageCode)
            {
                return;
            }

            Load(languageCode);
            _currentLanguage = languageCode;
            OnLanguageChanged?.Invoke();

#if UNITY_EDITOR
            Debug.Log($"[LocalizationManager] 언어 변경: {languageCode} ({_table.Count}개 키)");
#endif
        }

        /// <summary>
        /// 키에 해당하는 현재 언어의 텍스트를 반환합니다.
        /// 키가 없으면 키 자체를 반환하고 경고 로그를 출력합니다.
        /// </summary>
        /// <param name="key">텍스트 키</param>
        /// <returns>현재 언어의 텍스트. 키가 없으면 키 자체 반환</returns>
        public string Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }

            if (_table.TryGetValue(key, out var text))
            {
                return text;
            }

            Debug.LogWarning($"[LocalizationManager] 키를 찾을 수 없습니다: '{key}' (language: {_currentLanguage})");
            return key;
        }

        /// <summary>
        /// 키에 해당하는 텍스트가 존재하는지 확인합니다.
        /// </summary>
        /// <param name="key">확인할 텍스트 키</param>
        /// <returns>키가 존재하면 true</returns>
        public bool HasKey(string key)
        {
            return _table.ContainsKey(key);
        }

        #endregion

        #region Private Methods

        private void Load(string languageCode)
        {
            var asset = Resources.Load<TextAsset>($"{ResourcePath}/{languageCode}");

            if (asset == null)
            {
                if (languageCode != FallbackLanguage)
                {
                    Debug.LogWarning($"[LocalizationManager] '{languageCode}.json'을 찾을 수 없습니다. '{FallbackLanguage}'로 대체합니다.");
                    Load(FallbackLanguage);
                    return;
                }

                throw new InvalidOperationException(
                    $"[LocalizationManager] 언어 파일을 찾을 수 없습니다: Resources/{ResourcePath}/{languageCode}.json");
            }

            var parsed = JsonConvert.DeserializeObject<Dictionary<string, string>>(asset.text)
                ?? throw new InvalidOperationException(
                    $"[LocalizationManager] JSON 파싱 실패: Resources/{ResourcePath}/{languageCode}.json");

            _table.Clear();

            foreach (var pair in parsed)
            {
                _table[pair.Key] = pair.Value;
            }
        }

        #endregion
    }
}
