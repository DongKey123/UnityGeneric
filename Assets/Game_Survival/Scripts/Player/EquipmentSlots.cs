using System;
using System.Collections.Generic;
using SurvivalGame.Defines;
using SurvivalGame.Inventories;

namespace SurvivalGame.Player
{
    /// <summary>
    /// 플레이어 장비 슬롯을 관리합니다.
    /// 아이템은 인벤토리에 그대로 보관되며, 슬롯은 해당 InventorySlot의 참조만 보관합니다.
    /// 장착된 슬롯은 <see cref="InventorySlot.IsEquipped"/>가 true로 표시됩니다.
    /// PlayerController가 소유하며 순수 C# 클래스로 구현됩니다.
    /// </summary>
    public class EquipmentSlots
    {
        #region Events

        /// <summary>장비 슬롯 내용이 변경될 때 발생합니다. (UI 갱신용)</summary>
        public event Action OnChanged;

        #endregion

        #region Fields

        private readonly Dictionary<EquipmentSlotType, InventorySlot> _slots = new();

        #endregion

        #region Public Methods

        /// <summary>
        /// 인벤토리 슬롯을 장비 슬롯에 장착합니다.
        /// 해당 슬롯에 이미 아이템이 있으면 먼저 해제 후 교체합니다.
        /// </summary>
        /// <param name="slot">장착할 인벤토리 슬롯</param>
        /// <returns>장착에 성공하면 true</returns>
        public bool TryEquip(InventorySlot slot)
        {
            var slotType = slot.Data.SlotType;
            if (slotType == EquipmentSlotType.None) return false;

            // 기존 장비 해제
            if (_slots.TryGetValue(slotType, out var current))
                current.MarkUnequipped();

            _slots[slotType] = slot;
            slot.MarkEquipped();
            OnChanged?.Invoke();
            return true;
        }

        /// <summary>
        /// 해당 슬롯의 장비를 해제합니다.
        /// 아이템은 인벤토리에 그대로 남습니다.
        /// </summary>
        /// <param name="slotType">해제할 슬롯 타입</param>
        /// <returns>해제에 성공하면 true</returns>
        public bool TryUnequip(EquipmentSlotType slotType)
        {
            if (!_slots.TryGetValue(slotType, out var slot)) return false;

            slot.MarkUnequipped();
            _slots.Remove(slotType);
            OnChanged?.Invoke();
            return true;
        }

        /// <summary>해당 슬롯에 장착된 인벤토리 슬롯을 반환합니다. 없으면 null입니다.</summary>
        public InventorySlot GetEquipped(EquipmentSlotType slotType) =>
            _slots.TryGetValue(slotType, out var slot) ? slot : null;

        /// <summary>해당 슬롯에 아이템이 장착되어 있는지 확인합니다.</summary>
        public bool IsEquipped(EquipmentSlotType slotType) => _slots.ContainsKey(slotType);

        #endregion
    }
}
