using System.Collections.Generic;
using UnityEngine;

namespace Framework.Utils.Coroutine
{
    /// <summary>
    /// <see cref="WaitForSeconds"/> 및 <see cref="WaitForSecondsRealtime"/> 인스턴스를 캐싱하여
    /// 반복 생성으로 인한 GC 압력을 줄이는 정적 유틸리티입니다.
    /// </summary>
    public static class WaitCache
    {
        #region Fields

        private static readonly Dictionary<float, WaitForSeconds> _cache
            = new Dictionary<float, WaitForSeconds>();

        private static readonly Dictionary<float, WaitForSecondsRealtime> _realtimeCache
            = new Dictionary<float, WaitForSecondsRealtime>();

        #endregion

        #region Public Methods

        /// <summary>
        /// 지정한 시간의 <see cref="WaitForSeconds"/>를 반환합니다.
        /// 캐시에 없으면 새로 생성하여 저장합니다.
        /// </summary>
        /// <param name="seconds">대기할 시간 (초)</param>
        /// <returns>캐싱된 <see cref="WaitForSeconds"/> 인스턴스</returns>
        public static WaitForSeconds Get(float seconds)
        {
            if (!_cache.TryGetValue(seconds, out WaitForSeconds wait))
            {
                wait = new WaitForSeconds(seconds);
                _cache[seconds] = wait;
            }

            return wait;
        }

        /// <summary>
        /// 지정한 시간의 <see cref="WaitForSecondsRealtime"/>을 반환합니다.
        /// 캐시에 없으면 새로 생성하여 저장합니다.
        /// </summary>
        /// <param name="seconds">대기할 실제 시간 (초, TimeScale 무시)</param>
        /// <returns>캐싱된 <see cref="WaitForSecondsRealtime"/> 인스턴스</returns>
        public static WaitForSecondsRealtime GetRealtime(float seconds)
        {
            if (!_realtimeCache.TryGetValue(seconds, out WaitForSecondsRealtime wait))
            {
                wait = new WaitForSecondsRealtime(seconds);
                _realtimeCache[seconds] = wait;
            }

            return wait;
        }

        /// <summary>
        /// 캐시를 모두 비웁니다.
        /// </summary>
        public static void Clear()
        {
            _cache.Clear();
            _realtimeCache.Clear();
        }

        #endregion
    }
}
