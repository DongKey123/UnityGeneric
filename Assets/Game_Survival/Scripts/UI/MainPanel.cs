using Framework.UI;
using SurvivalGame.Inventories;
using SurvivalGame.Player;
using UnityEngine.UI;

namespace SurvivalGame.UI
{
    /// <summary>
    /// 인게임 메인 HUD 패널입니다.
    /// 항상 표시되며 조이스틱, 인벤토리 열기 버튼 등 HUD 요소를 포함합니다.
    /// SurvivalEntry에서 UIManager.Instance.ShowOverlay&lt;MainPanel, PlayerController&gt;(player)로 열어주세요.
    /// </summary>
    public class MainPanel : UIPanel, IInitializable<PlayerController>
    {
        #region Inspector

        [UnityEngine.SerializeField] private Button _inventoryButton;

        #endregion

        #region Private Fields

        private PlayerController _player;

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

        #endregion
    }
}
