using System;

namespace Framework.Core.Singleton
{
    /// <summary>
    /// 순수 C# 싱글톤입니다.
    /// MonoBehaviour가 필요 없는 매니저, 서비스 클래스에 사용합니다.
    /// 스레드 세이프하게 구현되어 있습니다.
    /// </summary>
    public abstract class Singleton<T> where T : Singleton<T>, new()
    {
        #region Fields

        private static T _instance;
        private static readonly object _lock = new object();

        #endregion

        #region Properties

        public static T Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new T();
                        _instance.OnInitialize();
                    }

                    return _instance;
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 싱글톤 인스턴스를 초기화합니다.
        /// </summary>
        public static void Release()
        {
            lock (_lock)
            {
                _instance = null;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// 싱글톤 초기화 시 호출됩니다.
        /// </summary>
        protected virtual void OnInitialize() { }

        #endregion
    }
}
