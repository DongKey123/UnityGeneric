namespace SurvivalGame.Battle
{
    /// <summary>적이 플레이어를 탐지 범위 내에서 발견했을 때 발행합니다.</summary>
    public struct EnemyDetectedPlayerEvent
    {
        /// <summary>탐지한 적</summary>
        public Enemy Source;
    }

    /// <summary>적이 사망했을 때 발행합니다.</summary>
    public struct EnemyDiedEvent
    {
        /// <summary>사망한 적</summary>
        public Enemy Source;
    }

    /// <summary>적이 플레이어 공격 범위에 진입했을 때 발행합니다.</summary>
    public struct EnemyEnteredAttackRangeEvent
    {
        /// <summary>범위에 진입한 적</summary>
        public Enemy Source;
    }

    /// <summary>적이 플레이어 공격 범위에서 이탈했을 때 발행합니다.</summary>
    public struct EnemyExitedAttackRangeEvent
    {
        /// <summary>범위에서 이탈한 적</summary>
        public Enemy Source;
    }

    /// <summary>적이 플레이어를 공격했을 때 발행합니다.</summary>
    public struct EnemyAttackedEvent
    {
        /// <summary>공격한 적</summary>
        public Enemy Source;

        /// <summary>최종 피해량</summary>
        public int Damage;
    }
}
