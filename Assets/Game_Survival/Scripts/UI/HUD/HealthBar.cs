using UnityEngine;
using UnityEngine.UI;

namespace SurvivalGame.UI.HUD
{
    /// <summary>
    /// World Space Canvas에 붙는 체력/기력 바입니다.
    /// 카메라를 항상 바라보며(빌보드), hideWhenFull이 true이면 max 상태일 때 자신을 비활성화합니다.
    /// </summary>
    public class HealthBar : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private Image _fill;
        [SerializeField] private bool  _hideWhenFull = false;

        #endregion

        #region Private Fields

        private UnityEngine.Camera _cam;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            _cam = UnityEngine.Camera.main;
        }

        private void LateUpdate()
        {
            transform.forward = _cam.transform.forward;
        }

        #endregion

        #region Public Methods

        /// <summary>현재/최대 값으로 바를 갱신합니다.</summary>
        public void SetValue(int current, int max)
        {
            if (max <= 0) return;

            if (_hideWhenFull)
                gameObject.SetActive(current < max);

            _fill.fillAmount = (float)current / max;
        }

        /// <summary>0~1 비율로 바를 갱신합니다.</summary>
        public void SetValue(float ratio)
        {
            if (_hideWhenFull)
                gameObject.SetActive(ratio < 1f);

            _fill.fillAmount = Mathf.Clamp01(ratio);
        }

        #endregion
    }
}
