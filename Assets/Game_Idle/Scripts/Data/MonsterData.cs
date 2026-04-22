using System;

namespace IdleGame.Data
{
    /// <summary>몬스터 스탯 및 드롭 데이터입니다.</summary>
    [Serializable]
    public class MonsterData
    {
        /// <summary>몬스터 고유 ID</summary>
        public int monster_id;

        /// <summary>몬스터 이름</summary>
        public string name;

        /// <summary>최대 체력</summary>
        public int hp;

        /// <summary>공격력</summary>
        public int atk;

        /// <summary>방어력</summary>
        public int def;

        /// <summary>공격 속도 (초당 횟수)</summary>
        public float atk_speed;

        /// <summary>이동 속도</summary>
        public float move_speed;

        /// <summary>공격 사거리</summary>
        public float atk_range;

        /// <summary>플레이어 탐지 범위 (ChaseState 진입 거리)</summary>
        public float detect_range;

        /// <summary>처치 시 지급 경험치</summary>
        public int exp;

        /// <summary>처치 시 지급 골드</summary>
        public int gold;
    }
}
