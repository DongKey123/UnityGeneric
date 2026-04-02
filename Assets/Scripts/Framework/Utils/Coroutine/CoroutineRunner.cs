using System.Collections;
using Framework.Core.Singleton;
using UnityEngine;

namespace Framework.Utils.Coroutine
{
    /// <summary>
    /// MonoBehaviour가 없는 일반 클래스에서 코루틴을 실행할 수 있게 해주는 싱글톤 러너입니다.
    /// DontDestroyOnLoad로 유지되므로 씬 전환 후에도 사용 가능합니다.
    /// </summary>
    public class CoroutineRunner : PersistentMonoSingleton<CoroutineRunner>
    {
        #region Public Methods

        /// <summary>
        /// 코루틴을 실행합니다.
        /// </summary>
        /// <param name="coroutine">실행할 코루틴</param>
        /// <returns>중지 시 사용할 <see cref="UnityEngine.Coroutine"/> 핸들</returns>
        public new UnityEngine.Coroutine StartCoroutine(IEnumerator coroutine)
        {
            return base.StartCoroutine(coroutine);
        }

        /// <summary>
        /// 실행 중인 코루틴을 중지합니다.
        /// </summary>
        /// <param name="coroutine">중지할 코루틴 핸들</param>
        public new void StopCoroutine(UnityEngine.Coroutine coroutine)
        {
            if (coroutine != null)
            {
                base.StopCoroutine(coroutine);
            }
        }

        /// <summary>
        /// 현재 실행 중인 모든 코루틴을 중지합니다.
        /// </summary>
        public new void StopAllCoroutines()
        {
            base.StopAllCoroutines();
        }

        #endregion
    }
}
