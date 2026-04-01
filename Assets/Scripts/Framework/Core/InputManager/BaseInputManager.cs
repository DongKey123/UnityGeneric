using Framework.Core.Singleton;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Framework.Core.InputManager
{
    /// <summary>
    /// 플랫폼별 InputManager의 공통 기반 클래스입니다.
    /// PersistentMonoSingleton을 상속하여 씬 전환 후에도 유지됩니다.
    /// <br/><br/>
    /// - PC/콘솔: <see cref="DesktopInputManager"/>
    /// - 모바일: <see cref="MobileInputManager"/>
    /// </summary>
    public abstract class BaseInputManager : PersistentMonoSingleton<BaseInputManager>
    {
        #region Public Methods

        /// <summary>
        /// 지정한 Action이 이번 프레임에 눌렸는지 반환합니다.
        /// </summary>
        /// <param name="action">확인할 InputAction</param>
        /// <returns>이번 프레임에 눌렸으면 true</returns>
        public bool GetButtonDown(InputAction action)
        {
            return action.WasPressedThisFrame();
        }

        /// <summary>
        /// 지정한 Action이 현재 눌려 있는지 반환합니다.
        /// </summary>
        /// <param name="action">확인할 InputAction</param>
        /// <returns>현재 눌려 있으면 true</returns>
        public bool GetButton(InputAction action)
        {
            return action.IsPressed();
        }

        /// <summary>
        /// 지정한 Action이 이번 프레임에 떼어졌는지 반환합니다.
        /// </summary>
        /// <param name="action">확인할 InputAction</param>
        /// <returns>이번 프레임에 떼어졌으면 true</returns>
        public bool GetButtonUp(InputAction action)
        {
            return action.WasReleasedThisFrame();
        }

        /// <summary>
        /// 지정한 Action의 Vector2 값을 반환합니다. (이동, 조이스틱 등)
        /// </summary>
        /// <param name="action">확인할 InputAction</param>
        /// <returns>Vector2 입력값</returns>
        public Vector2 GetAxis(InputAction action)
        {
            return action.ReadValue<Vector2>();
        }

        #endregion
    }
}
