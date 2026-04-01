using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Framework.Core.InputManager
{
    /// <summary>
    /// 모바일 환경의 입력을 처리하는 InputManager입니다.
    /// 탭, 스와이프, 핀치 제스처를 지원합니다.
    /// </summary>
    public class MobileInputManager : BaseInputManager
    {
        #region Fields

        [SerializeField] private float _swipeThreshold = 100f;
        [SerializeField] private float _tapTimeThreshold = 0.2f;

        private Vector2 _touchStartPosition;
        private float _touchStartTime;

        #endregion

        #region Properties

        /// <summary>현재 활성화된 터치 개수입니다.</summary>
        public int TouchCount => Touch.activeTouches.Count;

        /// <summary>현재 핀치 중인지 여부입니다. (터치 2개)</summary>
        public bool IsPinching => Touch.activeTouches.Count == 2;

        #endregion

        #region Unity Lifecycle

        protected override void OnInitialize()
        {
            EnhancedTouchSupport.Enable();
        }

        private void OnDestroy()
        {
            EnhancedTouchSupport.Disable();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 이번 프레임에 탭(짧은 터치)이 발생했는지 반환합니다.
        /// </summary>
        /// <returns>탭이 발생했으면 true</returns>
        public bool GetTap()
        {
            foreach (var touch in Touch.activeTouches)
            {
                if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended
                    && touch.time - touch.startTime <= _tapTimeThreshold
                    && Vector2.Distance(touch.screenPosition, touch.startScreenPosition) < _swipeThreshold)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 이번 프레임에 스와이프가 완료됐는지 확인하고 방향을 반환합니다.
        /// </summary>
        /// <param name="direction">스와이프 방향 벡터 (정규화됨). 스와이프가 없으면 Vector2.zero</param>
        /// <returns>스와이프가 발생했으면 true</returns>
        public bool GetSwipe(out Vector2 direction)
        {
            direction = Vector2.zero;

            foreach (var touch in Touch.activeTouches)
            {
                if (touch.phase != UnityEngine.InputSystem.TouchPhase.Ended)
                {
                    continue;
                }

                Vector2 delta = touch.screenPosition - touch.startScreenPosition;

                if (delta.magnitude >= _swipeThreshold)
                {
                    direction = delta.normalized;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 현재 핀치 제스처의 델타값을 반환합니다.
        /// 양수면 핀치 아웃(확대), 음수면 핀치 인(축소)입니다.
        /// </summary>
        /// <returns>이전 프레임 대비 두 손가락 거리 변화량. 핀치 중이 아니면 0</returns>
        public float GetPinchDelta()
        {
            if (!IsPinching)
            {
                return 0f;
            }

            var touch0 = Touch.activeTouches[0];
            var touch1 = Touch.activeTouches[1];

            float currentDistance = Vector2.Distance(touch0.screenPosition, touch1.screenPosition);
            float previousDistance = Vector2.Distance(
                touch0.screenPosition - touch0.delta,
                touch1.screenPosition - touch1.delta
            );

            return currentDistance - previousDistance;
        }

        /// <summary>
        /// 첫 번째 터치의 현재 스크린 위치를 반환합니다.
        /// </summary>
        /// <returns>첫 번째 터치 위치. 터치가 없으면 Vector2.zero</returns>
        public Vector2 GetTouchPosition()
        {
            if (Touch.activeTouches.Count == 0)
            {
                return Vector2.zero;
            }

            return Touch.activeTouches[0].screenPosition;
        }

        /// <summary>
        /// 현재 터치 중인지 반환합니다.
        /// </summary>
        /// <returns>터치가 하나 이상 활성화되어 있으면 true</returns>
        public bool IsTouching()
        {
            return Touch.activeTouches.Count > 0;
        }

        #endregion
    }
}
