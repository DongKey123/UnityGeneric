using Framework.Patterns.StateMachine;
using UnityEngine;

namespace IdleGame.Battle
{
    /// <summary>
    /// 몬스터 추격 상태입니다.
    /// 플레이어를 향해 이동하며, 공격 사거리 내 진입 시 공격 상태로 전환합니다.
    /// </summary>
    public class MonsterChaseState : BaseState<MonsterController>
    {
        public override void Update(MonsterController owner)
        {
            if (owner.Player == null) return;

            float dist = Vector3.Distance(owner.transform.position, owner.Player.transform.position);

            if (dist <= owner.Data.atk_range)
            {
                owner.Agent.ResetPath();
                owner.ChangeToAttack();
                return;
            }

            owner.Agent.SetDestination(owner.Player.transform.position);
        }
    }
}
