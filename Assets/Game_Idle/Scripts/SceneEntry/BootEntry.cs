using Framework.Core.DataManager;
using Framework.Core.SceneLoader;
using UnityEngine;

namespace IdleGame.SceneEntry
{
    /// <summary>
    /// BootScene 진입점입니다.
    /// 게임 데이터를 로드하고 MainScene으로 전환합니다.
    /// </summary>
    public class BootEntry : MonoBehaviour
    {
        #region Fields

        [SerializeField] private string _nextScene = "MainScene";

        #endregion

        #region Unity Lifecycle

        private void Start()
        {
            InGameDataManager.Instance.LoadAll();
            SceneLoader.Instance.LoadScene(_nextScene);
        }

        #endregion
    }
}
