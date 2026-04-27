using SurvivalGame.Battle;
using SurvivalGame.Input;
using SurvivalGame.Inventories;
using UnityEngine;

namespace SurvivalGame.Player
{
    /// <summary>
    /// 플레이어 이동 및 인벤토리를 관리하는 컨트롤러입니다.
    /// Rigidbody 기반으로 이동하며, SurvivalInputManager에서 조이스틱 방향을 읽습니다.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour, IDamageable
    {
        #region Inspector

        [SerializeField] private float _moveSpeed  = 5f;
        [SerializeField] private int   _maxSlots   = 20;
        [SerializeField] private float _maxWeight  = 50f;
        [SerializeField] private int   _maxHp      = 100;

        #endregion

        #region Private Fields

        private Rigidbody _rb;

        #endregion

        #region Properties

        /// <summary>현재 이동 중인지 여부입니다.</summary>
        public bool IsMoving { get; private set; }

        /// <summary>플레이어 인벤토리입니다.</summary>
        public Inventory Inventory { get; private set; }

        /// <summary>현재 체력입니다.</summary>
        public int CurrentHp { get; private set; }

        /// <summary>사망 여부입니다.</summary>
        public bool IsDead { get; private set; }

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            _rb       = GetComponent<Rigidbody>();
            _rb.freezeRotation = true;
            Inventory  = new Inventory(_maxSlots, _maxWeight);
            CurrentHp  = _maxHp;
        }

        private void FixedUpdate()
        {
            Move();
        }

        #endregion

        #region IDamageable

        /// <inheritdoc/>
        public void TakeDamage(int attackPower)
        {
            if (IsDead) return;

            int damage = Mathf.Max(1, attackPower);
            CurrentHp  = Mathf.Max(0, CurrentHp - damage);

            if (CurrentHp == 0)
                IsDead = true;

            // TODO: 플레이어 사망 처리, 피격 UI
        }

        #endregion

        #region Private Methods

        private void Move()
        {
            Vector2 input = SurvivalInputManager.Instance.JoystickDirection;
            IsMoving = SurvivalInputManager.Instance.HasJoystickInput;

            if (!IsMoving) return;

            Vector3 direction = new Vector3(input.x, 0f, input.y);
            Vector3 targetPosition = _rb.position + direction * _moveSpeed * Time.fixedDeltaTime;
            _rb.MovePosition(targetPosition);

            // 이동 방향으로 캐릭터 회전
            transform.forward = direction;
        }

        #endregion
    }
}
