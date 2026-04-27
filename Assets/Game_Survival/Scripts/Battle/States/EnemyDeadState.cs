using Framework.Core.EventBus;
using Framework.Patterns.StateMachine;
using UnityEngine;

namespace SurvivalGame.Battle
{
    /// <summary>
    /// 적 사망 상태입니다.
    /// 드롭 처리 후 GameObject를 제거합니다.
    /// </summary>
    public class EnemyDeadState : BaseState<Enemy>
    {
        private float _timer;
        private const float DeadDuration = 0.5f;

        public override void Enter(Enemy owner)
        {
            _timer = 0f;
            owner.Agent.ResetPath();

            EventBus.Publish(new EnemyDiedEvent { Source = owner });
        }

        public override void Update(Enemy owner)
        {
            _timer += Time.deltaTime;
            if (_timer >= DeadDuration)
                Object.Destroy(owner.gameObject);
        }
    }
}
