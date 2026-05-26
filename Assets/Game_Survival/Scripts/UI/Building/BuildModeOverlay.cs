using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivalGame.UI
{
    /// <summary>
    /// 배치 모드 중 화면 하단에 표시되는 소형 오버레이입니다.
    /// UIManager 스택과 무관하게 독립적으로 Show/Hide됩니다.
    /// </summary>
    public class BuildModeOverlay : MonoBehaviour
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

        #region Unity Lifecycle

        private void Awake()
        {
            _cancelButton.onClick.AddListener(OnClickCancel);
            _confirmButton.onClick.AddListener(OnClickConfirm);
            gameObject.SetActive(false);
        }

        #endregion

        #region Public API

        public void Show(string buildingName, int gridW, int gridH)
        {
            if (_nameText != null) _nameText.text = buildingName;
            if (_gridText != null) _gridText.text = $"{gridW} × {gridH}";
            gameObject.SetActive(true);
        }

        public void Hide() => gameObject.SetActive(false);

        public void SetCanPlace(bool ok, string reason = null)
        {
            if (_statusBg   != null) _statusBg.sprite   = ok ? _sprStatusOkBg  : _sprStatusNgBg;
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

        #region Button Handlers (stub — 로직은 추후 연결)

        private void OnClickCancel()  { /* TODO: BuildingPlacer.ExitPlacementMode + 큐 패널 복귀 */ }
        private void OnClickConfirm() { /* TODO: BuildingPlacer.TryPlace */ }

        #endregion
    }
}
