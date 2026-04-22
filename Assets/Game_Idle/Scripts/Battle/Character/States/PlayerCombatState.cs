using Framework.Patterns.StateMachine;
using UnityEngine;

namespace IdleGame.Battle
{
    /// <summary>
    /// 플레이어 전투 상태입니다.
    /// 자동/수동 모드에 따라 이동과 공격 방식이 달라집니다.
    /// </summary>
    public class PlayerCombatState : BaseState<PlayerController>
    {
        private float _attackTimer;

        public override void Enter(PlayerController owner)
        {
            _attackTimer = 0f;
        }

        public override void Update(PlayerController owner)
        {
            RefreshTarget(owner);
            if (owner.Target == null) return;

            if (owner.IsAutoMode)
                UpdateAuto(owner);
            else
                UpdateManual(owner);
        }

        public override void Exit(PlayerController owner)
        {
            owner.Agent.ResetPath();
        }

        #region Private Methods

        /// <summary>타겟이 없거나 사망했을 때 다음 타겟을 찾거나 대기 상태로 전환합니다.</summary>
        private void RefreshTarget(PlayerController owner)
        {
            if (owner.Target != null && !owner.Target.IsDead) return;

            var next = owner.FindNearestTarget();
            if (next == null)
            {
                owner.ChangeToIdle();
                return;
            }
            owner.SetTarget(next);
        }

        /// <summary>자동 모드 — 타겟 자동 추격, 스킬 우선 공격</summary>
        private void UpdateAuto(PlayerController owner)
        {
            float dist = Vector3.Distance(owner.transform.position, owner.TargetTransform.position);
            bool inRange = dist <= owner.Stat.AtkRange;

            if (inRange)
            {
                owner.Agent.ResetPath();
                TryAttack(owner);
            }
            else
            {
                owner.Agent.SetDestination(owner.TargetTransform.position);
            }
        }

        /// <summary>수동 모드 — 이동 입력 없고 정지 상태일 때만 공격</summary>
        private void UpdateManual(PlayerController owner)
        {
            bool isMoving = owner.Agent.velocity.sqrMagnitude > 0.01f;
            if (isMoving) return;

            float dist = Vector3.Distance(owner.transform.position, owner.TargetTransform.position);
            bool inRange = dist <= owner.Stat.AtkRange;

            if (inRange)
            {
                TryAttack(owner);
            }
            else
            {
                // 사거리 밖이면 타겟까지 자동 이동
                owner.Agent.SetDestination(owner.TargetTransform.position);
            }
        }

        /// <summary>스킬 우선, 기본 공격 타이머 기반으로 공격을 시도합니다.</summary>
        private void TryAttack(PlayerController owner)
        {
            // 쿨타임 완료된 스킬이 있으면 스킬 우선 발동
            if (owner.SkillSystem.TryUseAny(owner)) return;

            _attackTimer += Time.deltaTime;
            float attackInterval = 1f / owner.Stat.AtkSpeed;

            if (_attackTimer < attackInterval) return;

            _attackTimer = 0f;
            PerformAttack(owner);
        }

        /// <summary>크리티컬 판정 후 타겟에 공격력을 전달합니다.</summary>
        private void PerformAttack(PlayerController owner)
        {
            bool isCrit = Random.value < owner.Stat.CritRate;
            int attackPower = isCrit
                ? Mathf.RoundToInt(owner.Stat.Atk * owner.Stat.CritDmg)
                : owner.Stat.Atk;

            owner.Target.TakeDamage(attackPower);

            // TODO: 공격 애니메이션 / SFX
        }

        #endregion
    }
}
