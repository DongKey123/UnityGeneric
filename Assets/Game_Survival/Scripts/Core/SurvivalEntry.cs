using Framework.UI;
using SurvivalGame.Data;
using SurvivalGame.Player;
using SurvivalGame.UI;
using UnityEngine;

namespace SurvivalGame.Core
{
    /// <summary>
    /// 서바이벌 게임 진입점입니다.
    /// 씬 시작 시 데이터 로드 및 HUD 초기화를 담당합니다.
    /// MainScene의 빈 GameObject에 붙여 사용하세요.
    /// </summary>
    public class SurvivalEntry : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private PlayerController _player;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            SurvivalDataLoader.LoadAll();
        }

        private void Start()
        {
            UIManager.Instance.ShowOverlay<MainPanel, PlayerController>(_player);
        }

        #endregion
    }
}
