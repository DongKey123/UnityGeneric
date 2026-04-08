using UnityEngine;
using UnityEngine.InputSystem;

namespace Framework.Core.InputManager
{
    /// <summary>
    /// PC/콘솔 환경의 입력을 처리하는 InputManager입니다.
    /// 키보드, 마우스, 게임패드 입력을 지원합니다.
    /// </summary>
    public class DesktopInputManager : BaseInputManager
    {
        #region Unity Lifecycle

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                HandleBack();
            }
        }

        #endregion

        #region Properties

        /// <summary>현재 프레임의 마우스 위치 (스크린 좌표)입니다.</summary>
        public Vector2 MousePosition => Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;

        /// <summary>현재 프레임의 마우스 이동 델타값입니다.</summary>
        public Vector2 MouseDelta => Mouse.current != null ? Mouse.current.delta.ReadValue() : Vector2.zero;

        /// <summary>마우스 스크롤 델타값입니다.</summary>
        public Vector2 ScrollDelta => Mouse.current != null ? Mouse.current.scroll.ReadValue() : Vector2.zero;

        #endregion

        #region Public Methods

        /// <summary>
        /// 지정한 마우스 버튼이 이번 프레임에 눌렸는지 반환합니다.
        /// </summary>
        /// <param name="button">확인할 마우스 버튼 (기본값: Left)</param>
        /// <returns>이번 프레임에 눌렸으면 true</returns>
        public bool GetMouseButtonDown(MouseButton button = MouseButton.Left)
        {
            if (Mouse.current == null)
            {
                return false;
            }

            return button switch
            {
                MouseButton.Left   => Mouse.current.leftButton.wasPressedThisFrame,
                MouseButton.Right  => Mouse.current.rightButton.wasPressedThisFrame,
                MouseButton.Middle => Mouse.current.middleButton.wasPressedThisFrame,
                _                  => false
            };
        }

        /// <summary>
        /// 지정한 마우스 버튼이 현재 눌려 있는지 반환합니다.
        /// </summary>
        /// <param name="button">확인할 마우스 버튼 (기본값: Left)</param>
        /// <returns>현재 눌려 있으면 true</returns>
        public bool GetMouseButton(MouseButton button = MouseButton.Left)
        {
            if (Mouse.current == null)
            {
                return false;
            }

            return button switch
            {
                MouseButton.Left   => Mouse.current.leftButton.isPressed,
                MouseButton.Right  => Mouse.current.rightButton.isPressed,
                MouseButton.Middle => Mouse.current.middleButton.isPressed,
                _                  => false
            };
        }

        /// <summary>
        /// 지정한 마우스 버튼이 이번 프레임에 떼어졌는지 반환합니다.
        /// </summary>
        /// <param name="button">확인할 마우스 버튼 (기본값: Left)</param>
        /// <returns>이번 프레임에 떼어졌으면 true</returns>
        public bool GetMouseButtonUp(MouseButton button = MouseButton.Left)
        {
            if (Mouse.current == null)
            {
                return false;
            }

            return button switch
            {
                MouseButton.Left   => Mouse.current.leftButton.wasReleasedThisFrame,
                MouseButton.Right  => Mouse.current.rightButton.wasReleasedThisFrame,
                MouseButton.Middle => Mouse.current.middleButton.wasReleasedThisFrame,
                _                  => false
            };
        }

        /// <summary>
        /// 게임패드가 현재 연결되어 있는지 반환합니다.
        /// </summary>
        /// <returns>연결된 게임패드가 있으면 true</returns>
        public bool IsGamepadConnected()
        {
            return Gamepad.current != null;
        }

        /// <summary>
        /// 게임패드의 왼쪽 스틱 값을 반환합니다.
        /// </summary>
        /// <returns>왼쪽 스틱 Vector2 값. 게임패드 미연결 시 Vector2.zero</returns>
        public Vector2 GetGamepadLeftStick()
        {
            return Gamepad.current != null ? Gamepad.current.leftStick.ReadValue() : Vector2.zero;
        }

        /// <summary>
        /// 게임패드의 오른쪽 스틱 값을 반환합니다.
        /// </summary>
        /// <returns>오른쪽 스틱 Vector2 값. 게임패드 미연결 시 Vector2.zero</returns>
        public Vector2 GetGamepadRightStick()
        {
            return Gamepad.current != null ? Gamepad.current.rightStick.ReadValue() : Vector2.zero;
        }

        #endregion
    }
}
