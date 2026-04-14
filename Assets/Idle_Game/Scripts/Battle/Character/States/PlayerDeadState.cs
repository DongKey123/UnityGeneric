using Framework.Patterns.StateMachine;
using UnityEngine;

namespace IdleGame.Battle
{
    /// <summary>
    /// 플레이어 사망 상태입니다.
    /// 사망 연출 후 3초 대기, HP 전체 회복 후 대기 상태로 복귀합니다.
    /// </summary>
    public class PlayerDeadState : BaseState<PlayerController>
    {
        private float _timer;
        private const float ReviveDelay = 3f;

        public override void Enter(PlayerController owner)
        {
            _timer = 0f;
            owner.Agent.ResetPath();

            // TODO: 사망 애니메이션 (0.5초)
        }

        public override void Update(PlayerController owner)
        {
            _timer += Time.deltaTime;
            if (_timer >= ReviveDelay)
                owner.Revive();
        }
    }
}
