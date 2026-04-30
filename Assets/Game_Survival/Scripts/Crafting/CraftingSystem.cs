using Framework.Core.DataManager;
using SurvivalGame.Data;
using SurvivalGame.Inventories;

namespace SurvivalGame.Crafting
{
    /// <summary>
    /// 크래프팅 로직을 담당합니다.
    /// 재료 확인 및 소모, 결과 아이템 지급을 처리합니다.
    /// </summary>
    public static class CraftingSystem
    {
        /// <summary>인벤토리에 레시피 재료가 충분한지 확인합니다.</summary>
        public static bool CanCraft(RecipeData recipe, Inventory inventory)
        {
            foreach (var ing in recipe.ingredients)
            {
                if (inventory.GetCount(ing.item_id) < ing.count)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 제작을 시도합니다.
        /// 재료가 충분하면 소모 후 결과 아이템을 지급하고 true를 반환합니다.
        /// </summary>
        public static bool TryCraft(RecipeData recipe, Inventory inventory)
        {
            if (!CanCraft(recipe, inventory)) return false;

            var resultItem = InGameDataManager.Instance.Get<SurvivalItemData>(recipe.result_item_id);
            if (resultItem == null) return false;

            foreach (var ing in recipe.ingredients)
                inventory.TryRemove(ing.item_id, ing.count);

            inventory.TryAdd(resultItem, recipe.result_count);
            return true;
        }
    }
}
