namespace SurvivalGame.Battle
{
    /// <summary>
    /// 피해를 받을 수 있는 오브젝트가 구현하는 인터페이스입니다.
    /// 플레이어, 적 등 피격 대상 모두에 적용합니다.
    /// </summary>
    public interface IDamageable
    {
        /// <summary>사망 여부</summary>
        bool IsDead { get; }

        /// <summary>
        /// 피해를 받습니다.
        /// 구현체가 내부적으로 방어력을 적용하여 최종 피해를 계산합니다.
        /// </summary>
        /// <param name="attackPower">공격자의 공격력</param>
        void TakeDamage(int attackPower);
    }
}
