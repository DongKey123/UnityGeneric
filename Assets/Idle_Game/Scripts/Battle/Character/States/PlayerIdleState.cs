using Framework.Patterns.StateMachine;
using UnityEngine;

namespace IdleGame.Battle
{
    /// <summary>
    /// 플레이어 대기 상태입니다.
    /// 주변 몬스터를 탐색하고, 탐지되면 전투 상태로 전환합니다.
    /// </summary>
    public class PlayerIdleState : BaseState<PlayerController>
    {
        private float _detectTimer;
        private const float DetectInterval = 0.5f;  // 0.5초마다 탐색

        public override void Enter(PlayerController owner)
        {
            _detectTimer = 0f;
            owner.Agent.ResetPath();
        }

        public override void Update(PlayerController owner)
        {
            _detectTimer += Time.deltaTime;
            if (_detectTimer < DetectInterval) return;

            _detectTimer = 0f;

            var target = owner.FindNearestTarget();
            if (target == null) return;

            owner.SetTarget(target);
            owner.ChangeToCombat();
        }
    }
}
