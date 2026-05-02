using Framework.Core.EventBus;
using SurvivalGame.Battle;
using SurvivalGame.Input;
using SurvivalGame.Inventories;
using UnityEngine;

namespace SurvivalGame.Player
{
    /// <summary>
    /// 플레이어 이동, 공격, 인벤토리를 관리하는 컨트롤러입니다.
    /// Rigidbody 기반으로 이동하며, SurvivalInputManager에서 조이스틱 방향을 읽습니다.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour, IDamageable
    {
        #region Inspector

        [SerializeField] private float    _moveSpeed    = 5f;
        [SerializeField] private int      _maxSlots     = 20;
        [SerializeField] private float    _maxWeight    = 50f;
        [SerializeField] private int      _maxHp        = 100;
        [SerializeField] private int      _attackPower  = 10;
        [SerializeField] private float    _attackRadius = 2.5f;
        [SerializeField] private Animator _animator;

        #endregion

        #region Private Fields

        private static readonly int SpeedHash = Animator.StringToHash("Speed");

        private Rigidbody _rb;
        private readonly Collider[] _overlapBuffer = new Collider[16];

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

        /// <summary>공격력입니다.</summary>
        public int AttackPower => _attackPower;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            _rb                    = GetComponent<Rigidbody>();
            _rb.freezeRotation     = true;
            Inventory              = new Inventory(_maxSlots, _maxWeight);
            CurrentHp              = _maxHp;

            var rangeTrigger       = gameObject.AddComponent<SphereCollider>();
            rangeTrigger.isTrigger = true;
            rangeTrigger.radius    = _attackRadius;
        }

        private void Update()
        {
            HandleTouchAttack();
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

        #region Public Methods

        /// <summary>
        /// 공격 범위 내 가장 가까운 살아있는 적을 반환합니다.
        /// 없으면 null을 반환합니다.
        /// </summary>
        public Enemy GetNearestEnemy()
        {
            int count   = Physics.OverlapSphereNonAlloc(transform.position, _attackRadius, _overlapBuffer);
            Enemy nearest = null;
            float minDist = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                if (!_overlapBuffer[i].TryGetComponent<Enemy>(out var enemy) || enemy.IsDead) continue;

                float dist = Vector3.Distance(transform.position, _overlapBuffer[i].transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = enemy;
                }
            }

            return nearest;
        }

        /// <summary>
        /// 공격 범위 내 가장 가까운 적을 자동 타겟팅해 공격합니다.
        /// </summary>
        public void Attack()
        {
            var target = GetNearestEnemy();
            if (target == null) return;

            FaceTarget(target.transform.position);
            target.TakeDamage(_attackPower);
        }

        #endregion

        #region Private Methods

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<Enemy>(out var enemy) && !enemy.IsDead)
                EventBus.Publish(new EnemyEnteredAttackRangeEvent { Source = enemy });
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<Enemy>(out var enemy))
                EventBus.Publish(new EnemyExitedAttackRangeEvent { Source = enemy });
        }

        private void Move()
        {
            Vector2 input = SurvivalInputManager.Instance.JoystickDirection;
            IsMoving      = SurvivalInputManager.Instance.HasJoystickInput;

            if (_animator != null) _animator.SetFloat(SpeedHash, IsMoving ? input.magnitude : 0f);

            if (!IsMoving) return;

            Vector3 direction      = new Vector3(input.x, 0f, input.y);
            Vector3 targetPosition = _rb.position + direction * _moveSpeed * Time.fixedDeltaTime;
            _rb.MovePosition(targetPosition);

            transform.forward = direction;
        }

        /// <summary>적을 직접 탭했을 때 공격합니다.</summary>
        private void HandleTouchAttack()
        {
            if (!SurvivalInputManager.Instance.GetTap(out Vector2 screenPos)) return;

            Ray ray = UnityEngine.Camera.main.ScreenPointToRay(screenPos);
            if (!Physics.Raycast(ray, out RaycastHit hit, 50f)) return;
            if (!hit.collider.TryGetComponent<Enemy>(out var enemy) || enemy.IsDead) return;

            FaceTarget(enemy.transform.position);
            enemy.TakeDamage(_attackPower);
        }

        private void FaceTarget(Vector3 targetPosition)
        {
            Vector3 dir = targetPosition - transform.position;
            dir.y = 0f;
            if (dir != Vector3.zero)
                transform.forward = dir.normalized;
        }

        #endregion
    }
}
