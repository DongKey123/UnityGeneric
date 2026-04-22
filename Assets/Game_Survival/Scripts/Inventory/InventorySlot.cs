using SurvivalGame.Data;

namespace SurvivalGame.Inventories
{
    /// <summary>
    /// 런타임 인벤토리 슬롯입니다.
    /// 아이템 데이터, 현재 수량, 현재 내구도를 보관합니다.
    /// </summary>
    public class InventorySlot
    {
        /// <summary>슬롯에 담긴 아이템 데이터</summary>
        public SurvivalItemData Data { get; }

        /// <summary>현재 수량</summary>
        public int Count { get; private set; }

        /// <summary>현재 내구도 (장비가 아니면 0)</summary>
        public int Durability { get; private set; }

        /// <summary>슬롯이 꽉 찼는지 여부</summary>
        public bool IsFull => Count >= Data.max_stack;

        /// <summary>슬롯 무게 합계</summary>
        public float TotalWeight => Data.weight * Count;

        public InventorySlot(SurvivalItemData data, int count = 1)
        {
            Data       = data;
            Count      = count;
            Durability = data.durability_max;
        }

        /// <summary>수량을 추가합니다. 최대 스택을 초과하지 않습니다.</summary>
        /// <returns>실제로 추가된 수량</returns>
        public int Add(int amount)
        {
            int addable = Data.max_stack - Count;
            int added   = System.Math.Min(amount, addable);
            Count += added;
            return added;
        }

        /// <summary>수량을 제거합니다.</summary>
        /// <returns>실제로 제거된 수량</returns>
        public int Remove(int amount)
        {
            int removed = System.Math.Min(amount, Count);
            Count -= removed;
            return removed;
        }

        /// <summary>내구도를 감소시킵니다.</summary>
        public void ReduceDurability(int amount)
        {
            Durability = System.Math.Max(0, Durability - amount);
        }
    }
}
