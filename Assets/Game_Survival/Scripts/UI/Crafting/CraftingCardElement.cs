using System;
using System.Collections.Generic;
using Framework.Core.DataManager;
using SurvivalGame.Crafting;
using SurvivalGame.Data;
using SurvivalGame.Inventories;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivalGame.UI
{
    /// <summary>
    /// 크래프팅/빌딩 탭 공통 레시피·건물 카드 Element입니다.
    /// CraftingPanel에서 목록을 동적으로 생성합니다.
    /// </summary>
    public class CraftingCardElement : MonoBehaviour
    {
        #region Inspector

        [Header("Card")]
        [SerializeField] private Image             _bg;
        [SerializeField] private Button            _button;

        [Header("Icon")]
        [SerializeField] private Image             _iconSlot;
        [SerializeField] private Image             _icon;
        [SerializeField] private TextMeshProUGUI   _countBadge;

        [Header("Info")]
        [SerializeField] private TextMeshProUGUI   _nameText;
        [SerializeField] private Transform         _ingredientsRoot;

        [Header("Badges")]
        [SerializeField] private GameObject        _lvBadgeGo;
        [SerializeField] private TextMeshProUGUI   _lvText;
        [SerializeField] private GameObject        _gridBadgeGo;
        [SerializeField] private TextMeshProUGUI   _gridText;

        [Header("State Sprites")]
        [SerializeField] private Sprite _cardNormal;
        [SerializeField] private Sprite _cardSelected;
        [SerializeField] private Sprite _cardDisabled;
        [SerializeField] private Sprite _cardInProgressCraft;
        [SerializeField] private Sprite _cardInProgressBuild;

        [Header("Prefabs")]
        [SerializeField] private IngredientChipElement _chipPrefab;

        #endregion

        public event Action<CraftingCardElement> OnClicked;

        public RecipeData   BoundRecipe   { get; private set; }
        public BuildingData BoundBuilding { get; private set; }

        private bool _isSelected;

        // ─────────────────────────────────────────
        // Unity
        // ─────────────────────────────────────────

        private void Awake()
        {
            _button.onClick.AddListener(() => OnClicked?.Invoke(this));
        }

        // ─────────────────────────────────────────
        // Setup
        // ─────────────────────────────────────────

        public void SetupRecipe(RecipeData recipe, Inventory inventory)
        {
            BoundRecipe   = recipe;
            BoundBuilding = null;

            _nameText.text = recipe.name;
            _lvBadgeGo.SetActive(false);
            _gridBadgeGo.SetActive(false);

            var resultItem = InGameDataManager.Instance.Get<SurvivalItemData>(recipe.result_item_id);
            if (resultItem != null && !string.IsNullOrEmpty(resultItem.icon_path))
            {
                var sp = Resources.Load<Sprite>(resultItem.icon_path);
                _icon.sprite  = sp;
                _icon.enabled = sp != null;
            }
            else
            {
                _icon.enabled = false;
            }

            RebuildIngredientChips(ToList(recipe.ingredients), inventory);
            RefreshState(inventory);
        }

        public void SetupBuilding(BuildingData building, Inventory inventory)
        {
            BoundBuilding = building;
            BoundRecipe   = null;

            _nameText.text = building.name;
            _icon.enabled  = false;
            _lvBadgeGo.SetActive(false);

            bool multiGrid = building.grid_width > 1 || building.grid_height > 1;
            _gridBadgeGo.SetActive(multiGrid);
            if (multiGrid)
                _gridText.text = $"{building.grid_width}×{building.grid_height}";

            RebuildIngredientChips(ToList(building.costs), inventory);
            RefreshState(inventory);
        }

        // ─────────────────────────────────────────
        // 상태 갱신
        // ─────────────────────────────────────────

        public void SetSelected(bool selected)
        {
            _isSelected = selected;
            UpdateSprite(null);
        }

        public void RefreshState(Inventory inventory)
        {
            UpdateSprite(CanMake(inventory));
        }

        public bool CanMake(Inventory inventory)
        {
            if (BoundRecipe != null)
                return CraftingSystem.CanCraft(BoundRecipe, inventory);

            if (BoundBuilding != null && BoundBuilding.costs != null)
            {
                foreach (var c in BoundBuilding.costs)
                    if (inventory.GetCount(c.item_id) < c.count) return false;
                return true;
            }
            return false;
        }

        // ─────────────────────────────────────────
        // Private helpers
        // ─────────────────────────────────────────

        private void UpdateSprite(bool? canMake)
        {
            if (_bg == null) return;
            if (_isSelected)
                _bg.sprite = _cardSelected;
            else if (canMake == false)
                _bg.sprite = _cardDisabled;
            else
                _bg.sprite = _cardNormal;
        }

        private void RebuildIngredientChips(List<(int id, int need)> items, Inventory inventory)
        {
            if (_ingredientsRoot == null) return;
            foreach (Transform child in _ingredientsRoot)
                Destroy(child.gameObject);

            if (_chipPrefab == null) return;

            foreach (var (id, need) in items)
            {
                int have = inventory != null ? inventory.GetCount(id) : 0;
                var itemData = InGameDataManager.Instance.Get<SurvivalItemData>(id);
                string itemName = itemData != null ? itemData.name : $"#{id}";

                var chip = Instantiate(_chipPrefab, _ingredientsRoot, false);
                chip.Setup(itemName, have, need, 13f);
            }
        }

        private static List<(int, int)> ToList(List<RecipeIngredient> src)
        {
            var r = new List<(int, int)>();
            if (src != null) foreach (var i in src) r.Add((i.item_id, i.count));
            return r;
        }

        private static List<(int, int)> ToList(List<BuildingCost> src)
        {
            var r = new List<(int, int)>();
            if (src != null) foreach (var c in src) r.Add((c.item_id, c.count));
            return r;
        }
    }
}
