using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core.EventBus
{
    /// <summary>
    /// 오브젝트 간 직접 참조 없이 이벤트를 주고받는 글로벌 이벤트 시스템입니다.
    /// 이벤트는 GC 부하를 줄이기 위해 struct 사용을 권장하며, Project/Events/ 폴더에 카테고리별로 모아두는 것을 권장합니다.
    ///
    /// [구독 해제 책임]
    /// Subscribe한 쪽에서 반드시 OnDestroy 등에서 Unsubscribe를 호출해야 합니다.
    /// 해제하지 않으면 오브젝트가 파괴된 후에도 콜백이 호출되어 오류가 발생할 수 있습니다.
    ///
    /// void OnDestroy()
    /// {
    ///     EventBus.Unsubscribe&lt;PlayerDeadEvent&gt;(OnPlayerDead);
    /// }
    /// </summary>
    public static class EventBus
    {
        #region Fields

        private static readonly Dictionary<Type, List<Delegate>> _subscribers = new();

        #endregion

        #region Initialization

        /// <summary>
        /// 플레이 모드 재시작 시 정적 상태를 초기화합니다.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            _subscribers.Clear();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 이벤트를 구독합니다.
        /// </summary>
        /// <typeparam name="T">구독할 이벤트 타입</typeparam>
        /// <param name="handler">이벤트 수신 시 호출될 콜백</param>
        public static void Subscribe<T>(Action<T> handler)
        {
            var type = typeof(T);

            if (!_subscribers.ContainsKey(type))
            {
                _subscribers[type] = new List<Delegate>();
            }

            _subscribers[type].Add(handler);

#if UNITY_EDITOR
            Debug.Log($"[EventBus] Subscribed: {typeof(T).Name} ← {handler.Target?.GetType().Name}.{handler.Method.Name}");
#endif
        }

        /// <summary>
        /// 이벤트 구독을 해제합니다. 오브젝트 파괴 시 반드시 호출하세요.
        /// </summary>
        /// <typeparam name="T">해제할 이벤트 타입</typeparam>
        /// <param name="handler">등록했던 콜백</param>
        public static void Unsubscribe<T>(Action<T> handler)
        {
            var type = typeof(T);

            if (!_subscribers.TryGetValue(type, out var list))
            {
                return;
            }

            list.Remove(handler);

#if UNITY_EDITOR
            Debug.Log($"[EventBus] Unsubscribed: {typeof(T).Name} ← {handler.Target?.GetType().Name}.{handler.Method.Name}");
#endif
        }

        /// <summary>
        /// 이벤트를 발행합니다. 구독 중인 모든 핸들러에 전달됩니다.
        /// </summary>
        /// <typeparam name="T">발행할 이벤트 타입</typeparam>
        /// <param name="eventData">이벤트 데이터</param>
        public static void Publish<T>(T eventData)
        {
#if UNITY_EDITOR
            Debug.Log($"[EventBus] Published: {typeof(T).Name}");
#endif

            var type = typeof(T);

            if (!_subscribers.TryGetValue(type, out var list) || list.Count == 0)
            {
                return;
            }

            // 핸들러 내부에서 Subscribe/Unsubscribe가 호출될 경우를 대비해 복사본으로 순회
            var copy = list.ToArray();

            foreach (var subscriber in copy)
            {
                ((Action<T>)subscriber).Invoke(eventData);
            }
        }

        #endregion
    }
}
