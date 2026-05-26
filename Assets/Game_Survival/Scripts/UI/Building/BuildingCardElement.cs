using System;
using SurvivalGame.Building;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivalGame.UI
{
    /// <summary>
    /// 건물 관리 패널 좌측 목록의 카드 한 장입니다.
    /// Pending(배치 대기) / Placed(설치됨) 두 상태를 표시합니다.
    /// </summary>
    public class BuildingCardElement : MonoBehaviour
    {
        [SerializeField] private Image           _cardBg;
        [SerializeField] private Button          _button;
        [SerializeField] private Image           _icon;
        [SerializeField] private Image           _checkOverlay;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _gridSizeText;
        [SerializeField] private Image           _badgeImage;
        [SerializeField] private TextMeshProUGUI _badgeText;

        [Header("State Sprites")]
        [SerializeField] private Sprite _sprNormal;
        [SerializeField] private Sprite _sprSelected;
        [SerializeField] private Sprite _sprPlaced;
        [SerializeField] private Sprite _sprPlacedSelected;
        [SerializeField] private Sprite _sprBadgePending;
        [SerializeField] private Sprite _sprBadgePlaced;

        public BuildingQueueEntry       BoundEntry { get; private set; }
        public event Action<BuildingCardElement> OnClicked;

        private bool _isSelected;

        private void Awake()
        {
            _button.onClick.AddListener(() => OnClicked?.Invoke(this));
        }

        public void Setup(BuildingQueueEntry entry)
        {
            BoundEntry         = entry;
            _nameText.text     = entry.Data.name;
            _gridSizeText.text = $"{entry.Data.grid_width} × {entry.Data.grid_height}";

            bool placed = entry.State == BuildingState.Placed;
            if (_checkOverlay != null) _checkOverlay.gameObject.SetActive(placed);
            if (_badgeImage   != null) _badgeImage.sprite = placed ? _sprBadgePlaced   : _sprBadgePending;
            if (_badgeText    != null) _badgeText.text    = placed ? "설치됨"           : "대기 중";

            RefreshBg();
        }

        public void SetSelected(bool selected)
        {
            _isSelected = selected;
            RefreshBg();
        }

        private void RefreshBg()
        {
            if (_cardBg == null) return;
            bool placed = BoundEntry?.State == BuildingState.Placed;
            _cardBg.sprite = (_isSelected, placed) switch
            {
                (true,  true)  => _sprPlacedSelected,
                (true,  false) => _sprSelected,
                (false, true)  => _sprPlaced,
                _              => _sprNormal,
            };
        }
    }
}
