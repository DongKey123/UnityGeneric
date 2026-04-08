using System.Collections;
using TMPro;
using UnityEngine;

namespace Framework.UI
{
    /// <summary>
    /// ToastManager가 사용하는 토스트 UI 컴포넌트입니다.
    /// CanvasGroup 페이드 인/아웃으로 표시되고 사라집니다.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class ToastPanel : MonoBehaviour
    {
        #region Fields

        [SerializeField] private TMP_Text _messageText;
        [SerializeField] private GameObject _defaultIcon;
        [SerializeField] private GameObject _successIcon;
        [SerializeField] private GameObject _warningIcon;
        [SerializeField] private GameObject _errorIcon;

        private CanvasGroup _canvasGroup;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0f;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 토스트를 페이드 인 → 유지 → 페이드 아웃 순서로 재생합니다.
        /// </summary>
        /// <param name="request">표시할 토스트 요청 데이터</param>
        /// <param name="fadeDuration">페이드 인/아웃 시간 (초)</param>
        public IEnumerator PlayShow(ToastRequest request, float fadeDuration)
        {
            _messageText.text = request.Message;
            SetIcon(request.Type);

            yield return Fade(0f, 1f, fadeDuration);
            yield return new WaitForSeconds(request.Duration);
            yield return Fade(1f, 0f, fadeDuration);
        }

        #endregion

        #region Private Methods

        private IEnumerator Fade(float from, float to, float duration)
        {
            var elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
                yield return null;
            }

            _canvasGroup.alpha = to;
        }

        private void SetIcon(ToastType type)
        {
            if (_defaultIcon != null) _defaultIcon.SetActive(type == ToastType.Default);
            if (_successIcon != null) _successIcon.SetActive(type == ToastType.Success);
            if (_warningIcon != null) _warningIcon.SetActive(type == ToastType.Warning);
            if (_errorIcon != null)   _errorIcon.SetActive(type == ToastType.Error);
        }

        #endregion
    }
}
