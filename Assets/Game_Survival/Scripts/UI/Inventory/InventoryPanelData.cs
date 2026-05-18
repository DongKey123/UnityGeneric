using SurvivalGame.Inventories;
using SurvivalGame.Player;

namespace SurvivalGame.UI
{
    /// <summary>
    /// InventoryPanel을 열 때 전달하는 데이터 묶음입니다.
    /// </summary>
    public class InventoryPanelData
    {
        public Inventory      Inventory;
        public EquipmentSlots Equipment;
    }
}
