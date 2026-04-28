using SurvivalGame.Data;
using UnityEngine;

namespace SurvivalGame.Building
{
    /// <summary>
    /// 배치된 건물 오브젝트에 붙는 컴포넌트입니다.
    /// 그리드 정보를 보유하며 철거 시 그리드 해제와 자원 반환을 처리합니다.
    /// </summary>
    public class PlacedBuilding : MonoBehaviour
    {
        #region Properties

        public BuildingData Data   { get; private set; }
        public Vector2Int   Origin { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>배치 완료 시 BuildingPlacer에서 호출합니다.</summary>
        public void Initialize(BuildingData data, Vector2Int origin)
        {
            Data   = data;
            Origin = origin;
        }

        /// <summary>건물을 철거합니다. 재료의 50%를 반환합니다.</summary>
        public void Demolish(Inventories.Inventory inventory)
        {
            BuildingGrid.Instance.Unregister(Origin, Data.grid_width, Data.grid_height);

            foreach (var cost in Data.costs)
            {
                var itemData = InGameDataManager.Instance.Get<SurvivalItemData>(cost.item_id);
                if (itemData == null) continue;

                int refund = Mathf.Max(1, cost.count / 2);
                inventory.TryAdd(itemData, refund);
            }

            Destroy(gameObject);
        }

        #endregion
    }
}
