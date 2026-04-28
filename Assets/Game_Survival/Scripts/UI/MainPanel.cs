using Framework.Core.EventBus;
using Framework.UI;
using SurvivalGame.Farming;
using SurvivalGame.Inventories;
using SurvivalGame.Player;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivalGame.UI
{
    /// <summary>
    /// 인게임 메인 HUD 패널입니다.
    /// 항상 표시되며 조이스틱, 인벤토리 열기 버튼, 채집 버튼 등 HUD 요소를 포함합니다.
    /// SurvivalEntry에서 UIManager.Instance.Open&lt;MainPanel, PlayerController&gt;(player)로 열어주세요.
    /// </summary>
    public class MainPanel : UIPanel, IInitializable<PlayerController>
    {
        #region Inspector

        [SerializeField] private Button _inventoryButton;
        [SerializeField] private Button _harvestButton;
        [SerializeField] private Button _attackButton;

        #endregion

        #region Private Fields

        private PlayerController _player;
        private ResourceObject   _currentResource;

        #endregion

        /// <summary>HUD는 닫히면 안 됩니다.</summary>
        public override bool CanClose    => false;

        /// <summary>뒤로가기로 닫히지 않습니다.</summary>
        public override bool CloseOnBack => false;

        #region UIPanel Lifecycle

        protected override void Awake()
        {
            base.Awake();
            _inventoryButton.onClick.AddListener(OnClickInventory);
            _harvestButton.onClick.AddListener(OnClickHarvest);
            _attackButton.onClick.AddListener(OnClickAttack);
        }

        private void Update()
        {
            if (_player == null) return;
            _attackButton.gameObject.SetActive(_player.GetNearestEnemy() != null);
        }

        protected override void OnOpened()
        {
            EventBus.Subscribe<HarvestRangeEnteredEvent>(OnHarvestRangeEntered);
            EventBus.Subscribe<HarvestRangeExitedEvent>(OnHarvestRangeExited);
            EventBus.Subscribe<ResourceHarvestedEvent>(OnResourceHarvested);
            SetHarvestButtonActive(false);
        }

        protected override void OnClosed()
        {
            EventBus.Unsubscribe<HarvestRangeEnteredEvent>(OnHarvestRangeEntered);
            EventBus.Unsubscribe<HarvestRangeExitedEvent>(OnHarvestRangeExited);
            EventBus.Unsubscribe<ResourceHarvestedEvent>(OnResourceHarvested);
        }

        #endregion

        #region IInitializable

        /// <summary>열릴 때 플레이어를 주입받습니다.</summary>
        public void Initialize(PlayerController player)
        {
            _player = player;
        }

        #endregion

        #region Private Methods

        private void OnClickInventory()
        {
            if (UIManager.Instance.IsOpen<InventoryPanel>())
                UIManager.Instance.Close();
            else
                UIManager.Instance.Open<InventoryPanel, Inventory>(_player.Inventory);
        }

        private void OnClickHarvest()
        {
            _currentResource?.Harvest();
        }

        private void OnClickAttack()
        {
            if (_player != null) _player.Attack();
        }

        private void OnHarvestRangeEntered(HarvestRangeEnteredEvent e)
        {
            _currentResource = e.Resource;
            SetHarvestButtonActive(true);
        }

        private void OnHarvestRangeExited(HarvestRangeExitedEvent e)
        {
            if (_currentResource != e.Resource) return;
            _currentResource = null;
            SetHarvestButtonActive(false);
        }

        private void OnResourceHarvested(ResourceHarvestedEvent e)
        {
            ToastManager.Instance.Show($"{e.ItemName} x{e.Count} obtained", ToastType.Success);
        }

        private void SetHarvestButtonActive(bool active)
        {
            _harvestButton.gameObject.SetActive(active);
        }

        #endregion
    }
}
