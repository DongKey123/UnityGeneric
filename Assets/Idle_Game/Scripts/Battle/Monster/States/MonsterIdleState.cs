using Framework.Patterns.StateMachine;
using UnityEngine;

namespace IdleGame.Battle
{
    /// <summary>
    /// 몬스터 대기 상태입니다.
    /// 스폰 후 0.5초 대기한 뒤 추격 상태로 전환합니다.
    /// </summary>
    public class MonsterIdleState : BaseState<MonsterController>
    {
        private float _timer;
        private const float SpawnDelay = 0.5f;

        public override void Enter(MonsterController owner)
        {
            _timer = 0f;
            owner.Agent.ResetPath();

            // TODO: 스폰 연출 (이펙트)
        }

        public override void Update(MonsterController owner)
        {
            _timer += Time.deltaTime;
            if (_timer >= SpawnDelay)
                owner.ChangeToChase();
        }
    }
}
