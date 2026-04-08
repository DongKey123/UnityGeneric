using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Framework.Utils.AsyncHelper
{
    /// <summary>
    /// UniTask 기반의 비동기 유틸리티 모음입니다.
    /// async/await 패턴을 간결하게 사용할 수 있도록 래핑합니다.
    /// </summary>
    public static class AsyncHelper
    {
        #region Delay

        /// <summary>
        /// 지정한 시간(초)만큼 대기합니다.
        /// </summary>
        /// <param name="seconds">대기 시간 (초)</param>
        /// <param name="cancellationToken">취소 토큰</param>
        public static UniTask Delay(float seconds, CancellationToken cancellationToken = default)
        {
            return UniTask.Delay(TimeSpan.FromSeconds(seconds), cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 지정한 프레임 수만큼 대기합니다.
        /// </summary>
        /// <param name="frameCount">대기할 프레임 수</param>
        /// <param name="cancellationToken">취소 토큰</param>
        public static UniTask DelayFrame(int frameCount, CancellationToken cancellationToken = default)
        {
            return UniTask.DelayFrame(frameCount, cancellationToken: cancellationToken);
        }

        #endregion

        #region Frame

        /// <summary>
        /// 다음 프레임까지 대기합니다.
        /// </summary>
        /// <param name="cancellationToken">취소 토큰</param>
        public static UniTask NextFrame(CancellationToken cancellationToken = default)
        {
            return UniTask.NextFrame(cancellationToken);
        }

        /// <summary>
        /// 다음 FixedUpdate까지 대기합니다.
        /// </summary>
        /// <param name="cancellationToken">취소 토큰</param>
        public static UniTask WaitForFixedUpdate(CancellationToken cancellationToken = default)
        {
            return UniTask.WaitForFixedUpdate(cancellationToken);
        }

        /// <summary>
        /// 다음 EndOfFrame까지 대기합니다.
        /// </summary>
        /// <param name="cancellationToken">취소 토큰</param>
        public static UniTask WaitForEndOfFrame(CancellationToken cancellationToken = default)
        {
            return UniTask.WaitForEndOfFrame(cancellationToken);
        }

        #endregion

        #region Condition

        /// <summary>
        /// 조건이 true가 될 때까지 매 프레임 대기합니다.
        /// </summary>
        /// <param name="predicate">대기 종료 조건</param>
        /// <param name="cancellationToken">취소 토큰</param>
        public static UniTask WaitUntil(Func<bool> predicate, CancellationToken cancellationToken = default)
        {
            return UniTask.WaitUntil(predicate, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 조건이 false가 될 때까지 매 프레임 대기합니다.
        /// </summary>
        /// <param name="predicate">대기 지속 조건</param>
        /// <param name="cancellationToken">취소 토큰</param>
        public static UniTask WaitWhile(Func<bool> predicate, CancellationToken cancellationToken = default)
        {
            return UniTask.WaitWhile(predicate, cancellationToken: cancellationToken);
        }

        #endregion

        #region Cancellation

        /// <summary>
        /// 새로운 <see cref="CancellationTokenSource"/>를 생성합니다.
        /// </summary>
        /// <returns>새로 생성된 CancellationTokenSource</returns>
        public static CancellationTokenSource CreateCTS()
        {
            return new CancellationTokenSource();
        }

        /// <summary>
        /// MonoBehaviour의 수명과 연동된 CancellationToken을 반환합니다.
        /// GameObject가 파괴되면 자동으로 취소됩니다.
        /// </summary>
        /// <param name="behaviour">연동할 MonoBehaviour</param>
        /// <returns>GameObject 수명과 연동된 CancellationToken</returns>
        public static CancellationToken GetCancellationToken(MonoBehaviour behaviour)
        {
            return behaviour.GetCancellationTokenOnDestroy();
        }

        #endregion
    }
}
