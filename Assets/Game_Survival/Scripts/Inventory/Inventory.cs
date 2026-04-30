using System;
using System.Collections.Generic;
using SurvivalGame.Data;

namespace SurvivalGame.Inventories
{
    /// <summary>
    /// 플레이어 인벤토리입니다.
    /// 슬롯 수 + 무게 이중 제한으로 아이템을 관리합니다.
    /// PlayerController가 소유하며 순수 C# 클래스로 구현됩니다.
    /// </summary>
    public class Inventory
    {
        #region Events

        /// <summary>인벤토리 내용이 변경될 때 발생합니다. (UI 갱신용)</summary>
        public event Action OnChanged;

        #endregion

        #region Fields

        private readonly List<InventorySlot> _slots = new();

        #endregion

        #region Properties

        /// <summary>최대 슬롯 수</summary>
        public int MaxSlots { get; }

        /// <summary>최대 무게</summary>
        public float MaxWeight { get; }

        /// <summary>현재 사용 중인 슬롯 수</summary>
        public int UsedSlots => _slots.Count;

        /// <summary>현재 총 무게</summary>
        public float CurrentWeight
        {
            get
            {
                float total = 0f;
                foreach (var slot in _slots)
                    total += slot.TotalWeight;
                return total;
            }
        }

        /// <summary>슬롯 목록 (읽기 전용, UI 표시용)</summary>
        public IReadOnlyList<InventorySlot> Slots => _slots;

        #endregion

        #region Constructor

        public Inventory(int maxSlots, float maxWeight)
        {
            MaxSlots  = maxSlots;
            MaxWeight = maxWeight;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 아이템 추가를 시도합니다.
        /// 슬롯이 꽉 찼거나 무게 초과 시 false를 반환합니다.
        /// </summary>
        /// <param name="data">추가할 아이템 데이터</param>
        /// <param name="count">추가할 수량</param>
        /// <returns>모두 추가됐으면 true, 일부라도 실패하면 false</returns>
        public bool TryAdd(SurvivalItemData data, int count = 1)
        {
            if (CurrentWeight + data.weight * count > MaxWeight) return false;

            int remaining = count;

            // 같은 아이템의 비어 있는 슬롯에 먼저 채움
            foreach (var slot in _slots)
            {
                if (slot.Data.item_id != data.item_id) continue;
                if (slot.IsFull) continue;

                remaining -= slot.Add(remaining);
                if (remaining <= 0) break;
            }

            // 새 슬롯 생성
            while (remaining > 0)
            {
                if (_slots.Count >= MaxSlots) return false;

                var newSlot = new InventorySlot(data);
                remaining--;                        // 생성자에서 1개 추가됨
                remaining -= newSlot.Add(remaining);
                _slots.Add(newSlot);
            }

            OnChanged?.Invoke();
            return true;
        }

        /// <summary>
        /// 아이템 제거를 시도합니다.
        /// 보유량이 부족하면 false를 반환합니다.
        /// </summary>
        /// <param name="itemId">제거할 아이템 ID</param>
        /// <param name="count">제거할 수량</param>
        /// <returns>모두 제거됐으면 true</returns>
        public bool TryRemove(int itemId, int count = 1)
        {
            if (GetCount(itemId) < count) return false;

            int remaining = count;
            for (int i = _slots.Count - 1; i >= 0 && remaining > 0; i--)
            {
                if (_slots[i].Data.item_id != itemId) continue;

                remaining -= _slots[i].Remove(remaining);
                if (_slots[i].Count <= 0)
                    _slots.RemoveAt(i);
            }

            OnChanged?.Invoke();
            return true;
        }

        /// <summary>특정 아이템의 보유 수량을 반환합니다.</summary>
        public int GetCount(int itemId)
        {
            int total = 0;
            foreach (var slot in _slots)
                if (slot.Data.item_id == itemId)
                    total += slot.Count;
            return total;
        }

        /// <summary>특정 아이템을 지정 수량 이상 보유하는지 확인합니다.</summary>
        public bool HasItem(int itemId, int count = 1) => GetCount(itemId) >= count;

        #endregion
    }
}
