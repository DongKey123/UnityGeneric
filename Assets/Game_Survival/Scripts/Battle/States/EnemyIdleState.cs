using Framework.Patterns.StateMachine;
using UnityEngine;

namespace SurvivalGame.Battle
{
    /// <summary>
    /// 적 대기 상태입니다.
    /// 매 프레임 플레이어와의 거리를 확인하여 탐지 반경 내 진입 시 추격 상태로 전환합니다.
    /// </summary>
    public class EnemyIdleState : BaseState<Enemy>
    {
        public override void Enter(Enemy owner)
        {
            owner.Agent.ResetPath();
        }

        public override void Update(Enemy owner)
        {
            if (owner.Player == null || owner.Player.IsDead) return;

            float dist = Vector3.Distance(owner.transform.position, owner.Player.transform.position);
            if (dist <= owner.Data.detect_radius)
                owner.ChangeToChase();
        }
    }
}
