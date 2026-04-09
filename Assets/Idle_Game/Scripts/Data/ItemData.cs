using System;

namespace IdleGame.Data
{
    /// <summary>장비 아이템 기본 스탯 데이터입니다.</summary>
    [Serializable]
    public class ItemData
    {
        /// <summary>아이템 고유 ID</summary>
        public int item_id;

        /// <summary>아이템 이름</summary>
        public string name;

        /// <summary>
        /// 장비 슬롯 타입
        /// weapon / helmet / armor / gloves / boots
        /// </summary>
        public string slot_type;

        /// <summary>공격력 보너스</summary>
        public int atk;

        /// <summary>공격 사거리 보너스</summary>
        public float atk_range;

        /// <summary>최대 체력 보너스</summary>
        public int max_hp;

        /// <summary>방어력 보너스</summary>
        public int def;

        /// <summary>공격 속도 보너스</summary>
        public float atk_speed;

        /// <summary>이동 속도 보너스</summary>
        public float move_speed;
    }
}
