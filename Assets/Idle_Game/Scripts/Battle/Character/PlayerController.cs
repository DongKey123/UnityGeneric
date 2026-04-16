using System;
using System.Linq;
using Framework.Core.DataManager;
using Framework.Patterns.StateMachine;
using IdleGame.Data;
using UnityEngine;
using UnityEngine.AI;

namespace IdleGame.Battle
{
    /// <summary>
    /// 플레이어를 제어하는 중심 컨트롤러입니다.
    /// IDamageable을 구현하며, StateMachine으로 대기 / 전투 / 사망 상태를 관리합니다.
    /// </summary>
    public class PlayerController : MonoBehaviour, IDamageable
    {
        #region Inspector

        [SerializeField] private HpBar _hpBar;

        #endregion

        #region Events

        /// <summary>플레이어 부활 시 발생합니다.</summary>
        public event Action OnRevived;

        #endregion

        #region Constants

        private const float DetectRadius   = 15f;
        private const string MonsterLayer  = "Monster";

        #endregion

        private LayerMask _monsterLayer;

        #region Properties

        /// <summary>플레이어 스탯</summary>
        public PlayerStat Stat { get; private set; }

        /// <summary>현재 체력</summary>
        public int CurrentHp { get; private set; }

        /// <summary>사망 여부</summary>
        public bool IsDead { get; private set; }

        /// <summary>자동 모드 여부 (false = 수동 모드)</summary>
        public bool IsAutoMode { get; private set; } = true;

        /// <summary>현재 공격 타겟</summary>
        public IDamageable Target { get; private set; }

        /// <summary>타겟의 Transform (이동/거리 계산용)</summary>
        public Transform TargetTransform { get; private set; }

        /// <summary>NavMeshAgent 컴포넌트</summary>
        public NavMeshAgent Agent { get; private set; }

        /// <summary>스킬 시스템</summary>
        public SkillSystem SkillSystem { get; private set; }

        #endregion

        #region Private Fields

        private StateMachine<PlayerController> _stateMachine;
        private PlayerIdleState _idleState;
        private PlayerCombatState _combatState;
        private PlayerDeadState _deadState;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
            Stat  = new PlayerStat();
            _monsterLayer = LayerMask.GetMask(MonsterLayer);
            SkillSystem   = new SkillSystem();

            _idleState   = new PlayerIdleState();
            _combatState = new PlayerCombatState();
            _deadState   = new PlayerDeadState();

            _stateMachine = new StateMachine<PlayerController>(this);
        }

        private void Update()
        {
            _stateMachine.Update();
            SkillSystem.Tick(Time.deltaTime);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 플레이어를 초기화합니다. MainEntry에서 호출하세요.
        /// </summary>
        public void Initialize()
        {
            Stat.Recalculate();
            CurrentHp = Stat.MaxHp;
            IsDead    = false;

            var defaultSkills = InGameDataManager.Instance
                .GetAll<SkillData>()
                .Where(s => s.unlock_type == "Default");
            SkillSystem.Initialize(defaultSkills);

            _hpBar?.UpdateHp(CurrentHp, Stat.MaxHp);
            _stateMachine.SetInitialState(_idleState);
        }

        /// <summary>자동/수동 모드를 전환합니다.</summary>
        /// <param name="isAuto">true = 자동, false = 수동</param>
        public void SetAutoMode(bool isAuto)
        {
            IsAutoMode = isAuto;
        }

        /// <summary>공격 타겟을 설정합니다.</summary>
        /// <param name="target">타겟 IDamageable</param>
        public void SetTarget(IDamageable target)
        {
            Target          = target;
            TargetTransform = (target as MonoBehaviour)?.transform;
        }

        /// <summary>탐지 범위(15m) 내 가장 가까운 살아있는 몬스터를 반환합니다.</summary>
        /// <returns>가장 가까운 IDamageable, 없으면 null</returns>
        public IDamageable FindNearestTarget()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, DetectRadius, _monsterLayer);

            IDamageable nearest  = null;
            float       minDist  = float.MaxValue;

            foreach (var hit in hits)
            {
                var damageable = hit.GetComponent<IDamageable>();
                if (damageable == null || damageable.IsDead) continue;

                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist >= minDist) continue;

                minDist = dist;
                nearest = damageable;
            }

            return nearest;
        }

        /// <summary>체력을 회복합니다. SkillSystem(Heal)에서 호출됩니다.</summary>
        /// <param name="amount">회복량</param>
        public void Heal(int amount)
        {
            if (IsDead) return;
            CurrentHp = Mathf.Min(Stat.MaxHp, CurrentHp + amount);
            _hpBar?.UpdateHp(CurrentHp, Stat.MaxHp);
        }

        /// <summary>부활 처리합니다. PlayerDeadState에서 호출됩니다.</summary>
        public void Revive()
        {
            IsDead    = false;
            CurrentHp = Stat.MaxHp;
            _hpBar?.UpdateHp(CurrentHp, Stat.MaxHp);
            OnRevived?.Invoke();
            ChangeToIdle();

            // TODO: 레벨업 이펙트와 동일한 복귀 이펙트
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

            int minDamage  = Mathf.Max(1, Mathf.RoundToInt(attackPower * 0.1f));
            int rawDamage  = attackPower - Mathf.RoundToInt(Stat.Def * 0.5f);
            int finalDamage = Mathf.Max(minDamage, rawDamage);

            CurrentHp = Mathf.Max(0, CurrentHp - finalDamage);
            _hpBar?.UpdateHp(CurrentHp, Stat.MaxHp);

            // TODO: 데미지 숫자 팝업 (머리 위)
            // TODO: 피격 SFX

            if (CurrentHp == 0)
                Die();
        }

        #endregion

        #region State Transitions

        public void ChangeToIdle()   => _stateMachine.ChangeState(_idleState);
        public void ChangeToCombat() => _stateMachine.ChangeState(_combatState);
        public void ChangeToDead()   => _stateMachine.ChangeState(_deadState);

        #endregion

        #region Private Methods

        private void Die()
        {
            IsDead = true;
            Target = null;
            TargetTransform = null;
            ChangeToDead();
        }

        #endregion
    }
}
