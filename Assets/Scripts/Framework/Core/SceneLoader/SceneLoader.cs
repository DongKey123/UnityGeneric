using System;
using System.Collections;
using Framework.Core.Singleton;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Framework.Core.SceneLoader
{
    /// <summary>
    /// 씬 전환과 로딩 화면을 관리하는 싱글톤 매니저입니다.
    /// 비동기 씬 로딩, 진행률 이벤트, 선택적 트랜지션 효과를 지원합니다.
    /// DontDestroyOnLoad로 유지되므로 씬 전환 후에도 사용 가능합니다.
    /// </summary>
    public class SceneLoader : PersistentMonoSingleton<SceneLoader>
    {
        #region Fields

        private bool _isLoading;

        #endregion

        #region Properties

        /// <summary>현재 씬 로딩 진행 중 여부입니다.</summary>
        public bool IsLoading => _isLoading;

        #endregion

        #region Events

        /// <summary>씬 로딩이 시작될 때 발생합니다.</summary>
        public event Action OnLoadStart;

        /// <summary>
        /// 씬 로딩 진행률이 갱신될 때 발생합니다.
        /// 인자는 0~1 범위의 float 값입니다.
        /// </summary>
        public event Action<float> OnLoadProgress;

        /// <summary>씬 로딩이 완료되고 씬이 활성화된 후 발생합니다.</summary>
        public event Action OnLoadComplete;

        #endregion

        #region Public Methods

        /// <summary>
        /// 씬 이름으로 비동기 씬 전환을 시작합니다.
        /// </summary>
        /// <param name="sceneName">로드할 씬 이름 (Build Settings에 등록된 이름)</param>
        /// <param name="transition">전환 효과 구현체 (null이면 즉시 전환)</param>
        public void LoadScene(string sceneName, ISceneTransition transition = null)
        {
            if (_isLoading)
            {
                Debug.LogWarning("[SceneLoader] 이미 씬 로딩 중입니다.");
                return;
            }

            StartCoroutine(LoadSceneAsyncInternal(sceneName, transition));
        }

        /// <summary>
        /// 씬 인덱스로 비동기 씬 전환을 시작합니다.
        /// </summary>
        /// <param name="sceneIndex">로드할 씬 인덱스 (Build Settings 기준)</param>
        /// <param name="transition">전환 효과 구현체 (null이면 즉시 전환)</param>
        public void LoadScene(int sceneIndex, ISceneTransition transition = null)
        {
            if (_isLoading)
            {
                Debug.LogWarning("[SceneLoader] 이미 씬 로딩 중입니다.");
                return;
            }

            StartCoroutine(LoadSceneAsyncInternal(sceneIndex, transition));
        }

        /// <summary>
        /// 로딩 씬을 경유하여 씬을 전환합니다.
        /// 로딩 씬에서 <see cref="OnLoadProgress"/> 이벤트를 구독해 진행률 UI를 표시하세요.
        /// </summary>
        /// <param name="targetScene">최종 목적지 씬 이름</param>
        /// <param name="loadingScene">중간 로딩 씬 이름</param>
        public void LoadSceneWithLoadingScene(string targetScene, string loadingScene)
        {
            if (_isLoading)
            {
                Debug.LogWarning("[SceneLoader] 이미 씬 로딩 중입니다.");
                return;
            }

            StartCoroutine(LoadSceneViaLoadingScene(targetScene, loadingScene));
        }

        /// <summary>
        /// 현재 활성 씬을 다시 로드합니다.
        /// </summary>
        /// <param name="transition">전환 효과 구현체 (null이면 즉시 전환)</param>
        public void ReloadScene(ISceneTransition transition = null)
        {
            LoadScene(SceneManager.GetActiveScene().name, transition);
        }

        #endregion

        #region Private Methods

        private IEnumerator LoadSceneAsyncInternal(string sceneName, ISceneTransition transition)
        {
            _isLoading = true;
            OnLoadStart?.Invoke();

            if (transition != null)
            {
                yield return StartCoroutine(transition.OnTransitionIn());
            }

            var operation = SceneManager.LoadSceneAsync(sceneName);
            // allowSceneActivation = false로 설정해 로딩 완료 후 씬 활성화 타이밍을 직접 제어
            operation.allowSceneActivation = false;

            while (operation.progress < 0.9f)
            {
                // LoadSceneAsync의 progress는 0~0.9 범위이므로 0~0.99로 정규화
                // 씬 활성화 완료(isDone) 시점에 1.0을 전달하여 100%를 표시
                OnLoadProgress?.Invoke(operation.progress / 0.9f * 0.99f);
                yield return null;
            }

            OnLoadProgress?.Invoke(0.99f);

            if (transition != null)
            {
                yield return StartCoroutine(transition.OnTransitionOut());
            }

            operation.allowSceneActivation = true;

            while (!operation.isDone)
            {
                yield return null;
            }

            OnLoadProgress?.Invoke(1f);
            _isLoading = false;
            OnLoadComplete?.Invoke();
        }

        private IEnumerator LoadSceneAsyncInternal(int sceneIndex, ISceneTransition transition)
        {
            _isLoading = true;
            OnLoadStart?.Invoke();

            if (transition != null)
            {
                yield return StartCoroutine(transition.OnTransitionIn());
            }

            var operation = SceneManager.LoadSceneAsync(sceneIndex);
            operation.allowSceneActivation = false;

            while (operation.progress < 0.9f)
            {
                OnLoadProgress?.Invoke(operation.progress / 0.9f * 0.99f);
                yield return null;
            }

            OnLoadProgress?.Invoke(0.99f);

            if (transition != null)
            {
                yield return StartCoroutine(transition.OnTransitionOut());
            }

            operation.allowSceneActivation = true;

            while (!operation.isDone)
            {
                yield return null;
            }

            OnLoadProgress?.Invoke(1f);
            _isLoading = false;
            OnLoadComplete?.Invoke();
        }

        private IEnumerator LoadSceneViaLoadingScene(string targetScene, string loadingScene)
        {
            _isLoading = true;
            OnLoadStart?.Invoke();

            // 로딩 씬으로 먼저 전환
            yield return SceneManager.LoadSceneAsync(loadingScene);

            // 로딩 씬이 OnLoadProgress를 구독할 수 있도록 한 프레임 대기
            yield return null;

            var operation = SceneManager.LoadSceneAsync(targetScene);
            operation.allowSceneActivation = false;

            while (operation.progress < 0.9f)
            {
                OnLoadProgress?.Invoke(operation.progress / 0.9f * 0.99f);
                yield return null;
            }

            OnLoadProgress?.Invoke(0.99f);

            // 로딩 씬이 완료 연출을 처리할 수 있도록 한 프레임 대기
            yield return null;

            operation.allowSceneActivation = true;

            while (!operation.isDone)
            {
                yield return null;
            }

            OnLoadProgress?.Invoke(1f);
            _isLoading = false;
            OnLoadComplete?.Invoke();
        }

        #endregion
    }
}
