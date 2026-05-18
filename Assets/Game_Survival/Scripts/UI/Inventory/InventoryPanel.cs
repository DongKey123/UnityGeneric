using Framework.UI;
using SurvivalGame.Inventories;
using SurvivalGame.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivalGame.UI
{
    /// <summary>
    /// 인벤토리 패널입니다. 장비 슬롯·아이템 그리드·상세 패널·액션 버튼으로 구성됩니다.
    /// UIManager.Open&lt;InventoryPanel, InventoryPanelData&gt;(data)로 열고, Close()로 닫습니다.
    /// 프리팹은 Resources/UI/InventoryPanel 경로에 저장하세요.
    /// </summary>
    public class InventoryPanel : UIPanel, IInitializable<InventoryPanelData>
    {
        #region Constants

        private const int SlotCount = 20;

        #endregion

        #region Inspector

        [Header("Header")]
        [SerializeField] private Button          _closeButton;
        [SerializeField] private TextMeshProUGUI _weightText;
        [SerializeField] private TextMeshProUGUI _slotText;

        [Header("Sub Panels")]
        [SerializeField] private EquipmentSlotsSubPanel _equipmentPanel;
        [SerializeField] private ItemDetailSubPanel     _detailPanel;

        [Header("Grid")]
        [SerializeField] private Transform            _slotGrid;
        [SerializeField] private InventorySlotElement _slotElementPrefab;

        [Header("Footer Buttons")]
        [SerializeField] private Button _btnUse;
        [SerializeField] private Button _btnEquip;
        [SerializeField] private Button _btnUnequip;
        [SerializeField] private Button _btnDrop;

        #endregion

        #region Private Fields

        private Inventory              _inventory;
        private EquipmentSlots         _equipment;
        private InventorySlotElement[] _slotElements;
        private InventorySlotElement   _selectedElement;

        #endregion

        #region UIPanel Lifecycle

        protected override void Awake()
        {
            base.Awake();

            _closeButton.onClick.AddListener(() => UIManager.Instance.Close());
            _btnUse.onClick.AddListener(OnClickUse);
            _btnEquip.onClick.AddListener(OnClickEquip);
            _btnUnequip.onClick.AddListener(OnClickUnequip);
            _btnDrop.onClick.AddListener(OnClickDrop);

            _slotElements = new InventorySlotElement[SlotCount];
            for (int i = 0; i < SlotCount; i++)
            {
                _slotElements[i] = Instantiate(_slotElementPrefab, _slotGrid);
                _slotElements[i].OnClicked += OnSlotClicked;
            }
        }

        protected override void OnOpened()
        {
            _inventory.OnChanged += RefreshGrid;
            _equipment.OnChanged += RefreshGrid;

            _equipmentPanel.Setup(_equipment);
            _equipmentPanel.Show();

            Select(null);
            RefreshGrid();
        }

        protected override void OnClosed()
        {
            _inventory.OnChanged -= RefreshGrid;
            _equipment.OnChanged -= RefreshGrid;

            _equipmentPanel.Cleanup();
            _equipmentPanel.Hide();
            _detailPanel.Hide();
        }

        #endregion

        #region IInitializable

        public void Initialize(InventoryPanelData data)
        {
            _inventory = data.Inventory;
            _equipment = data.Equipment;
        }

        #endregion

        #region Private Methods

        private void RefreshGrid()
        {
            var slots = _inventory.Slots;

            for (int i = 0; i < _slotElements.Length; i++)
            {
                InventorySlot slot = i < slots.Count ? slots[i] : null;
                _slotElements[i].Refresh(slot);
            }

            // 선택된 슬롯이 변경됐을 수 있으므로 선택 상태 재표시
            if (_selectedElement != null)
                _selectedElement.SetSelected(true);

            RefreshHeader();
            RefreshFooter();
        }

        private void RefreshHeader()
        {
            _weightText.text = $"{_inventory.CurrentWeight:F1} / {_inventory.MaxWeight:F0} kg";
            _slotText.text   = $"{_inventory.UsedSlots} / {_inventory.MaxSlots}";
        }

        private void RefreshFooter()
        {
            var slot = _selectedElement != null ? _selectedElement.Slot : null;

            bool hasSelection = slot != null;
            bool isEquipment  = hasSelection && slot.Data.category == "Equipment";
            bool isEquipped   = hasSelection && slot.IsEquipped;
            bool isConsumable = hasSelection && slot.Data.category == "Consumable";

            _btnUse.gameObject.SetActive(isConsumable);
            _btnEquip.gameObject.SetActive(isEquipment && !isEquipped);
            _btnUnequip.gameObject.SetActive(isEquipment && isEquipped);
            _btnDrop.gameObject.SetActive(hasSelection);
        }

        private void Select(InventorySlotElement element)
        {
            if (_selectedElement != null)
                _selectedElement.SetSelected(false);

            _selectedElement = element;

            if (_selectedElement != null && _selectedElement.Slot != null)
            {
                _selectedElement.SetSelected(true);
                _detailPanel.SetItem(_selectedElement.Slot);
                if (!_detailPanel.IsVisible) _detailPanel.Show();
            }
            else
            {
                _selectedElement = null;
                _detailPanel.Hide();
            }

            RefreshFooter();
        }

        private void OnSlotClicked(InventorySlotElement element)
        {
            if (element == _selectedElement)
                Select(null);
            else
                Select(element != null && element.Slot != null ? element : null);
        }

        private void OnClickUse()
        {
            if (_selectedElement == null) return;
            var slot = _selectedElement.Slot;
            if (slot == null) return;

            _inventory.TryRemove(slot.Data.item_id, 1);
            Select(null);
        }

        private void OnClickEquip()
        {
            if (_selectedElement == null) return;
            var slot = _selectedElement.Slot;
            if (slot == null) return;

            _equipment.TryEquip(slot);
            RefreshGrid();
        }

        private void OnClickUnequip()
        {
            if (_selectedElement == null) return;
            var slot = _selectedElement.Slot;
            if (slot == null) return;

            _equipment.TryUnequip(slot.Data.SlotType);
            RefreshGrid();
        }

        private void OnClickDrop()
        {
            if (_selectedElement == null) return;
            var slot = _selectedElement.Slot;
            if (slot == null) return;

            if (slot.IsEquipped)
                _equipment.TryUnequip(slot.Data.SlotType);

            _inventory.TryRemove(slot.Data.item_id, slot.Count);
            Select(null);
        }

        #endregion
    }
}
