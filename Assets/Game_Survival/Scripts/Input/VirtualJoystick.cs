using UnityEngine;
using UnityEngine.EventSystems;

namespace SurvivalGame.Input
{
    /// <summary>
    /// 좌하단 가상 조이스틱 UI 컴포넌트입니다.
    /// 드래그 입력을 감지해 SurvivalInputManager에 방향을 전달합니다.
    /// </summary>
    public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        #region Inspector

        [SerializeField] private RectTransform _background;
        [SerializeField] private RectTransform _handle;
        [SerializeField] private float _maxRadius = 60f;

        #endregion

        #region IPointer Handlers

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _background, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);

            Vector2 clamped = Vector2.ClampMagnitude(localPoint, _maxRadius);
            _handle.localPosition = clamped;

            SurvivalInputManager.Instance.SetJoystickDirection(clamped / _maxRadius);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _handle.localPosition = Vector2.zero;
            SurvivalInputManager.Instance.SetJoystickDirection(Vector2.zero);
        }

        #endregion
    }
}
