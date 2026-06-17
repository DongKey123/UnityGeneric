using Framework.UI;
using SurvivalGame.Building;
using SurvivalGame.Inventories;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivalGame.UI
{
    public class BuildModeOverlayData
    {
        public BuildingQueueEntry Entry     { get; set; }
        public Inventory          Inventory { get; set; }
    }

    /// <summary>
    /// 배치 모드 중 화면 하단에 표시되는 오버레이 패널입니다.
    /// UIManager.ShowOverlay&lt;BuildModeOverlay, BuildModeOverlayData&gt;로 열고
    /// UIManager.HideOverlay&lt;BuildModeOverlay&gt;로 닫습니다.
    /// </summary>
    public class BuildModeOverlay : UIPanel, IInitializable<BuildModeOverlayData>
    {
        #region Inspector — Building Info

        [Header("Building Info")]
        [SerializeField] private Image           _iconSlot;
        [SerializeField] private Image           _icon;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _gridText;
        [SerializeField] private TextMeshProUGUI _coordText;

        #endregion

        #region Inspector — Status

        [Header("Status Bar")]
        [SerializeField] private Image           _statusBg;
        [SerializeField] private Image           _statusIcon;
        [SerializeField] private TextMeshProUGUI _statusText;

        #endregion

        #region Inspector — Buttons

        [Header("Buttons")]
        [SerializeField] private Button          _cancelButton;
        [SerializeField] private Button          _confirmButton;
        [SerializeField] private Image           _confirmButtonImage;

        #endregion

        #region Inspector — State Sprites

        [Header("State Sprites")]
        [SerializeField] private Sprite _sprStatusOkBg;
        [SerializeField] private Sprite _sprStatusNgBg;
        [SerializeField] private Sprite _sprStatusOkIcon;
        [SerializeField] private Sprite _sprStatusNgIcon;
        [SerializeField] private Sprite _sprBtnConfirmActive;
        [SerializeField] private Sprite _sprBtnConfirmDisabled;

        #endregion

        #region Private Fields

        private Inventory _currentInventory;

        #endregion

        public override bool CloseOnBack => false;

        #region UIPanel Lifecycle

        protected override void Awake()
        {
            base.Awake();
            _cancelButton.onClick.AddListener(OnClickCancel);
            _confirmButton.onClick.AddListener(OnClickConfirm);
        }

        #endregion

        #region Update

        private void Update()
        {
            if (BuildingPlacer.Instance == null || !BuildingPlacer.Instance.IsPlacing) return;

            bool ok = BuildingPlacer.Instance.CanPlaceAtCurrentPosition();
            SetCanPlace(ok);

            var cell = BuildingPlacer.Instance.CurrentCell;
            SetCoord(cell.x, cell.y);
        }

        #endregion

        #region IInitializable

        public void Initialize(BuildModeOverlayData data)
        {
            _currentInventory = data.Inventory;

            if (_nameText != null) _nameText.text = data.Entry.Data.name;
            if (_gridText != null) _gridText.text = $"{data.Entry.Data.grid_width} × {data.Entry.Data.grid_height}";
        }

        #endregion

        #region Public API

        public void SetCanPlace(bool ok, string reason = null)
        {
            if (_statusBg   != null) _statusBg.sprite   = ok ? _sprStatusOkBg   : _sprStatusNgBg;
            if (_statusIcon != null) _statusIcon.sprite  = ok ? _sprStatusOkIcon : _sprStatusNgIcon;
            if (_statusText != null) _statusText.text    = ok ? "배치 가능" : $"배치 불가 — {reason ?? "공간 부족"}";

            if (_confirmButton      != null) _confirmButton.interactable = ok;
            if (_confirmButtonImage != null) _confirmButtonImage.sprite  = ok ? _sprBtnConfirmActive : _sprBtnConfirmDisabled;
        }

        public void SetCoord(int x, int y)
        {
            if (_coordText != null) _coordText.text = $"현재: ({x}, {y})";
        }

        #endregion

        #region Button Handlers

        private void OnClickCancel()
        {
            BuildingPlacer.Instance.ExitPlacementMode();
            UIManager.Instance.HideOverlay<BuildModeOverlay>();
            UIManager.Instance.Open<BuildingQueuePanel, Inventory>(_currentInventory);
        }

        private void OnClickConfirm()
        {
            if (BuildingPlacer.Instance.TryPlace())
                UIManager.Instance.HideOverlay<BuildModeOverlay>();
        }

        #endregion
    }
}
