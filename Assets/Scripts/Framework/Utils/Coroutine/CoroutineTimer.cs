using System;
using System.Collections;
using UnityEngine;

namespace Framework.Utils.Coroutine
{
    /// <summary>
    /// 코루틴 기반의 타이머 유틸리티입니다.
    /// 내부적으로 <see cref="CoroutineRunner"/>와 <see cref="WaitCache"/>를 사용합니다.
    /// MonoBehaviour 없이도 호출할 수 있습니다.
    /// </summary>
    public static class CoroutineTimer
    {
        #region Public Methods

        /// <summary>
        /// 지정한 시간 후 콜백을 실행합니다.
        /// </summary>
        /// <param name="delay">대기 시간 (초)</param>
        /// <param name="callback">실행할 콜백</param>
        /// <returns>중지 시 사용할 <see cref="UnityEngine.Coroutine"/> 핸들</returns>
        public static UnityEngine.Coroutine Delay(float delay, Action callback)
        {
            return CoroutineRunner.Instance.StartCoroutine(DelayRoutine(delay, callback));
        }

        /// <summary>
        /// 지정한 시간 후 콜백을 실행합니다. TimeScale의 영향을 받지 않습니다.
        /// </summary>
        /// <param name="delay">대기 시간 (실제 초)</param>
        /// <param name="callback">실행할 콜백</param>
        /// <returns>중지 시 사용할 <see cref="UnityEngine.Coroutine"/> 핸들</returns>
        public static UnityEngine.Coroutine DelayRealtime(float delay, Action callback)
        {
            return CoroutineRunner.Instance.StartCoroutine(DelayRealtimeRoutine(delay, callback));
        }

        /// <summary>
        /// 일정 간격으로 콜백을 반복 실행합니다.
        /// </summary>
        /// <param name="interval">반복 간격 (초)</param>
        /// <param name="callback">반복 실행할 콜백</param>
        /// <param name="count">반복 횟수. -1이면 무한 반복</param>
        /// <returns>중지 시 사용할 <see cref="UnityEngine.Coroutine"/> 핸들</returns>
        public static UnityEngine.Coroutine Repeat(float interval, Action callback, int count = -1)
        {
            return CoroutineRunner.Instance.StartCoroutine(RepeatRoutine(interval, callback, count));
        }

        /// <summary>
        /// 조건이 참이 될 때까지 대기한 뒤 콜백을 실행합니다.
        /// </summary>
        /// <param name="condition">대기 종료 조건</param>
        /// <param name="callback">조건 충족 시 실행할 콜백</param>
        /// <returns>중지 시 사용할 <see cref="UnityEngine.Coroutine"/> 핸들</returns>
        public static UnityEngine.Coroutine WaitUntil(Func<bool> condition, Action callback)
        {
            return CoroutineRunner.Instance.StartCoroutine(WaitUntilRoutine(condition, callback));
        }

        /// <summary>
        /// 값을 <paramref name="from"/>에서 <paramref name="to"/>까지 선형 보간하며
        /// 매 프레임 <paramref name="onUpdate"/>를 호출합니다.
        /// </summary>
        /// <param name="from">시작 값</param>
        /// <param name="to">목표 값</param>
        /// <param name="duration">보간 시간 (초)</param>
        /// <param name="onUpdate">매 프레임 호출되는 콜백. 인자는 현재 보간 값</param>
        /// <param name="onComplete">보간 완료 시 호출되는 콜백</param>
        /// <returns>중지 시 사용할 <see cref="UnityEngine.Coroutine"/> 핸들</returns>
        public static UnityEngine.Coroutine Lerp(float from, float to, float duration,
            Action<float> onUpdate, Action onComplete = null)
        {
            return CoroutineRunner.Instance.StartCoroutine(LerpRoutine(from, to, duration, onUpdate, onComplete));
        }

        /// <summary>
        /// 실행 중인 타이머를 중지합니다.
        /// </summary>
        /// <param name="coroutine">중지할 코루틴 핸들</param>
        public static void Cancel(UnityEngine.Coroutine coroutine)
        {
            CoroutineRunner.Instance.StopCoroutine(coroutine);
        }

        #endregion

        #region Private Methods

        private static IEnumerator DelayRoutine(float delay, Action callback)
        {
            yield return WaitCache.Get(delay);
            callback?.Invoke();
        }

        private static IEnumerator DelayRealtimeRoutine(float delay, Action callback)
        {
            yield return WaitCache.GetRealtime(delay);
            callback?.Invoke();
        }

        private static IEnumerator RepeatRoutine(float interval, Action callback, int count)
        {
            int executed = 0;

            while (count < 0 || executed < count)
            {
                yield return WaitCache.Get(interval);
                callback?.Invoke();
                executed++;
            }
        }

        private static IEnumerator WaitUntilRoutine(Func<bool> condition, Action callback)
        {
            yield return new UnityEngine.WaitUntil(condition);
            callback?.Invoke();
        }

        private static IEnumerator LerpRoutine(float from, float to, float duration,
            Action<float> onUpdate, Action onComplete)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                onUpdate?.Invoke(Mathf.Lerp(from, to, elapsed / duration));
                yield return null;
            }

            onUpdate?.Invoke(to);
            onComplete?.Invoke();
        }

        #endregion
    }
}
