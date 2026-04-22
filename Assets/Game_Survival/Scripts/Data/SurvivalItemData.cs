using System;

namespace SurvivalGame.Data
{
    /// <summary>서바이벌 게임 아이템 데이터입니다.</summary>
    [Serializable]
    public class SurvivalItemData
    {
        /// <summary>아이템 고유 ID</summary>
        public int item_id;

        /// <summary>아이템 이름</summary>
        public string name;

        /// <summary>아이템 설명</summary>
        public string description;

        /// <summary>분류 — Resource / Consumable / Equipment / Special</summary>
        public string category;

        /// <summary>무게 (인벤토리 무게 합산에 사용)</summary>
        public float weight;

        /// <summary>최대 스택 수 (1이면 스택 불가)</summary>
        public int max_stack;

        /// <summary>아이콘 경로 (Resources 기준)</summary>
        public string icon_path;

        // 장비 전용 필드 (category == "Equipment" 일 때만 사용)

        /// <summary>장비 슬롯 — Weapon / Armor / Tool (장비가 아니면 빈 문자열)</summary>
        public string equipment_slot;

        /// <summary>최대 내구도 (장비가 아니면 0)</summary>
        public int durability_max;

        /// <summary>장비 티어 (1 ~ N)</summary>
        public int tier;
    }
}
