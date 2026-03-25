using UnityEngine;

namespace Framework.Core.Singleton
{
    /// <summary>
    /// DontDestroyOnLoad가 적용된 MonoBehaviour 싱글톤입니다.
    /// 씬에 없으면 자동으로 GameObject를 생성합니다.
    /// 씬 전환 후에도 파괴되지 않고 유지됩니다.
    /// </summary>
    public abstract class PersistentMonoSingleton<T> : MonoBehaviour where T : PersistentMonoSingleton<T>
    {
        #region Fields

        private static T _instance;

        #endregion

        #region Properties

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();

                    if (_instance == null)
                    {
                        var go = new GameObject(typeof(T).Name);
                        _instance = go.AddComponent<T>();
                    }
                }

                return _instance;
            }
        }

        #endregion

        #region Unity Lifecycle

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
                OnInitialize();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// 싱글톤 초기화 시 호출됩니다. Awake 대신 이 메서드를 오버라이드하세요.
        /// </summary>
        protected virtual void OnInitialize() { }

        #endregion
    }
}
