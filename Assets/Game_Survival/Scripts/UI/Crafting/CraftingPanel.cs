using System.Collections.Generic;
using Framework.Core.DataManager;
using Framework.UI;
using SurvivalGame.Crafting;
using SurvivalGame.Data;
using SurvivalGame.Inventories;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivalGame.UI
{
    /// <summary>
    /// 크래프팅 패널입니다.
    /// 제작 가능한 레시피 목록을 표시하고, 선택 시 재료를 소모해 아이템을 제작합니다.
    /// </summary>
    public class CraftingPanel : UIPanel, IInitializable<Inventory>
    {
        #region Inspector

        [SerializeField] private Button    _closeButton;
        [SerializeField] private Transform _recipeListRoot;
        [SerializeField] private Button    _recipeButtonTemplate;

        #endregion

        #region Private Fields

        private Inventory _inventory;
        private readonly List<(Button btn, RecipeData recipe)> _recipeButtons = new();

        #endregion

        #region UIPanel Lifecycle

        protected override void Awake()
        {
            base.Awake();
            _closeButton.onClick.AddListener(() => UIManager.Instance.Close());
            _recipeButtonTemplate.gameObject.SetActive(false);
        }

        protected override void OnOpened()
        {
            _inventory.OnChanged += RefreshButtons;
            PopulateRecipeList();
        }

        protected override void OnClosed()
        {
            if (_inventory != null)
                _inventory.OnChanged -= RefreshButtons;
        }

        #endregion

        #region IInitializable

        public void Initialize(Inventory inventory)
        {
            _inventory = inventory;
        }

        #endregion

        #region Private Methods

        private void PopulateRecipeList()
        {
            foreach (Transform child in _recipeListRoot)
            {
                if (child.gameObject != _recipeButtonTemplate.gameObject)
                    Destroy(child.gameObject);
            }
            _recipeButtons.Clear();

            var allRecipes = InGameDataManager.Instance.GetAll<RecipeData>();
            if (allRecipes == null) return;

            foreach (var recipe in allRecipes)
            {
                if (recipe.workbench_type != 0) continue;

                var btn = Instantiate(_recipeButtonTemplate, _recipeListRoot);
                btn.gameObject.SetActive(true);

                var tmp = btn.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (tmp != null)
                    tmp.text = BuildRecipeText(recipe);

                var captured = recipe;
                btn.onClick.AddListener(() => OnClickCraft(captured));
                _recipeButtons.Add((btn, recipe));
            }

            RefreshButtons();
        }

        private void RefreshButtons()
        {
            foreach (var (btn, recipe) in _recipeButtons)
                btn.interactable = CraftingSystem.CanCraft(recipe, _inventory);
        }

        private void OnClickCraft(RecipeData recipe)
        {
            bool crafted = CraftingSystem.TryCraft(recipe, _inventory);
            if (crafted)
                ToastManager.Instance.Show($"{recipe.name} crafted!", ToastType.Success);
        }

        private string BuildRecipeText(RecipeData recipe)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append($"{recipe.name} x{recipe.result_count}\n");

            foreach (var ing in recipe.ingredients)
            {
                var itemData = InGameDataManager.Instance.Get<SurvivalItemData>(ing.item_id);
                string itemName = itemData != null ? itemData.name : $"Item{ing.item_id}";
                int have = _inventory.GetCount(ing.item_id);
                sb.Append($"{itemName} {have}/{ing.count}  ");
            }

            return sb.ToString().TrimEnd();
        }

        #endregion
    }
}
