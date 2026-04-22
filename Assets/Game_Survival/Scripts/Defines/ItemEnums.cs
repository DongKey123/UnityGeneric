namespace SurvivalGame.Defines
{
    /// <summary>아이템 분류입니다.</summary>
    public enum ItemCategory
    {
        Resource   = 0,   // 자원류 — 스택 가능
        Consumable = 1,   // 소비류 — 스택 가능
        Equipment  = 2,   // 장비류 — 스택 불가, 내구도 있음
        Special    = 3    // 특수류 — 스택 불가 (퀘스트 아이템 등)
    }

    /// <summary>장비 슬롯 종류입니다.</summary>
    public enum EquipmentSlotType
    {
        None   = 0,
        Weapon = 1,
        Armor  = 2,
        Tool   = 3
    }
}
