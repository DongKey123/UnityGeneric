using Framework.Patterns.StateMachine;
using Random = UnityEngine.Random;
using SurvivalGame.Data;
using SurvivalGame.Inventories;
using SurvivalGame.Player;
using UnityEngine;
using UnityEngine.AI;

namespace SurvivalGame.Battle
{
    /// <summary>
    /// 적 오브젝트를 제어하는 컨트롤러입니다.
    /// StateMachine으로 Idle / Chase / Attack / Dead 상태를 관리합니다.
    /// </summary>
    /// <remarks>
    /// [프리팹 구조]
    /// Enemy (루트 — Enemy 스크립트, NavMeshAgent, Capsule Collider)
    /// └── Visual (자식 GameObject — 메시 등 비주얼)
    ///
    /// [군집 반응]
    /// 피격 시 alert_radius 내 다른 Enemy에게 AlertChase()를 호출합니다.
    /// </remarks>
    [RequireComponent(typeof(NavMeshAgent))]
    public class Enemy : MonoBehaviour, IDamageable
    {
        #region Properties

        /// <summary>적 데이터</summary>
        public EnemyData Data { get; private set; }

        /// <summary>현재 체력</summary>
        public int CurrentHp { get; private set; }

        /// <summary>사망 여부</summary>
        public bool IsDead { get; private set; }

        /// <summary>플레이어 참조</summary>
        public PlayerController Player { get; private set; }

        /// <summary>NavMeshAgent 컴포넌트</summary>
        public NavMeshAgent Agent { get; private set; }

        #endregion

        #region Private Fields

        private static readonly int SpeedHash = Animator.StringToHash("Speed");

        private StateMachine<Enemy> _stateMachine;
        private EnemyIdleState   _idleState;
        private EnemyChaseState  _chaseState;
        private EnemyAttackState _attackState;
        private EnemyDeadState   _deadState;
        private Inventory        _playerInventory;
        private Animator         _animator;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            Agent     = GetComponent<NavMeshAgent>();
            _animator = GetComponentInChildren<Animator>();

            _idleState   = new EnemyIdleState();
            _chaseState  = new EnemyChaseState();
            _attackState = new EnemyAttackState();
            _deadState   = new EnemyDeadState();

            _stateMachine = new StateMachine<Enemy>(this);
        }

        private void Update()
        {
            _stateMachine.Update();
            if (_animator != null)
                _animator.SetFloat(SpeedHash, Agent.velocity.magnitude);
        }

        #endregion

        #region Public Methods

        /// <summary>EnemySpawner에서 초기화 시 호출합니다.</summary>
        public void Initialize(EnemyData data, PlayerController player, Inventory playerInventory)
        {
            Data             = data;
            Player           = player;
            _playerInventory = playerInventory;
            CurrentHp        = data.hp_max;
            IsDead           = false;

            Agent.speed = data.move_speed;
            _stateMachine.SetInitialState(_idleState);
        }

        /// <summary>
        /// 군집 반응 — 범위 내 동료가 피격됐을 때 호출됩니다.
        /// Idle 상태에서만 Chase로 전환합니다.
        /// </summary>
        public void AlertChase()
        {
            if (!IsDead)
                ChangeToChase();
        }

        #endregion

        #region IDamageable

        /// <inheritdoc/>
        public void TakeDamage(int attackPower)
        {
            if (IsDead) return;

            int damage    = Mathf.Max(1, attackPower - Data.defense);
            CurrentHp     = Mathf.Max(0, CurrentHp - damage);

            // 군집 반응 — 피격 시 주변 적에게 알림
            NotifyNearbyEnemies();

            if (CurrentHp == 0)
                Die();
        }

        #endregion

        #region State Transitions

        public void ChangeToIdle()   => _stateMachine.ChangeState(_idleState);
        public void ChangeToChase()  => _stateMachine.ChangeState(_chaseState);
        public void ChangeToAttack() => _stateMachine.ChangeState(_attackState);
        public void ChangeToDead()   => _stateMachine.ChangeState(_deadState);

        #endregion

        #region Private Methods

        private void Die()
        {
            IsDead = true;
            Drop();
            ChangeToDead();
        }

        private void Drop()
        {
            if (Data.drop_item_id <= 0) return;

            // TODO: InGameDataManager에서 SurvivalItemData 조회 후 인벤토리 추가
            // 현재는 드롭 이벤트만 발행하며 EnemySpawner가 처리합니다.
            int dropCount = Random.Range(Data.drop_count_min, Data.drop_count_max + 1);
            _ = dropCount; // EnemyDiedEvent에서 처리
        }

        private void NotifyNearbyEnemies()
        {
            var hits = Physics.OverlapSphere(transform.position, Data.alert_radius);
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<Enemy>(out var other) && other != this)
                    other.AlertChase();
            }
        }

        #endregion
    }
}
