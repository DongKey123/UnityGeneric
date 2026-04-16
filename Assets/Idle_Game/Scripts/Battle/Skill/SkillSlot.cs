using IdleGame.Data;

namespace IdleGame.Battle
{
    /// <summary>
    /// 런타임 스킬 슬롯입니다.
    /// SkillData 원본과 현재 강화 단계, 쿨타임 상태를 보관합니다.
    /// </summary>
    public class SkillSlot
    {
        /// <summary>스킬 원본 데이터</summary>
        public SkillData Data { get; }

        /// <summary>현재 강화 단계</summary>
        public int EnhanceLevel { get; private set; }

        /// <summary>쿨타임 잔여 시간 (0 이면 사용 가능)</summary>
        public float CooldownRemaining { get; private set; }

        /// <summary>사용 가능 여부</summary>
        public bool IsReady => CooldownRemaining <= 0f;

        public SkillSlot(SkillData data, int enhanceLevel = 0)
        {
            Data          = data;
            EnhanceLevel  = enhanceLevel;
        }

        /// <summary>쿨타임을 시작합니다.</summary>
        public void StartCooldown()
        {
            CooldownRemaining = Data.cooldown;
        }

        /// <summary>쿨타임을 deltaTime만큼 줄입니다.</summary>
        public void TickCooldown(float deltaTime)
        {
            if (CooldownRemaining > 0f)
                CooldownRemaining -= deltaTime;
        }

        /// <summary>
        /// 강화가 적용된 damage_ratio를 반환합니다.
        /// enhance_target이 "damage_ratio"가 아니면 원본 값을 그대로 반환합니다.
        /// </summary>
        public float GetDamageRatio()
        {
            if (Data.enhance_target == "damage_ratio")
                return Data.damage_ratio + Data.enhance_value_per_level * EnhanceLevel;
            return Data.damage_ratio;
        }

        /// <summary>
        /// 강화가 적용된 range를 반환합니다.
        /// enhance_target이 "range"가 아니면 원본 값을 그대로 반환합니다.
        /// </summary>
        public float GetRange()
        {
            if (Data.enhance_target == "range")
                return Data.range + Data.enhance_value_per_level * EnhanceLevel;
            return Data.range;
        }
    }
}
