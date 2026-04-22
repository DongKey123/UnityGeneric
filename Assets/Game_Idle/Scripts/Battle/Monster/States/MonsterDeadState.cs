using Framework.Patterns.StateMachine;
using UnityEngine;

namespace IdleGame.Battle
{
    /// <summary>
    /// 몬스터 사망 상태입니다.
    /// 사망 연출 후 드롭 처리, ObjectPool 반환합니다.
    /// </summary>
    public class MonsterDeadState : BaseState<MonsterController>
    {
        private float _timer;
        private const float DeadDuration = 0.5f;

        public override void Enter(MonsterController owner)
        {
            _timer = 0f;
            owner.Agent.ResetPath();

            // TODO: 사망 애니메이션 (0.5초)
        }

        public override void Update(MonsterController owner)
        {
            _timer += Time.deltaTime;
            if (_timer >= DeadDuration)
                owner.ReturnToPool();
        }
    }
}
