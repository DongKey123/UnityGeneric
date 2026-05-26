using System.Collections.Generic;
using Framework.UI;
using SurvivalGame.Building;
using SurvivalGame.Data;
using SurvivalGame.Inventories;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivalGame.UI
{
    /// <summary>
    /// 건물 관리 팝업입니다.
    /// 배치 대기 / 설치됨 목록을 표시하고 배치·버리기·이동·수리·철거 액션을 제공합니다.
    /// </summary>
    public class BuildingQueuePanel : UIPanel, IInitializable<Inventory>
    {
        #region Inspector — Popup Chrome

        [Header("Popup Chrome")]
        [SerializeField] private TextMeshProUGUI _popupTitleText;
        [SerializeField] private TextMeshProUGUI _popupStateTagText;
        [SerializeField] private Button          _closeButton;

        #endregion

        #region Inspector — List

        [Header("List")]
        [SerializeField] private TextMeshProUGUI   _totalCountText;
        [SerializeField] private GameObject        _pendingSection;
        [SerializeField] private TextMeshProUGUI   _pendingCountText;
        [SerializeField] private Transform         _pendingCardsRoot;
        [SerializeField] private GameObject        _placedSection;
        [SerializeField] private TextMeshProUGUI   _placedCountText;
        [SerializeField] private Transform         _placedCardsRoot;
        [SerializeField] private GameObject        _listEmptyHint;
        [SerializeField] private BuildingCardElement _cardPrefab;

        #endregion

        #region Inspector — Detail Common

        [Header("Detail — Common")]
        [SerializeField] private GameObject      _detailEmpty;
        [SerializeField] private GameObject      _detailBody;
        [SerializeField] private Image           _heroIconBg;
        [SerializeField] private Image           _heroIcon;
        [SerializeField] private Image           _heroCheckOverlay;
        [SerializeField] private TextMeshProUGUI _heroNameText;
        [SerializeField] private Image           _categoryTagBg;
        [SerializeField] private TextMeshProUGUI _categoryTagText;
        [SerializeField] private Image           _gridTagBg;
        [SerializeField] private TextMeshProUGUI _gridTagText;

        #endregion

        #region Inspector — Detail Info Cells

        [Header("Detail — Info Cells")]
        [SerializeField] private TextMeshProUGUI _infoCellLabelA;
        [SerializeField] private TextMeshProUGUI _infoCellValueA;
        [SerializeField] private TextMeshProUGUI _infoCellLabelB;
        [SerializeField] private TextMeshProUGUI _infoCellValueB;

        #endregion

        #region Inspector — Detail Mat Section

        [Header("Detail — Materials (Pending)")]
        [SerializeField] private GameObject      _matSection;
        [SerializeField] private TextMeshProUGUI _matSectionLabel;
        [SerializeField] private Transform       _matListRoot;

        #endregion

        #region Inspector — Detail Durability

        [Header("Detail — Durability (Placed)")]
        [SerializeField] private GameObject      _durabilitySection;
        [SerializeField] private Image           _durabilityFill;
        [SerializeField] private TextMeshProUGUI _durabilityText;

        #endregion

        #region Inspector — Detail Info Box

        [Header("Detail — Info Box")]
        [SerializeField] private Image           _infoBoxBg;
        [SerializeField] private TextMeshProUGUI _infoBoxText;

        #endregion

        #region Inspector — Actions Pending

        [Header("Actions — Pending")]
        [SerializeField] private GameObject      _actionsPending;
        [SerializeField] private Button          _btnPlace;
        [SerializeField] private Button          _btnDiscard;
        [SerializeField] private TextMeshProUGUI _subTextPending;

        #endregion

        #region Inspector — Actions Placed

        [Header("Actions — Placed")]
        [SerializeField] private GameObject      _actionsPlaced;
        [SerializeField] private Button          _btnMove;
        [SerializeField] private Button          _btnRepair;
        [SerializeField] private Image           _btnRepairImage;
        [SerializeField] private Button          _btnDemolish;
        [SerializeField] private TextMeshProUGUI _subTextPlaced;

        #endregion

        #region Inspector — State Sprites

        [Header("State Sprites — Category Tags")]
        [SerializeField] private Sprite _sprTagStructure;
        [SerializeField] private Sprite _sprTagFurniture;
        [SerializeField] private Sprite _sprTagProduction;
        [SerializeField] private Sprite _sprTagGrid;

        [Header("State Sprites — Info Box")]
        [SerializeField] private Sprite _sprInfoBoxPending;
        [SerializeField] private Sprite _sprInfoBoxPlaced;

        [Header("State Sprites — Durability Fill")]
        [SerializeField] private Sprite _sprDurFillHigh;
        [SerializeField] private Sprite _sprDurFillMid;
        [SerializeField] private Sprite _sprDurFillLow;

        [Header("State Sprites — Buttons")]
        [SerializeField] private Sprite _sprBtnRepairActive;
        [SerializeField] private Sprite _sprBtnDisabled;

        #endregion

        #region Private Fields

        private Inventory                          _inventory;
        private BuildingQueueEntry                 _selectedEntry;
        private readonly List<BuildingCardElement> _activeCards = new();

        #endregion

        #region UIPanel Lifecycle

        protected override void Awake()
        {
            base.Awake();
            _closeButton.onClick.AddListener(() => UIManager.Instance.Close());
            _btnPlace.onClick.AddListener(OnClickPlace);
            _btnDiscard.onClick.AddListener(OnClickDiscard);
            _btnMove.onClick.AddListener(OnClickMove);
            _btnRepair.onClick.AddListener(OnClickRepair);
            _btnDemolish.onClick.AddListener(OnClickDemolish);
        }

        protected override void OnOpened()
        {
            if (BuildingQueueManager.Instance != null)
                BuildingQueueManager.Instance.OnQueueChanged += RefreshList;
            RefreshList();
            ClearSelection();
        }

        protected override void OnClosed()
        {
            if (BuildingQueueManager.Instance != null)
                BuildingQueueManager.Instance.OnQueueChanged -= RefreshList;
        }

        #endregion

        #region IInitializable

        public void Initialize(Inventory inventory) => _inventory = inventory;

        #endregion

        #region List

        private void RefreshList()
        {
            foreach (var card in _activeCards)
                if (card != null) Destroy(card.gameObject);
            _activeCards.Clear();

            if (_cardPrefab == null || BuildingQueueManager.Instance == null) return;

            var all     = BuildingQueueManager.Instance.GetAll();
            int pending = 0, placed = 0;

            foreach (var entry in all)
            {
                var root = entry.State == BuildingState.Pending ? _pendingCardsRoot : _placedCardsRoot;
                var card = Instantiate(_cardPrefab, root);
                card.Setup(entry);
                card.OnClicked += OnCardClicked;
                _activeCards.Add(card);
                if (entry.State == BuildingState.Pending) pending++; else placed++;
            }

            bool hasAny = all.Count > 0;
            if (_listEmptyHint   != null) _listEmptyHint.SetActive(!hasAny);
            if (_pendingSection  != null) _pendingSection.SetActive(pending > 0);
            if (_placedSection   != null) _placedSection.SetActive(placed  > 0);
            if (_pendingCountText != null) _pendingCountText.text = $"{pending}건";
            if (_placedCountText  != null) _placedCountText.text  = $"{placed}건";
            if (_totalCountText   != null) _totalCountText.text   = $"총 {all.Count}건";
        }

        #endregion

        #region Selection

        private void OnCardClicked(BuildingCardElement card)
        {
            if (_selectedEntry != null)
            {
                foreach (var c in _activeCards)
                    if (c.BoundEntry == _selectedEntry) c.SetSelected(false);
            }

            _selectedEntry = card.BoundEntry;
            card.SetSelected(true);
            ShowDetail(_selectedEntry);
        }

        private void ClearSelection()
        {
            _selectedEntry = null;
            foreach (var c in _activeCards) c.SetSelected(false);
            if (_detailEmpty != null) _detailEmpty.SetActive(true);
            if (_detailBody  != null) _detailBody.SetActive(false);
        }

        #endregion

        #region Detail

        private void ShowDetail(BuildingQueueEntry entry)
        {
            if (_detailEmpty != null) _detailEmpty.SetActive(false);
            if (_detailBody  != null) _detailBody.SetActive(true);

            var data   = entry.Data;
            bool placed = entry.State == BuildingState.Placed;

            if (_heroNameText != null) _heroNameText.text = data.name;
            if (_heroCheckOverlay != null) _heroCheckOverlay.gameObject.SetActive(placed);

            // 카테고리 태그
            SetCategoryTag(data.category);

            // 그리드 태그
            if (_gridTagText != null) _gridTagText.text = $"{data.grid_width} × {data.grid_height}";

            // 인포 셀
            if (placed)
            {
                var p = entry.PlacedInstance;
                if (_infoCellLabelA != null) _infoCellLabelA.text = "설치 위치";
                if (_infoCellValueA != null) _infoCellValueA.text = p != null ? "—" : "—";
                if (_infoCellLabelB != null) _infoCellLabelB.text = "레벨";
                if (_infoCellValueB != null) _infoCellValueB.text = "Lv. 1";
            }
            else
            {
                if (_infoCellLabelA != null) _infoCellLabelA.text = "그리드 크기";
                if (_infoCellValueA != null) _infoCellValueA.text = $"{data.grid_width} × {data.grid_height}";
                if (_infoCellLabelB != null) _infoCellLabelB.text = "상태";
                if (_infoCellValueB != null) _infoCellValueB.text = "배치 대기 중";
            }

            // 재료 섹션 (Pending)
            if (_matSection != null) _matSection.SetActive(!placed);

            // 내구도 섹션 (Placed)
            if (_durabilitySection != null) _durabilitySection.SetActive(placed);
            if (placed && data.durability_max > 0)
            {
                float ratio = 1f; // TODO: PlacedBuilding.CurrentDurability / durability_max
                if (_durabilityFill != null)
                {
                    _durabilityFill.sprite     = ratio > 0.66f ? _sprDurFillHigh
                                               : ratio > 0.33f ? _sprDurFillMid : _sprDurFillLow;
                    _durabilityFill.fillAmount = ratio;
                }
                if (_durabilityText != null) _durabilityText.text = $"{data.durability_max} / {data.durability_max}";
            }

            // 인포 박스
            if (_infoBoxBg   != null) _infoBoxBg.sprite = placed ? _sprInfoBoxPlaced : _sprInfoBoxPending;
            if (_infoBoxText != null) _infoBoxText.text = placed
                ? "설치된 건물입니다. 이동·수리·철거가 가능합니다."
                : "다음 단계: 배치하기를 누르면 맵에서 위치를 선택할 수 있습니다.";

            // 액션 버튼
            if (_actionsPending != null) _actionsPending.SetActive(!placed);
            if (_actionsPlaced  != null) _actionsPlaced.SetActive(placed);

            if (placed && _btnRepair != null && _btnRepairImage != null)
            {
                bool canRepair = true; // TODO: ratio < 1f
                _btnRepair.interactable  = canRepair;
                _btnRepairImage.sprite   = canRepair ? _sprBtnRepairActive : _sprBtnDisabled;
            }

            if (_subTextPending != null) _subTextPending.text = "버리기 시 재료 일부를 돌려받습니다";
            if (_subTextPlaced  != null) _subTextPlaced.text  = "철거 시 재료 50%를 돌려받습니다";

            // 상태 태그 업데이트
            if (_popupStateTagText != null)
                _popupStateTagText.text = placed ? "건물 · 설치됨 선" : "건물 · 배치 대기 선";
        }

        private void SetCategoryTag(string category)
        {
            if (_categoryTagBg == null) return;
            _categoryTagBg.sprite = category switch
            {
                "Structure"  => _sprTagStructure,
                "Furniture"  => _sprTagFurniture,
                "Production" => _sprTagProduction,
                _            => _sprTagStructure,
            };
            if (_categoryTagText != null)
                _categoryTagText.text = category switch
                {
                    "Structure"  => "구조물",
                    "Furniture"  => "가구",
                    "Production" => "생산",
                    _            => category,
                };
        }

        #endregion

        #region Action Handlers (stub — 로직은 추후 연결)

        private void OnClickPlace()    { /* TODO: BuildingPlacer.EnterPlacementMode + Close */ }
        private void OnClickDiscard()  { /* TODO: 재료 반환 + BuildingQueueManager.Remove */ }
        private void OnClickMove()     { /* TODO: BuildingPlacer로 이동 모드 */ }
        private void OnClickRepair()   { /* TODO: 재료 소모 + 내구도 회복 */ }
        private void OnClickDemolish() { /* TODO: PlacedBuilding 제거 + 재료 50% 반환 */ }

        #endregion
    }
}
