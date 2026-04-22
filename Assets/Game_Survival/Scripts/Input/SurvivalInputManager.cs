using Framework.Core.InputManager;
using UnityEngine;

namespace SurvivalGame.Input
{
    /// <summary>
    /// 서바이벌 게임 전용 InputManager입니다.
    /// MobileInputManager를 상속받아 가상 조이스틱 방향을 추가로 관리합니다.
    /// VirtualJoystick이 방향을 설정하고, PlayerController가 읽어 이동에 사용합니다.
    /// </summary>
    public class SurvivalInputManager : MobileInputManager
    {
        #region Properties

        /// <summary>SurvivalInputManager 전용 Instance입니다.</summary>
        public static new SurvivalInputManager Instance => BaseInputManager.Instance as SurvivalInputManager;

        /// <summary>
        /// 가상 조이스틱의 현재 입력 방향입니다. (-1 ~ 1, 조작 없을 때 Vector2.zero)
        /// </summary>
        public Vector2 JoystickDirection { get; private set; }

        /// <summary>조이스틱 입력이 있는지 여부입니다.</summary>
        public bool HasJoystickInput => JoystickDirection.sqrMagnitude > 0.01f;

        #endregion

        #region Public Methods

        /// <summary>
        /// VirtualJoystick에서 호출합니다. 조이스틱 방향을 설정합니다.
        /// </summary>
        /// <param name="direction">정규화된 방향 벡터</param>
        public void SetJoystickDirection(Vector2 direction)
        {
            JoystickDirection = direction;
        }

        #endregion
    }
}
