using Framework.Core.EventBus;
using Framework.Patterns.StateMachine;
using UnityEngine;

namespace SurvivalGame.Battle
{
    /// <summary>
    /// 적 공격 상태입니다.
    /// 공격 간격마다 플레이어를 공격합니다.
    /// 플레이어가 공격 사거리 밖으로 이탈하면 추격 상태로 전환합니다.
    /// 공격 타이밍에 플레이어가 범위 밖이면 회피 처리 (데미지 없음).
    /// </summary>
    public class EnemyAttackState : BaseState<Enemy>
    {
        private float _attackTimer;

        public override void Enter(Enemy owner)
        {
            _attackTimer = 0f;
        }

        public override void Update(Enemy owner)
        {
            if (owner.Player == null || owner.Player.IsDead)
            {
                owner.ChangeToIdle();
                return;
            }

            float dist = Vector3.Distance(owner.transform.position, owner.Player.transform.position);

            if (dist > owner.Data.attack_radius)
            {
                owner.ChangeToChase();
                return;
            }

            _attackTimer += Time.deltaTime;
            if (_attackTimer < owner.Data.attack_interval) return;
            _attackTimer = 0f;

            // 공격 타이밍에 실제로 범위 내에 있을 때만 피해 적용 (회피 판정)
            if (dist <= owner.Data.attack_radius)
            {
                int damage = Mathf.Max(1, owner.Data.attack);
                owner.Player.TakeDamage(damage);

                EventBus.Publish(new EnemyAttackedEvent
                {
                    Source = owner,
                    Damage = damage
                });
            }
        }
    }
}
