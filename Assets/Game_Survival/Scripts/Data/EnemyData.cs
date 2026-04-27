using System;

namespace SurvivalGame.Data
{
    /// <summary>적 스탯 및 드롭 데이터입니다.</summary>
    [Serializable]
    public class EnemyData
    {
        /// <summary>적 고유 ID</summary>
        public int enemy_id;

        /// <summary>적 이름</summary>
        public string name;

        /// <summary>최대 체력</summary>
        public int hp_max;

        /// <summary>공격력</summary>
        public int attack;

        /// <summary>방어력</summary>
        public int defense;

        /// <summary>이동 속도</summary>
        public float move_speed;

        /// <summary>플레이어 탐지 반경 — 이 범위 안에 플레이어가 들어오면 추격 시작</summary>
        public float detect_radius;

        /// <summary>공격 사거리 — 이 범위 안에 플레이어가 있으면 공격 실행</summary>
        public float attack_radius;

        /// <summary>군집 반응 알림 반경 — 피격 시 이 범위 내 동료에게 추격 알림 전파</summary>
        public float alert_radius;

        /// <summary>공격 간격 (초)</summary>
        public float attack_interval;

        /// <summary>드롭 아이템 ID</summary>
        public int drop_item_id;

        /// <summary>최소 드롭 수량</summary>
        public int drop_count_min;

        /// <summary>최대 드롭 수량</summary>
        public int drop_count_max;

        /// <summary>프리팹 경로 (Resources 기준)</summary>
        public string prefab_path;
    }
}
