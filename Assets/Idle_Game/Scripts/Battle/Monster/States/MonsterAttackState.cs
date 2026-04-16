using Framework.Patterns.StateMachine;
using UnityEngine;

namespace IdleGame.Battle
{
    /// <summary>
    /// 몬스터 공격 상태입니다.
    /// 공격 쿨타임마다 플레이어를 공격합니다.
    /// 플레이어가 사거리 밖으로 이탈하면 추격 상태로 전환합니다.
    /// </summary>
    public class MonsterAttackState : BaseState<MonsterController>
    {
        private float _attackTimer;

        public override void Enter(MonsterController owner)
        {
            _attackTimer = 0f;
        }

        public override void Update(MonsterController owner)
        {
            if (owner.Player == null || owner.Player.IsDead)
            {
                owner.Agent.ResetPath();
                return;
            }

            float dist = Vector3.Distance(owner.transform.position, owner.Player.transform.position);

            if (dist > owner.Data.atk_range)
            {
                owner.ChangeToChase();
                return;
            }

            _attackTimer += Time.deltaTime;
            float attackInterval = 1f / owner.Data.atk_speed;

            if (_attackTimer < attackInterval) return;

            _attackTimer = 0f;
            owner.Player.TakeDamage(owner.Data.atk);

            // TODO: 공격 애니메이션 / SFX
        }
    }
}
