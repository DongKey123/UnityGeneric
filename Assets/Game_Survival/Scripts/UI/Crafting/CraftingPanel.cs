using System.Collections.Generic;
using Framework.Core.DataManager;
using Framework.UI;
using SurvivalGame.Crafting;
using SurvivalGame.Data;
using SurvivalGame.Inventories;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivalGame.UI
{
    /// <summary>
    /// 크래프팅 &amp; 빌딩 탭 패널입니다.
    /// 아이템 제작 탭과 빌딩 탭을 포함하며, 레시피/건물 목록·상세·액션 버튼을 관리합니다.
    /// </summary>
    public class CraftingPanel : UIPanel, IInitializable<Inventory>
    {
        #region Inspector — Popup Chrome

        [Header("Popup Chrome")]
        [SerializeField] private TextMeshProUGUI _popupTitleText;
        [SerializeField] private TextMeshProUGUI _popupStateTagText;
        [SerializeField] private Button          _closeButton;

        #endregion

        #region Inspector — Tabs

        [Header("Tabs")]
        [SerializeField] private Button _btnTabBuild;
        [SerializeField] private Button _btnTabCraft;
        [SerializeField] private Image  _tabBuildImage;
        [SerializeField] private Image  _tabCraftImage;

        #endregion

        #region Inspector — Resource Bar

        [Header("Resource Bar")]
        [SerializeField] private Image           _resChipWood;
        [SerializeField] private Image           _resChipStone;
        [SerializeField] private Image           _resChipScrap;
        [SerializeField] private TextMeshProUGUI _resTxtWood;
        [SerializeField] private TextMeshProUGUI _resTxtStone;
        [SerializeField] private TextMeshProUGUI _resTxtScrap;
        [SerializeField] private TextMeshProUGUI _resTxtGem;

        #endregion

        #region Inspector — List

        [Header("List")]
        [SerializeField] private Button[]            _filterChips;
        [SerializeField] private Transform           _cardListRoot;
        [SerializeField] private CraftingCardElement _cardElementPrefab;

        // 레거시 필드 — 구버전 prefab 호환용, 실제로 사용하지 않음
        [SerializeField] private Transform _recipeListRoot;
        #endregion

        #region Inspector — Detail Header

        [Header("Detail Header")]
        [SerializeField] private GameObject      _emptyHint;
        [SerializeField] private GameObject      _detailBody;
        [SerializeField] private Image           _detailIcon;
        [SerializeField] private TextMeshProUGUI _detailNameText;
        [SerializeField] private Image           _detailTagBg;
        [SerializeField] private TextMeshProUGUI _detailTagText;

        #endregion

        #region Inspector — Detail Info

        [Header("Detail Info")]
        [SerializeField] private TextMeshProUGUI _infoWeightText;
        [SerializeField] private TextMeshProUGUI _infoDurabilityText;
        [SerializeField] private TextMeshProUGUI _infoTierText;
        [SerializeField] private Image           _effectBoxBg;
        [SerializeField] private TextMeshProUGUI _effectBoxText;
        [SerializeField] private TextMeshProUGUI _descText;
        [SerializeField] private Transform       _ingredientsRoot;
        [SerializeField] private GameObject      _levelReqBox;
        [SerializeField] private Image           _levelReqBg;
        [SerializeField] private TextMeshProUGUI _levelReqText;

        #endregion

        #region Inspector — Progress

        [Header("Progress")]
        [SerializeField] private GameObject      _progressArea;
        [SerializeField] private Image           _progressFill;
        [SerializeField] private TextMeshProUGUI _progressPercentText;
        [SerializeField] private TextMeshProUGUI _progressTimerText;

        #endregion

        #region Inspector — Action Buttons

        [Header("Prefabs")]
        [SerializeField] private IngredientChipElement _detailChipPrefab;
        [SerializeField] private IngredientRowElement  _detailIngRowPrefab;

        [Header("Action Buttons")]
        [SerializeField] private Button          _btnAction;
        [SerializeField] private Image           _btnActionImage;
        [SerializeField] private TextMeshProUGUI _btnActionText;
        [SerializeField] private Button          _btnSpeed;
        [SerializeField] private Button          _btnStop;
        [SerializeField] private Button          _btnCancel;

        #endregion

        #region Inspector — State Sprites

        [Header("State Sprites — Tab")]
        [SerializeField] private Sprite _tabActiveSprite;
        [SerializeField] private Sprite _tabInactiveSprite;

        [Header("State Sprites — Buttons")]
        [SerializeField] private Sprite _btnCraftSprite;
        [SerializeField] private Sprite _btnBuildSprite;
        [SerializeField] private Sprite _btnDisabledSprite;

        [Header("State Sprites — Detail")]
        [SerializeField] private Sprite _effectBoxOkSprite;
        [SerializeField] private Sprite _effectBoxNgSprite;
        [SerializeField] private Sprite _effectBoxBuildSprite;
        [SerializeField] private Sprite _levelReqOkSprite;
        [SerializeField] private Sprite _levelReqNgSprite;
        [SerializeField] private Sprite _progressFillCraftSprite;
        [SerializeField] private Sprite _progressFillBuildSprite;

        #endregion

        #region Private Fields

        private enum TabMode { Craft, Build }

        private Inventory                          _inventory;
        private TabMode                            _currentTab = TabMode.Craft;
        private readonly List<CraftingCardElement> _activeCards = new();
        private CraftingCardElement                _selectedCard;

        #endregion

        #region UIPanel Lifecycle

        protected override void Awake()
        {
            base.Awake();
            _closeButton.onClick.AddListener(() => UIManager.Instance.Close());
            _btnTabCraft.onClick.AddListener(() => SwitchTab(TabMode.Craft));
            _btnTabBuild.onClick.AddListener(() => SwitchTab(TabMode.Build));
            _btnAction.onClick.AddListener(OnClickAction);
        }

        protected override void OnOpened()
        {
            _inventory.OnChanged += OnInventoryChanged;
            SwitchTab(TabMode.Craft);
        }

        protected override void OnClosed()
        {
            if (_inventory != null)
                _inventory.OnChanged -= OnInventoryChanged;
        }

        #endregion

        #region IInitializable

        public void Initialize(Inventory inventory)
        {
            _inventory = inventory;
        }

        #endregion

        #region Tab

        private void SwitchTab(TabMode tab)
        {
            _currentTab = tab;
            bool isCraft = tab == TabMode.Craft;

            if (_tabCraftImage != null)
                _tabCraftImage.sprite = isCraft ? _tabActiveSprite : _tabInactiveSprite;
            if (_tabBuildImage != null)
                _tabBuildImage.sprite = isCraft ? _tabInactiveSprite : _tabActiveSprite;

            if (_popupTitleText != null)
                _popupTitleText.text = isCraft ? "CRAFTING" : "BUILDING";
            if (_popupStateTagText != null)
                _popupStateTagText.text = isCraft ? "제작" : "빌딩";

            ClearSelection();
            PopulateList();
        }

        #endregion

        #region List

        private void PopulateList()
        {
            foreach (var card in _activeCards)
                if (card != null) Destroy(card.gameObject);
            _activeCards.Clear();

            if (_cardElementPrefab == null || _cardListRoot == null) return;

            if (_currentTab == TabMode.Craft)
            {
                var recipes = InGameDataManager.Instance.GetAll<RecipeData>();
                if (recipes == null) return;

                foreach (var recipe in recipes)
                {
                    if (recipe.workbench_type != 0) continue;
                    var card = Instantiate(_cardElementPrefab, _cardListRoot);
                    card.SetupRecipe(recipe, _inventory);
                    card.OnClicked += OnCardClicked;
                    _activeCards.Add(card);
                }
            }
            else
            {
                var buildings = InGameDataManager.Instance.GetAll<BuildingData>();
                if (buildings == null) return;

                foreach (var building in buildings)
                {
                    var card = Instantiate(_cardElementPrefab, _cardListRoot);
                    card.SetupBuilding(building, _inventory);
                    card.OnClicked += OnCardClicked;
                    _activeCards.Add(card);
                }
            }
        }

        #endregion

        #region Selection

        private void OnCardClicked(CraftingCardElement card)
        {
            if (_selectedCard != null)
                _selectedCard.SetSelected(false);

            _selectedCard = card;
            card.SetSelected(true);

            if (card.BoundRecipe != null)
                ShowRecipeDetail(card.BoundRecipe);
            else if (card.BoundBuilding != null)
                ShowBuildingDetail(card.BoundBuilding);
        }

        private void ClearSelection()
        {
            if (_selectedCard != null)
            {
                _selectedCard.SetSelected(false);
                _selectedCard = null;
            }
            ShowEmptyHint();
        }

        private void ShowEmptyHint()
        {
            if (_emptyHint   != null) _emptyHint.SetActive(true);
            if (_detailBody  != null) _detailBody.SetActive(false);
            if (_progressArea != null) _progressArea.SetActive(false);
            if (_btnAction   != null) _btnAction.gameObject.SetActive(false);
            if (_btnSpeed    != null) _btnSpeed.gameObject.SetActive(false);
            if (_btnStop     != null) _btnStop.gameObject.SetActive(false);
        }

        #endregion

        #region Detail

        private void ShowRecipeDetail(RecipeData recipe)
        {
            if (_emptyHint    != null) _emptyHint.SetActive(false);
            if (_detailBody   != null) _detailBody.SetActive(true);
            if (_progressArea != null) _progressArea.SetActive(false);

            _detailNameText.text = recipe.name;

            var resultItem = InGameDataManager.Instance.Get<SurvivalItemData>(recipe.result_item_id);
            if (resultItem != null && !string.IsNullOrEmpty(resultItem.icon_path))
            {
                var sp = Resources.Load<Sprite>(resultItem.icon_path);
                _detailIcon.sprite  = sp;
                _detailIcon.enabled = sp != null;
            }
            else
            {
                _detailIcon.enabled = false;
            }

            if (_infoWeightText     != null) _infoWeightText.text     = "-";
            if (_infoDurabilityText != null) _infoDurabilityText.text = "-";
            if (_infoTierText       != null) _infoTierText.text       = "-";
            if (_descText           != null) _descText.text           = "";
            if (_detailTagText      != null) _detailTagText.text      = "제작";
            if (_effectBoxBg        != null) _effectBoxBg.gameObject.SetActive(false);
            if (_levelReqBox        != null) _levelReqBox.SetActive(false);

            RebuildDetailIngredients(recipe.ingredients, null);
            RefreshDetailActionButton();
        }

        private void ShowBuildingDetail(BuildingData building)
        {
            if (_emptyHint    != null) _emptyHint.SetActive(false);
            if (_detailBody   != null) _detailBody.SetActive(true);
            if (_progressArea != null) _progressArea.SetActive(false);

            _detailNameText.text = building.name;
            _detailIcon.enabled  = false;

            if (_infoWeightText     != null) _infoWeightText.text     = "-";
            if (_infoDurabilityText != null) _infoDurabilityText.text = "-";
            if (_infoTierText       != null) _infoTierText.text       = $"{building.grid_width}×{building.grid_height}";
            if (_descText           != null) _descText.text           = "";
            if (_detailTagText      != null) _detailTagText.text      = "건설";
            if (_effectBoxBg        != null) _effectBoxBg.gameObject.SetActive(false);
            if (_levelReqBox        != null) _levelReqBox.SetActive(false);

            RebuildDetailIngredients(null, building.costs);
            RefreshDetailActionButton();
        }

        private void RebuildDetailIngredients(List<RecipeIngredient> recipeIngs, List<BuildingCost> buildCosts)
        {
            if (_ingredientsRoot == null) return;
            foreach (Transform child in _ingredientsRoot)
                Destroy(child.gameObject);

            if (_detailChipPrefab == null) return;

            var items = new List<(int id, int need)>();
            if (recipeIngs != null) foreach (var i in recipeIngs) items.Add((i.item_id, i.count));
            if (buildCosts != null) foreach (var c in buildCosts) items.Add((c.item_id, c.count));

            foreach (var (id, need) in items)
            {
                int have = _inventory != null ? _inventory.GetCount(id) : 0;
                var itemData = InGameDataManager.Instance.Get<SurvivalItemData>(id);
                string itemName = itemData != null ? itemData.name : $"#{id}";

                var chip = Instantiate(_detailChipPrefab, _ingredientsRoot, false);
                chip.Setup(itemName, have, need, 16f);
            }
        }

        #endregion

        #region Action Button

        private void RefreshDetailActionButton()
        {
            if (_selectedCard == null)
            {
                if (_btnAction != null) _btnAction.gameObject.SetActive(false);
                if (_btnSpeed  != null) _btnSpeed.gameObject.SetActive(false);
                if (_btnStop   != null) _btnStop.gameObject.SetActive(false);
                return;
            }

            _btnAction.gameObject.SetActive(true);
            if (_btnSpeed != null) _btnSpeed.gameObject.SetActive(false);
            if (_btnStop  != null) _btnStop.gameObject.SetActive(false);

            bool canMake = _selectedCard.CanMake(_inventory);

            if (_btnActionImage != null)
                _btnActionImage.sprite = canMake
                    ? (_currentTab == TabMode.Craft ? _btnCraftSprite : _btnBuildSprite)
                    : _btnDisabledSprite;

            if (_btnActionText != null)
                _btnActionText.text = canMake
                    ? (_currentTab == TabMode.Craft ? "제작" : "건설")
                    : "재료 부족";

            _btnAction.interactable = canMake;
        }

        private void OnClickAction()
        {
            if (_selectedCard == null) return;

            if (_selectedCard.BoundRecipe != null)
            {
                bool crafted = CraftingSystem.TryCraft(_selectedCard.BoundRecipe, _inventory);
                if (crafted)
                    ToastManager.Instance.Show($"{_selectedCard.BoundRecipe.name} 제작 완료!", ToastType.Success);
            }
            else if (_selectedCard.BoundBuilding != null)
            {
                ToastManager.Instance.Show($"{_selectedCard.BoundBuilding.name} 건설 배치 준비 중", ToastType.Success);
            }
        }

        #endregion

        #region Inventory Change

        private void OnInventoryChanged()
        {
            foreach (var card in _activeCards)
                card.RefreshState(_inventory);

            if (_selectedCard == null) return;

            RefreshDetailActionButton();
            if (_selectedCard.BoundRecipe != null)
                RebuildDetailIngredients(_selectedCard.BoundRecipe.ingredients, null);
            else if (_selectedCard.BoundBuilding != null)
                RebuildDetailIngredients(null, _selectedCard.BoundBuilding.costs);
        }

        #endregion
    }
}
