using SurvivalGame.Inventories;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivalGame.UI
{
    /// <summary>
    /// 인벤토리 슬롯 하나의 Element입니다.
    /// SlotGrid 안에 반복 배치되는 단위 프리팹에 붙여 사용하세요.
    /// </summary>
    public class InventorySlotElement : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private Image           _background;  // 항상 표시되는 슬롯 배경
        [SerializeField] private Image           _iconImage;
        [SerializeField] private TextMeshProUGUI _countText;

        #endregion

        #region Public Methods

        /// <summary>슬롯 데이터로 UI를 갱신합니다.</summary>
        public void Refresh(InventorySlot slot)
        {
            if (slot == null)
            {
                SetEmpty();
                return;
            }

            _iconImage.enabled = true;

            _countText.text = slot.Count > 1 ? slot.Count.ToString() : string.Empty;
        }

        #endregion

        #region Private Methods

        private void SetEmpty()
        {
            _iconImage.enabled = false;
            _countText.text    = string.Empty;
        }

        #endregion
    }
}
