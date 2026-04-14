using System;
using Framework.Core.ObjectPool;
using Framework.Patterns.StateMachine;
using IdleGame.Data;
using UnityEngine;
using UnityEngine.AI;

namespace IdleGame.Battle
{
    /// <summary>
    /// 몬스터를 제어하는 컨트롤러입니다.
    /// IDamageable과 IPoolable을 구현하며, StateMachine으로 대기 / 추격 / 공격 / 사망 상태를 관리합니다.
    /// </summary>
    public class MonsterController : MonoBehaviour, IDamageable, IPoolable
    {
        #region Events

        /// <summary>몬스터 사망 시 발생합니다. (this)</summary>
        public event Action<MonsterController> OnDied;

        #endregion

        #region Properties

        /// <summary>몬스터 데이터</summary>
        public MonsterData Data { get; private set; }

        /// <summary>현재 체력</summary>
        public int CurrentHp { get; private set; }

        /// <summary>사망 여부</summary>
        public bool IsDead { get; private set; }

        /// <summary>플레이어 참조 (공격/추격 대상)</summary>
        public PlayerController Player { get; private set; }

        /// <summary>NavMeshAgent 컴포넌트</summary>
        public NavMeshAgent Agent { get; private set; }

        #endregion

        #region Private Fields

        private StateMachine<MonsterController> _stateMachine;
        private MonsterIdleState   _idleState;
        private MonsterChaseState  _chaseState;
        private MonsterAttackState _attackState;
        private MonsterDeadState   _deadState;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();

            _idleState   = new MonsterIdleState();
            _chaseState  = new MonsterChaseState();
            _attackState = new MonsterAttackState();
            _deadState   = new MonsterDeadState();

            _stateMachine = new StateMachine<MonsterController>(this);
        }

        private void Update()
        {
            _stateMachine.Update();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 몬스터를 초기화합니다. 풀에서 꺼낼 때 StageManager가 호출합니다.
        /// </summary>
        /// <param name="data">몬스터 데이터</param>
        /// <param name="player">플레이어 참조</param>
        public void Initialize(MonsterData data, PlayerController player)
        {
            Data      = data;
            Player    = player;
            CurrentHp = data.hp;
            IsDead    = false;

            Agent.speed = data.move_speed;
            _stateMachine.SetInitialState(_idleState);
        }

        /// <summary>풀로 반환합니다. MonsterDeadState에서 호출됩니다.</summary>
        public void ReturnToPool()
        {
            OnDied?.Invoke(this);
            // TODO: StageManager가 OnDied를 구독하여 풀 반환 및 리스폰 처리
        }

        #endregion

        #region IDamageable

        /// <summary>
        /// 피해를 받습니다. DEF를 적용한 최종 피해를 계산합니다.
        /// 최소 피해는 ATK × 0.1입니다.
        /// </summary>
        /// <param name="attackPower">공격자의 공격력 (크리티컬 적용 후)</param>
        public void TakeDamage(int attackPower)
        {
            if (IsDead) return;

            int minDamage   = Mathf.Max(1, Mathf.RoundToInt(attackPower * 0.1f));
            int rawDamage   = attackPower - Mathf.RoundToInt(Data.def * 0.5f);
            int finalDamage = Mathf.Max(minDamage, rawDamage);

            CurrentHp = Mathf.Max(0, CurrentHp - finalDamage);

            // TODO: 데미지 숫자 팝업 (머리 위)
            // TODO: 피격 SFX

            if (CurrentHp == 0)
                Die();
        }

        #endregion

        #region IPoolable

        public void OnSpawn()
        {
            IsDead = false;
        }

        public void OnDespawn()
        {
            Agent.ResetPath();
            OnDied = null;
        }

        #endregion

        #region State Transitions

        public void ChangeToChase()  => _stateMachine.ChangeState(_chaseState);
        public void ChangeToAttack() => _stateMachine.ChangeState(_attackState);
        public void ChangeToDead()   => _stateMachine.ChangeState(_deadState);

        #endregion

        #region Private Methods

        private void Die()
        {
            IsDead = true;
            ChangeToDead();

            // TODO: 드롭 처리 (골드 / 경험치 오브젝트 필드 생성)
        }

        #endregion
    }
}
