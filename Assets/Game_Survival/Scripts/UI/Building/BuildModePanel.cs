using Framework.Core.DataManager;
using Framework.UI;
using SurvivalGame.Building;
using SurvivalGame.Data;
using SurvivalGame.Inventories;
using SurvivalGame.Player;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivalGame.UI
{
    /// <summary>
    /// 빌드 모드 패널입니다.
    /// 배치 가능한 건물 목록을 보여주고, 선택 시 BuildingPlacer로 배치 모드를 시작합니다.
    /// 배치 모드 중에는 Place / Cancel 버튼을 표시합니다.
    /// </summary>
    public class BuildModePanel : UIPanel, IInitializable<PlayerController>
    {
        #region Inspector

        [Header("선택 화면")]
        [SerializeField] private GameObject  _selectionView;
        [SerializeField] private Transform   _buildingListRoot;
        [SerializeField] private Button      _buildingButtonTemplate;

        [Header("배치 화면")]
        [SerializeField] private GameObject  _placementView;
        [SerializeField] private Button      _placeButton;
        [SerializeField] private Button      _cancelButton;

        #endregion

        #region Private Fields

        private PlayerController _player;

        #endregion

        #region UIPanel Lifecycle

        protected override void Awake()
        {
            base.Awake();
            _placeButton.onClick.AddListener(OnClickPlace);
            _cancelButton.onClick.AddListener(OnClickCancel);
            _buildingButtonTemplate.gameObject.SetActive(false);
        }

        protected override void OnOpened()
        {
            ShowSelectionView();
            PopulateBuildingList();
        }

        protected override void OnClosed()
        {
            if (BuildingPlacer.Instance.IsPlacing)
                BuildingPlacer.Instance.ExitPlacementMode();
        }

        private void Update()
        {
            if (!BuildingPlacer.Instance.IsPlacing) return;

            // 배치 가능 여부에 따라 Place 버튼 색상 변경
            bool canPlace = BuildingPlacer.Instance.CanPlaceAtCurrentPosition();
            _placeButton.interactable = canPlace;
        }

        #endregion

        #region IInitializable

        public void Initialize(PlayerController player)
        {
            _player = player;
        }

        #endregion

        #region Private Methods

        private void PopulateBuildingList()
        {
            // 기존 버튼 제거 (템플릿 제외)
            foreach (Transform child in _buildingListRoot)
            {
                if (child.gameObject != _buildingButtonTemplate.gameObject)
                    Destroy(child.gameObject);
            }

            var allBuildings = InGameDataManager.Instance.GetAll<BuildingData>();
            if (allBuildings == null) return;

            foreach (var data in allBuildings)
            {
                var btn = Instantiate(_buildingButtonTemplate, _buildingListRoot);
                btn.gameObject.SetActive(true);

                // 버튼 텍스트 설정
                var tmp = btn.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (tmp != null)
                {
                    string costText = BuildCostText(data);
                    tmp.text = $"{data.name}\n{costText}";
                }

                var captured = data;
                btn.onClick.AddListener(() => OnClickBuilding(captured));
            }
        }

        private string BuildCostText(BuildingData data)
        {
            if (data.costs == null || data.costs.Count == 0) return "";

            var sb = new System.Text.StringBuilder();
            foreach (var cost in data.costs)
            {
                var itemData = InGameDataManager.Instance.Get<SurvivalItemData>(cost.item_id);
                string itemName = itemData != null ? itemData.name : $"Item{cost.item_id}";
                sb.Append($"{itemName} x{cost.count}  ");
            }
            return sb.ToString().TrimEnd();
        }

        private void OnClickBuilding(BuildingData data)
        {
            BuildingPlacer.Instance.EnterPlacementMode(data, _player, _player.Inventory);
            ShowPlacementView();
        }

        private void OnClickPlace()
        {
            bool placed = BuildingPlacer.Instance.TryPlace();
            if (placed)
                ShowSelectionView();
        }

        private void OnClickCancel()
        {
            if (BuildingPlacer.Instance.IsPlacing)
            {
                BuildingPlacer.Instance.ExitPlacementMode();
                ShowSelectionView();
            }
            else
            {
                UIManager.Instance.Close();
            }
        }

        private void ShowSelectionView()
        {
            _selectionView.SetActive(true);
            _placementView.SetActive(false);
        }

        private void ShowPlacementView()
        {
            _selectionView.SetActive(false);
            _placementView.SetActive(true);
        }

        #endregion
    }
}
