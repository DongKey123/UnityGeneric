using UnityEngine;
using UnityEngine.UI;

namespace IdleGame.Battle
{
    /// <summary>
    /// 캐릭터 머리 위 월드스페이스 HP바입니다.
    /// PlayerController / MonsterController에서 TakeDamage 시 호출하세요.
    /// </summary>
    public class HpBar : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private Image _fillImage;

        #endregion

        #region Unity Lifecycle

        private void LateUpdate()
        {
            transform.forward = Camera.main.transform.forward;
        }

        #endregion

        #region Public Methods

        /// <summary>HP 비율로 바를 갱신합니다.</summary>
        /// <param name="current">현재 체력</param>
        /// <param name="max">최대 체력</param>
        public void UpdateHp(int current, int max)
        {
            if (max <= 0) return;
            _fillImage.fillAmount = (float)current / max;
        }

        #endregion
    }
}
