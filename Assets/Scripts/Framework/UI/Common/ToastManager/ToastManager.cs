using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Core.Singleton;
using UnityEngine;

namespace Framework.UI
{
    /// <summary>
    /// 화면에 잠깐 표시되고 자동으로 사라지는 토스트 메시지를 관리하는 싱글톤입니다.
    /// 여러 메시지가 동시에 요청되면 큐에 쌓아 순서대로 표시합니다.
    /// </summary>
    public class ToastManager : PersistentMonoSingleton<ToastManager>
    {
        #region Constants

        private const string ToastPanelResourcePath = "UI/ToastPanel";
        private const float DefaultDuration = 2f;
        private const float FadeDuration = 0.3f;

        #endregion

        #region Fields

        [SerializeField] private Canvas _toastCanvas;

        private readonly Queue<ToastRequest> _queue = new();
        private ToastPanel _panel;
        private bool _isShowing;

        #endregion

        #region Public Methods

        /// <summary>
        /// 기본 타입의 토스트 메시지를 표시합니다.
        /// </summary>
        /// <param name="message">표시할 메시지</param>
        /// <param name="duration">표시 유지 시간 (초)</param>
        public void Show(string message, float duration = DefaultDuration)
        {
            Show(message, ToastType.Default, duration);
        }

        /// <summary>
        /// 타입을 지정하여 토스트 메시지를 표시합니다.
        /// 현재 표시 중인 토스트가 있으면 큐에 추가됩니다.
        /// </summary>
        /// <param name="message">표시할 메시지</param>
        /// <param name="type">토스트 타입 (Default / Success / Warning / Error)</param>
        /// <param name="duration">표시 유지 시간 (초)</param>
        public void Show(string message, ToastType type, float duration = DefaultDuration)
        {
            _queue.Enqueue(new ToastRequest(message, type, duration));

            if (!_isShowing)
            {
                StartCoroutine(ProcessQueue());
            }
        }

        #endregion

        #region Private Methods

        private IEnumerator ProcessQueue()
        {
            _isShowing = true;

            while (_queue.Count > 0)
            {
                var request = _queue.Dequeue();
                var panel = GetOrCreatePanel();

                yield return panel.PlayShow(request, FadeDuration);
            }

            _isShowing = false;
        }

        private ToastPanel GetOrCreatePanel()
        {
            if (_panel != null)
            {
                return _panel;
            }

            var prefab = Resources.Load<GameObject>(ToastPanelResourcePath)
                ?? throw new InvalidOperationException($"[ToastManager] 프리팹을 찾을 수 없습니다: Resources/{ToastPanelResourcePath}");

            var instance = Instantiate(prefab, _toastCanvas != null ? _toastCanvas.transform : transform);
            _panel = instance.GetComponent<ToastPanel>()
                ?? throw new InvalidOperationException("[ToastManager] 프리팹에 ToastPanel 컴포넌트가 없습니다.");

            return _panel;
        }

        #endregion
    }
}
