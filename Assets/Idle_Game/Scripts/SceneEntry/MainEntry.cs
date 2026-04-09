using UnityEngine;

namespace IdleGame.SceneEntry
{
    /// <summary>
    /// MainScene 진입점입니다.
    /// 자동사냥, 강화, 설정 등 모든 게임 컨텐츠가 이 씬에서 동작합니다.
    /// </summary>
    public class MainEntry : MonoBehaviour
    {
        #region Unity Lifecycle

        private void Start()
        {
            // TODO: 오프라인 보상 계산 및 팝업
            // TODO: StageManager 초기화
            // TODO: PlayerController 초기화
        }

        #endregion
    }
}
