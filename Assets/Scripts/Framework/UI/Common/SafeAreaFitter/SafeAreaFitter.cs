using UnityEngine;

namespace Framework.UI
{
    /// <summary>
    /// 디바이스의 Safe Area에 맞게 RectTransform을 자동으로 조정하는 컴포넌트입니다.
    /// 노치, 펀치홀, 홈 인디케이터 등으로 인해 UI가 가려지는 것을 방지합니다.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaFitter : MonoBehaviour
    {
        #region Fields

        [SerializeField] private bool _applyTop = true;
        [SerializeField] private bool _applyBottom = true;
        [SerializeField] private bool _applyLeft = true;
        [SerializeField] private bool _applyRight = true;

        private RectTransform _rectTransform;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Start()
        {
            Apply();
        }

        private void OnRectTransformDimensionsChange()
        {
            Apply();
        }

        #endregion

        #region Private Methods

        private void Apply()
        {
            _lastSafeArea = Screen.safeArea;
            _lastOrientation = Screen.orientation;

            var safeArea = Screen.safeArea;
            var screenSize = new Vector2(Screen.width, Screen.height);

            var anchorMin = safeArea.position / screenSize;
            var anchorMax = (safeArea.position + safeArea.size) / screenSize;

            if (!_applyLeft)    anchorMin.x = 0f;
            if (!_applyBottom)  anchorMin.y = 0f;
            if (!_applyRight)   anchorMax.x = 1f;
            if (!_applyTop)     anchorMax.y = 1f;

            _rectTransform.anchorMin = anchorMin;
            _rectTransform.anchorMax = anchorMax;
        }

        #endregion
    }
}
