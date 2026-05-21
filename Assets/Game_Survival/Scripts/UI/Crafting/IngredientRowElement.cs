using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivalGame.UI
{
    /// <summary>
    /// 상세 패널 재료 행 — 아이콘 + 이름 + 수량 + 게이지 바로 구성됩니다.
    /// </summary>
    public class IngredientRowElement : MonoBehaviour
    {
        [SerializeField] private Image           _leftBorder;
        [SerializeField] private Image           _icon;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _countText;
        [SerializeField] private Image           _barFill;

        private static readonly Color ColOkBorder = new Color(0.353f, 0.596f, 0.188f, 1f);
        private static readonly Color ColNgBorder = new Color(0.784f, 0.314f, 0.188f, 1f);
        private static readonly Color ColOkCount  = new Color(0.416f, 0.659f, 0.188f, 1f);
        private static readonly Color ColNgCount  = new Color(0.816f, 0.314f, 0.188f, 1f);

        public void Setup(string itemName, Sprite icon, int have, int need)
        {
            bool ok = have >= need;

            if (_icon != null)
            {
                _icon.sprite  = icon;
                _icon.enabled = icon != null;
            }

            if (_nameText  != null) _nameText.text  = itemName;

            if (_countText != null)
            {
                _countText.text  = $"{have}/{need}";
                _countText.color = ok ? ColOkCount : ColNgCount;
            }

            if (_leftBorder != null) _leftBorder.color = ok ? ColOkBorder : ColNgBorder;

            if (_barFill != null)
            {
                _barFill.fillAmount = need > 0 ? Mathf.Clamp01((float)have / need) : 1f;
                _barFill.color      = ok ? ColOkBorder : ColNgBorder;
            }
        }
    }
}
