using IdleGame.Battle;
using UnityEngine;

namespace IdleGame.SceneEntry
{
    /// <summary>
    /// MainScene 진입점입니다.
    /// PlayerController와 StageManager를 초기화하고 연결합니다.
    /// </summary>
    public class MainEntry : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private PlayerController _player;
        [SerializeField] private StageManager     _stageManager;
        [SerializeField] private int              _startChapterId = 1;

        #endregion

        #region Unity Lifecycle

        private void Start()
        {
            // TODO: 오프라인 보상 계산 및 팝업

            _player.Initialize();
            _stageManager.LoadChapter(_startChapterId, _player, Vector3.zero);
        }

        #endregion
    }
}
