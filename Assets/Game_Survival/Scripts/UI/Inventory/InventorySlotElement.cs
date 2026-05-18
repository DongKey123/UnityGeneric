using System;
using SurvivalGame.Inventories;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivalGame.UI
{
    /// <summary>
    /// 인벤토리 그리드의 슬롯 한 칸입니다.
    /// 선택(금색 glow) / 장착중(초록 glow + 배지) 상태를 시각적으로 표시합니다.
    /// </summary>
    public class InventorySlotElement : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private Image           _slotBg;
        [SerializeField] private Image           _iconImage;
        [SerializeField] private TextMeshProUGUI _countText;
        [SerializeField] private Image           _selectedGlow;
        [SerializeField] private Image           _equippedGlow;
        [SerializeField] private Image           _equippedBadge;
        [SerializeField] private Button          _button;

        [Header("Sprites")]
        [SerializeField] private Sprite _spriteEmpty;
        [SerializeField] private Sprite _spriteNormal;

        #endregion

        #region Events

        public event Action<InventorySlotElement> OnClicked;

        #endregion

        #region Properties

        public InventorySlot Slot { get; private set; }

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            _button.onClick.AddListener(() => OnClicked?.Invoke(this));
        }

        #endregion

        #region Public Methods

        public void Refresh(InventorySlot slot)
        {
            Slot = slot;

            if (slot == null)
            {
                SetEmpty();
                return;
            }

            _slotBg.sprite    = _spriteNormal;
            _iconImage.enabled = true;

            var icon = Resources.Load<Sprite>(slot.Data.icon_path);
            if (icon != null) _iconImage.sprite = icon;

            _countText.text = slot.Data.max_stack > 1 && slot.Count > 1
                ? slot.Count.ToString()
                : string.Empty;

            SetEquipped(slot.IsEquipped);
        }

        public void SetSelected(bool selected)
        {
            _selectedGlow.enabled = selected;
        }

        public void SetEquipped(bool equipped)
        {
            _equippedGlow.enabled  = equipped;
            _equippedBadge.enabled = equipped;
        }

        #endregion

        #region Private Methods

        private void SetEmpty()
        {
            _slotBg.sprite     = _spriteEmpty;
            _iconImage.enabled = false;
            _countText.text    = string.Empty;
            SetSelected(false);
            SetEquipped(false);
        }

        #endregion
    }
}
