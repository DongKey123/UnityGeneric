using SurvivalGame.Defines;
using SurvivalGame.Inventories;
using SurvivalGame.Player;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivalGame.UI
{
    /// <summary>
    /// 캐릭터 실루엣 + 6개 장비 슬롯을 표시하는 서브 패널입니다.
    /// EquipmentSlotType 순서(1~6)와 _slotBgs / _slotIcons 배열 인덱스(0~5)가 1:1 대응합니다.
    /// </summary>
    public class EquipmentSlotsSubPanel : Framework.UI.SubPanel
    {
        #region Inspector

        [SerializeField] private Image[] _slotBgs;   // 6개 — Weapon/Tool/Head/Chest/Legs/Boots 순
        [SerializeField] private Image[] _slotIcons; // 6개 — 아이템 아이콘 (비면 placeholder)

        [Header("Sprites")]
        [SerializeField] private Sprite _eqslotEmpty;
        [SerializeField] private Sprite _eqslotFilled;
        [SerializeField] private Sprite[] _placeholderIcons; // 6개 — 슬롯 타입별 기본 아이콘

        #endregion

        #region Private Fields

        private EquipmentSlots _equipment;

        #endregion

        #region Public Methods

        public void Setup(EquipmentSlots equipment)
        {
            _equipment = equipment;
            _equipment.OnChanged += Refresh;
        }

        public void Cleanup()
        {
            if (_equipment != null)
                _equipment.OnChanged -= Refresh;
        }

        #endregion

        #region SubPanel

        protected override void OnShown()  => Refresh();
        protected override void OnRefresh() => RefreshSlots();

        #endregion

        #region Private Methods

        private void Refresh() => RefreshSlots();

        private void RefreshSlots()
        {
            for (int i = 0; i < 6; i++)
            {
                var slotType = (EquipmentSlotType)(i + 1);
                var slot     = _equipment?.GetEquipped(slotType);

                _slotBgs[i].sprite = slot != null ? _eqslotFilled : _eqslotEmpty;

                if (slot != null)
                {
                    var icon = Resources.Load<Sprite>(slot.Data.icon_path);
                    _slotIcons[i].sprite  = icon != null ? icon : _placeholderIcons[i];
                    _slotIcons[i].color   = Color.white;
                }
                else
                {
                    _slotIcons[i].sprite = _placeholderIcons[i];
                    _slotIcons[i].color  = new Color(1f, 1f, 1f, 0.3f);
                }
            }
        }

        #endregion
    }
}
