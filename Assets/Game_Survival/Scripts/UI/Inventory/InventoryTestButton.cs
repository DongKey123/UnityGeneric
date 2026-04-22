using Framework.Core.DataManager;
using SurvivalGame.Data;
using SurvivalGame.Player;
using UnityEngine;

namespace SurvivalGame.UI
{
    /// <summary>
    /// 인벤토리 테스트용 임시 버튼입니다.
    /// 버튼 클릭 시 지정한 아이템을 인벤토리에 추가합니다.
    /// 테스트 완료 후 제거하세요.
    /// </summary>
    public class InventoryTestButton : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private PlayerController _player;
        [SerializeField] private int              _itemId = 1001;
        [SerializeField] private int              _count  = 1;

        #endregion

        #region Unity Lifecycle

        private void Start()
        {
            if (_player == null)
                _player = FindFirstObjectByType<PlayerController>();
        }

        #endregion

        #region Public Methods

        /// <summary>버튼 OnClick에 연결하세요.</summary>
        public void OnClickAddItem()
        {
            var data = InGameDataManager.Instance.Get<SurvivalItemData>(_itemId);
            if (data == null)
            {
                Debug.LogWarning($"[InventoryTest] item_id {_itemId} 없음 — SurvivalEntry가 씬에 있는지 확인하세요.");
                return;
            }

            bool success = _player.Inventory.TryAdd(data, _count);
            Debug.Log($"[InventoryTest] {data.name} x{_count} 추가 {(success ? "성공" : "실패")}");
        }

        #endregion
    }
}
