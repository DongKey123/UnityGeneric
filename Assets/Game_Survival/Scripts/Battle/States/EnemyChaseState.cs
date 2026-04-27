using Framework.Patterns.StateMachine;
using UnityEngine;

namespace SurvivalGame.Battle
{
    /// <summary>
    /// 적 추격 상태입니다.
    /// 플레이어를 향해 NavMesh로 이동하며, 공격 사거리 진입 시 공격 상태로 전환합니다.
    /// </summary>
    public class EnemyChaseState : BaseState<Enemy>
    {
        public override void Update(Enemy owner)
        {
            if (owner.Player == null || owner.Player.IsDead)
            {
                owner.ChangeToIdle();
                return;
            }

            float dist = Vector3.Distance(owner.transform.position, owner.Player.transform.position);

            if (dist <= owner.Data.attack_radius)
            {
                owner.Agent.ResetPath();
                owner.ChangeToAttack();
                return;
            }

            owner.Agent.SetDestination(owner.Player.transform.position);
        }
    }
}
