using Framework.UI;
using SurvivalGame.Inventories;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivalGame.UI
{
    /// <summary>
    /// 인벤토리 패널입니다.
    /// UIManager.Open&lt;InventoryPanel, Inventory&gt;(inventory)으로 열고, Close()로 닫습니다.
    /// 프리팹은 Resources/UI/InventoryPanel 경로에 저장하세요.
    /// </summary>
    public class InventoryPanel : UIPanel, IInitializable<Inventory>
    {
        #region Constants

        private const int SlotCount = 20;

        #endregion

        #region Inspector

        [SerializeField] private Button               _closeButton;
        [SerializeField] private Transform            _slotGrid;
        [SerializeField] private InventorySlotElement _slotElementPrefab;

        #endregion

        #region Private Fields

        private Inventory              _inventory;
        private InventorySlotElement[] _slotElements;

        #endregion

        #region UIPanel Lifecycle

        protected override void Awake()
        {
            base.Awake();

            _closeButton.onClick.AddListener(() => UIManager.Instance.Close());

            _slotElements = new InventorySlotElement[SlotCount];
            for (int i = 0; i < SlotCount; i++)
                _slotElements[i] = Instantiate(_slotElementPrefab, _slotGrid);
        }

        protected override void OnOpened()
        {
            _inventory.OnChanged += Refresh;
            Refresh();
        }

        protected override void OnClosed()
        {
            _inventory.OnChanged -= Refresh;
        }

        #endregion

        #region IInitializable

        /// <summary>열릴 때 인벤토리를 주입받습니다.</summary>
        public void Initialize(Inventory inventory)
        {
            _inventory = inventory;
        }

        #endregion

        #region Private Methods

        private void Refresh()
        {
            var slots = _inventory.Slots;

            for (int i = 0; i < _slotElements.Length; i++)
            {
                InventorySlot slot = i < slots.Count ? slots[i] : null;
                _slotElements[i].Refresh(slot);
            }
        }

        #endregion
    }
}
