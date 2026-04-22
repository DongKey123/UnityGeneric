namespace IdleGame.Battle
{
    /// <summary>
    /// 피해를 받을 수 있는 오브젝트가 구현하는 인터페이스입니다.
    /// 플레이어, 몬스터, 함정 등 피격 대상 모두에 적용합니다.
    /// </summary>
    public interface IDamageable
    {
        /// <summary>사망 여부</summary>
        bool IsDead { get; }

        /// <summary>
        /// 피해를 받습니다.
        /// 공격자의 공격력(크리티컬 배율 적용 후)을 전달하면, 구현체가 내부적으로 DEF를 적용하여 최종 피해를 계산합니다.
        /// </summary>
        /// <param name="attackPower">공격자의 공격력 (크리티컬 적용 후)</param>
        void TakeDamage(int attackPower);
    }
}
