using SurvivalGame.Inventories;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivalGame.UI
{
    /// <summary>
    /// 선택된 아이템의 상세 정보를 표시하는 서브 패널입니다.
    /// </summary>
    public class ItemDetailSubPanel : Framework.UI.SubPanel
    {
        #region Inspector

        [SerializeField] private Image           _iconImage;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private Image           _categoryTag;
        [SerializeField] private TextMeshProUGUI _categoryText;
        [SerializeField] private TextMeshProUGUI _descText;
        [SerializeField] private TextMeshProUGUI _weightText;

        [Header("Durability (Equipment only)")]
        [SerializeField] private GameObject      _duraSection;
        [SerializeField] private Image           _durabarFill;
        [SerializeField] private TextMeshProUGUI _tierText;

        [Header("Category Tag Sprites")]
        [SerializeField] private Sprite _tagResource;
        [SerializeField] private Sprite _tagConsume;
        [SerializeField] private Sprite _tagEquipment;

        [Header("Dura Fill Sprites")]
        [SerializeField] private Sprite _duraHigh;
        [SerializeField] private Sprite _duraMid;
        [SerializeField] private Sprite _duraLow;

        #endregion

        #region Private Fields

        private InventorySlot _slot;

        #endregion

        #region Public Methods

        public void SetItem(InventorySlot slot)
        {
            _slot = slot;
            if (IsVisible) Refresh();
        }

        #endregion

        #region SubPanel

        protected override void OnShown()   => Refresh();
        protected override void OnRefresh() => Refresh();

        #endregion

        #region Private Methods

        private void Refresh()
        {
            if (_slot == null) return;

            var data = _slot.Data;

            // 아이콘
            var icon = Resources.Load<Sprite>(data.icon_path);
            _iconImage.sprite  = icon;
            _iconImage.enabled = icon != null;

            // 이름 / 설명 / 무게
            _nameText.text   = data.name;
            _descText.text   = data.description;
            _weightText.text = $"{data.weight:F1} kg";

            // 카테고리 태그
            switch (data.category)
            {
                case "Resource":
                    _categoryTag.sprite = _tagResource;
                    _categoryText.text  = "자원";
                    break;
                case "Consumable":
                    _categoryTag.sprite = _tagConsume;
                    _categoryText.text  = "소비";
                    break;
                case "Equipment":
                    _categoryTag.sprite = _tagEquipment;
                    _categoryText.text  = "장비";
                    break;
                default:
                    _categoryTag.sprite = _tagResource;
                    _categoryText.text  = data.category;
                    break;
            }

            // 내구도 / 티어 (장비 전용)
            bool isEquipment = data.category == "Equipment" && data.durability_max > 0;
            _duraSection.SetActive(isEquipment);

            if (isEquipment)
            {
                float ratio = (float)_slot.Durability / data.durability_max;
                _durabarFill.fillAmount = ratio;
                _durabarFill.sprite     = ratio > 0.5f ? _duraHigh
                                        : ratio > 0.25f ? _duraMid
                                        : _duraLow;
                _tierText.text = $"Tier {data.tier}";
            }
        }

        #endregion
    }
}
