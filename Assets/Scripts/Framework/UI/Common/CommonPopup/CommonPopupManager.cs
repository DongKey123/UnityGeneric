using System;
using System.Collections.Generic;
using Framework.Core.Singleton;
using UnityEngine;

namespace Framework.UI
{
    /// <summary>
    /// OneButtonPopup, TwoButtonPopup 등 공용 팝업을 스택으로 관리하는 싱글톤입니다.
    /// <para>
    /// - 팝업이 열릴 때 Dim 오버레이를 자동으로 표시하고, 모든 팝업이 닫히면 숨깁니다.<br/>
    /// - 팝업 프리팹은 <c>Resources/UI/</c> 경로에 클래스 이름과 동일한 이름으로 저장해야 합니다.
    /// </para>
    /// </summary>
    public class CommonPopupManager : PersistentMonoSingleton<CommonPopupManager>
    {
        #region Constants

        private const string PopupResourcePath = "UI";

        #endregion

        #region Fields

        [SerializeField] private Canvas _popupCanvas;

        /// <summary>팝업 뒤에 표시되는 반투명 배경입니다. 할당하지 않으면 Dim 처리를 생략합니다.</summary>
        [SerializeField] private CanvasGroup _dim;

        private readonly Stack<PopupBase> _stack = new();
        private readonly Dictionary<Type, PopupBase> _registry = new();

        #endregion

        #region Properties

        /// <summary>현재 열려 있는 팝업 수입니다.</summary>
        public int PopupCount => _stack.Count;

        /// <summary>팝업이 하나 이상 열려 있는지 여부입니다.</summary>
        public bool IsAnyOpen => _stack.Count > 0;

        #endregion

        #region Public Methods

        /// <summary>
        /// 확인 버튼 하나짜리 팝업을 표시합니다.
        /// </summary>
        /// <param name="title">팝업 제목</param>
        /// <param name="message">팝업 본문 메시지</param>
        /// <param name="buttonText">확인 버튼 텍스트</param>
        /// <param name="onConfirm">확인 버튼 클릭 콜백</param>
        public void ShowOneButton(string title, string message, string buttonText = "확인", Action onConfirm = null)
        {
            var popup = GetOrCreate<OneButtonPopup>();

            if (popup.IsOpen)
            {
                Debug.LogWarning("[CommonPopupManager] OneButtonPopup이 이미 열려 있습니다.");
                return;
            }

            popup.Setup(title, message, buttonText, onConfirm, Close);
            Push(popup);
        }

        /// <summary>
        /// 확인/취소 버튼 두 개짜리 팝업을 표시합니다.
        /// </summary>
        /// <param name="title">팝업 제목</param>
        /// <param name="message">팝업 본문 메시지</param>
        /// <param name="confirmText">확인 버튼 텍스트</param>
        /// <param name="cancelText">취소 버튼 텍스트</param>
        /// <param name="onConfirm">확인 버튼 클릭 콜백</param>
        /// <param name="onCancel">취소 버튼 클릭 콜백</param>
        public void ShowTwoButton(string title, string message,
            string confirmText = "확인", string cancelText = "취소",
            Action onConfirm = null, Action onCancel = null)
        {
            var popup = GetOrCreate<TwoButtonPopup>();

            if (popup.IsOpen)
            {
                Debug.LogWarning("[CommonPopupManager] TwoButtonPopup이 이미 열려 있습니다.");
                return;
            }

            popup.Setup(title, message, confirmText, cancelText, onConfirm, onCancel, Close);
            Push(popup);
        }

        /// <summary>
        /// 최상단 팝업을 닫습니다.
        /// </summary>
        public void Close()
        {
            if (_stack.Count == 0)
            {
                return;
            }

            var popup = _stack.Pop();
            popup.OnClose();
            UpdateDim();

#if UNITY_EDITOR
            Debug.Log($"[CommonPopupManager] Closed: {popup.GetType().Name} (stack: {_stack.Count})");
#endif
        }

        /// <summary>
        /// 열려 있는 모든 팝업을 닫습니다.
        /// </summary>
        public void CloseAll()
        {
            while (_stack.Count > 0)
            {
                _stack.Pop().OnClose();
            }

            UpdateDim();

#if UNITY_EDITOR
            Debug.Log("[CommonPopupManager] CloseAll");
#endif
        }

        /// <summary>
        /// 뒤로가기 입력을 처리합니다. 열린 팝업이 있으면 최상단을 닫습니다.
        /// 모바일 뒤로가기 버튼, Escape 키와 연동하여 사용하세요.
        /// </summary>
        /// <returns>닫은 팝업이 있으면 true</returns>
        public bool HandleBack()
        {
            if (_stack.Count == 0)
            {
                return false;
            }

            Close();
            return true;
        }

        #endregion

        #region Private Methods

        private void Push(PopupBase popup)
        {
            _stack.Push(popup);
            popup.transform.SetAsLastSibling();
            popup.OnOpen();
            UpdateDim();

#if UNITY_EDITOR
            Debug.Log($"[CommonPopupManager] Opened: {popup.GetType().Name} (stack: {_stack.Count})");
#endif
        }

        private void UpdateDim()
        {
            if (_dim == null)
            {
                return;
            }

            bool hasPopup = _stack.Count > 0;
            _dim.alpha = hasPopup ? 1f : 0f;
            _dim.blocksRaycasts = hasPopup;

            // Dim은 항상 팝업들 뒤에 위치
            _dim.transform.SetAsFirstSibling();
        }

        /// <summary>
        /// 레지스트리에서 팝업을 찾아 반환하거나, 없으면 <c>Resources/UI/</c>에서 로드 및 인스턴스화합니다.
        /// </summary>
        /// <typeparam name="T">팝업 타입</typeparam>
        /// <returns>등록된 또는 새로 생성된 팝업</returns>
        /// <exception cref="InvalidOperationException">프리팹을 찾을 수 없거나 컴포넌트가 없는 경우</exception>
        private T GetOrCreate<T>() where T : PopupBase
        {
            if (_registry.TryGetValue(typeof(T), out var existing))
            {
                return existing as T;
            }

            var prefab = Resources.Load<GameObject>($"{PopupResourcePath}/{typeof(T).Name}")
                ?? throw new InvalidOperationException(
                    $"[CommonPopupManager] 프리팹을 찾을 수 없습니다: Resources/{PopupResourcePath}/{typeof(T).Name}");

            var parent = _popupCanvas != null ? _popupCanvas.transform : transform;
            var instance = Instantiate(prefab, parent);
            var popup = instance.GetComponent<T>()
                ?? throw new InvalidOperationException(
                    $"[CommonPopupManager] 프리팹에 {typeof(T).Name} 컴포넌트가 없습니다.");

            _registry[typeof(T)] = popup;
            return popup;
        }

        #endregion
    }
}
