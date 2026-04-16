using System;

namespace IdleGame.Data
{
    /// <summary>스킬 정의 데이터입니다.</summary>
    [Serializable]
    public class SkillData
    {
        /// <summary>스킬 고유 ID</summary>
        public int skill_id;

        /// <summary>스킬 이름</summary>
        public string name;

        /// <summary>스킬 설명</summary>
        public string description;

        /// <summary>타겟 방식 — Single / AoE / Self / Line</summary>
        public string target_type;

        /// <summary>AoE 중심 — Self / Target (target_type == AoE 일 때만 사용)</summary>
        public string aoe_center;

        /// <summary>효과 방식 — Damage / DoT / Buff / Debuff / Shield / Heal</summary>
        public string effect_type;

        /// <summary>발동 방식 — Instant / Cast / Passive / Reaction</summary>
        public string cast_type;

        /// <summary>쿨타임 (초)</summary>
        public float cooldown;

        /// <summary>시전 시간 (Instant는 0)</summary>
        public float cast_time;

        /// <summary>피해 배율 (ATK × ratio)</summary>
        public float damage_ratio;

        /// <summary>AoE 반경 / Line 길이 (Single·Self는 0)</summary>
        public float range;

        /// <summary>해금 방식 — Default(기본 지급) / Condition(조건 해금)</summary>
        public string unlock_type;

        /// <summary>해금 조건 (추후 확정)</summary>
        public string unlock_condition;

        /// <summary>최대 강화 단계</summary>
        public int max_enhance_level;

        /// <summary>강화 시 변경되는 필드명 (damage_ratio / range / cooldown 등)</summary>
        public string enhance_target;

        /// <summary>강화 단계당 증가량</summary>
        public float enhance_value_per_level;
    }
}
