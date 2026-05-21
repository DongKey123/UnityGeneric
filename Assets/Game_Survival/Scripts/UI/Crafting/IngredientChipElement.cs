using TMPro;
using UnityEngine;

namespace SurvivalGame.UI
{
    /// <summary>
    /// 재료 칩/행 하나를 표시하는 최소 컴포넌트입니다.
    /// CraftingCardElement(카드 목록)와 CraftingPanel(상세 패널) 양쪽에서 공유합니다.
    /// </summary>
    public class IngredientChipElement : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;

        /// <param name="itemName">아이템 이름</param>
        /// <param name="have">현재 보유 수량</param>
        /// <param name="need">필요 수량</param>
        /// <param name="fontSize">폰트 크기 — 카드: 13, 상세: 16</param>
        public void Setup(string itemName, int have, int need, float fontSize = 14f)
        {
            bool ok      = have >= need;
            _text.text   = $"{itemName}  {have}/{need}";
            _text.fontSize = fontSize;
            _text.color  = ok
                ? new Color(0.35f, 0.60f, 0.19f)
                : new Color(0.78f, 0.31f, 0.19f);
        }
    }
}
