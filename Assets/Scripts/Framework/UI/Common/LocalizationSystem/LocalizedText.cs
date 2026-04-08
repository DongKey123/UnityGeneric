using TMPro;
using UnityEngine;

namespace Framework.UI
{
    /// <summary>
    /// TMP_Text에 붙이면 언어 변경 시 자동으로 텍스트를 갱신하는 컴포넌트입니다.
    /// Inspector에서 키를 지정하면 <see cref="LocalizationManager.OnLanguageChanged"/> 이벤트를 구독하여 자동 갱신합니다.
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizedText : MonoBehaviour
    {
        #region Fields

        [SerializeField] private string _key;

        private TMP_Text _text;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        private void OnEnable()
        {
            LocalizationManager.Instance.OnLanguageChanged += Refresh;
            Refresh();
        }

        private void OnDisable()
        {
            if (LocalizationManager.Instance != null)
            {
                LocalizationManager.Instance.OnLanguageChanged -= Refresh;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 키를 런타임에 변경하고 텍스트를 즉시 갱신합니다.
        /// </summary>
        /// <param name="key">새로 적용할 텍스트 키</param>
        public void SetKey(string key)
        {
            _key = key;
            Refresh();
        }

        #endregion

        #region Private Methods

        private void Refresh()
        {
            if (string.IsNullOrEmpty(_key))
            {
                return;
            }

            _text.text = LocalizationManager.Instance.Get(_key);
        }

        #endregion
    }
}
