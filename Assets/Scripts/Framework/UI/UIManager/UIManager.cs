using System;
using System.Collections.Generic;
using Framework.Core.Singleton;
using UnityEngine;

namespace Framework.UI
{
    /// <summary>
    /// UI 패널을 2레이어(Default 스택 / Overlay)로 관리하는 싱글톤 매니저입니다.
    /// <para>
    /// - Default 레이어: 스택 기반 패널 네비게이션 (메뉴, 설정, 인벤토리 등)<br/>
    /// - Overlay 레이어: 스택과 무관하게 최상단에 항상 표시되는 패널 (로딩, 알림 등)
    /// </para>
    /// 패널 프리팹은 <c>Resources/UI/</c> 경로에 클래스 이름과 동일한 이름으로 저장해야 합니다.
    /// </summary>
    public class UIManager : PersistentMonoSingleton<UIManager>
    {
        #region Constants

        private const string PanelResourcePath = "UI";

        #endregion

        #region Events

        /// <summary>패널이 열릴 때 발생합니다.</summary>
        public event Action<UIPanel> OnPanelOpened;

        /// <summary>패널이 닫힐 때 발생합니다.</summary>
        public event Action<UIPanel> OnPanelClosed;

        /// <summary>Default 스택이 비었을 때 발생합니다.</summary>
        public event Action OnStackEmpty;

        #endregion

        #region Fields

        [SerializeField] private Canvas _defaultCanvas;
        [SerializeField] private Canvas _overlayCanvas;

        private readonly Stack<UIPanel> _panelStack = new();
        private readonly List<UIPanel> _overlayList = new();
        private readonly Dictionary<Type, UIPanel> _panelRegistry = new();

        #endregion

        #region Properties

        /// <summary>Default 스택에 현재 열려 있는 패널 수입니다.</summary>
        public int PanelCount => _panelStack.Count;

        /// <summary>현재 Default 스택 최상단 패널입니다. 없으면 null을 반환합니다.</summary>
        public UIPanel Current => _panelStack.Count > 0 ? _panelStack.Peek() : null;

        /// <summary>Overlay 레이어에 하나 이상의 패널이 열려 있는지 여부입니다.</summary>
        public bool IsOverlayOpen => _overlayList.Count > 0;

        #endregion

        #region Public Methods — Default Layer

        /// <summary>
        /// 데이터를 전달하며 패널을 열고 Default 스택에 추가합니다.
        /// 미등록 패널은 <c>Resources/UI/</c> 경로에서 자동으로 로드 및 인스턴스화합니다.
        /// 이미 열려 있는 패널은 중복 추가하지 않습니다.
        /// </summary>
        /// <typeparam name="T">열 패널 타입</typeparam>
        /// <typeparam name="TData">전달할 데이터 타입</typeparam>
        /// <param name="data">패널에 전달할 데이터. null이면 Initialize를 호출하지 않습니다.</param>
        public void Open<T, TData>(TData data = null) where T : UIPanel where TData : class
        {
            var panel = GetOrCreate<T>(_defaultCanvas);

            if (panel.IsOpen)
            {
                Debug.LogWarning($"[UIManager] 이미 열려 있는 패널입니다: {typeof(T).Name}");
                return;
            }

            if (data != null && panel is IInitializable<TData> initializable)
            {
                initializable.Initialize(data);
            }

            _panelStack.Push(panel);
            panel.OnOpen();
            OnPanelOpened?.Invoke(panel);

#if UNITY_EDITOR
            Debug.Log($"[UIManager] Opened: {typeof(T).Name} (stack: {_panelStack.Count})");
#endif
        }

        /// <summary>
        /// 패널을 열고 Default 스택에 추가합니다.
        /// 미등록 패널은 <c>Resources/UI/</c> 경로에서 자동으로 로드 및 인스턴스화합니다.
        /// 이미 열려 있는 패널은 중복 추가하지 않습니다.
        /// </summary>
        /// <typeparam name="T">열 패널 타입</typeparam>
        public void Open<T>() where T : UIPanel
        {
            var panel = GetOrCreate<T>(_defaultCanvas);

            if (panel.IsOpen)
            {
                Debug.LogWarning($"[UIManager] 이미 열려 있는 패널입니다: {typeof(T).Name}");
                return;
            }

            _panelStack.Push(panel);
            panel.OnOpen();
            OnPanelOpened?.Invoke(panel);

#if UNITY_EDITOR
            Debug.Log($"[UIManager] Opened: {typeof(T).Name} (stack: {_panelStack.Count})");
#endif
        }

        /// <summary>
        /// 최상단 패널을 닫고 스택에서 제거합니다.
        /// <see cref="UIPanel.CanClose"/>가 false인 패널은 건너뜁니다.
        /// </summary>
        public void Close()
        {
            if (_panelStack.Count == 0)
            {
                Debug.LogWarning("[UIManager] 닫을 패널이 없습니다.");
                return;
            }

            var panel = _panelStack.Peek();

            if (!panel.CanClose)
            {
                Debug.LogWarning($"[UIManager] CanClose=false 패널은 닫을 수 없습니다: {panel.GetType().Name}");
                return;
            }

            _panelStack.Pop();
            panel.OnClose();
            OnPanelClosed?.Invoke(panel);
            DestroyIfNeeded(panel);

            if (_panelStack.Count == 0)
            {
                OnStackEmpty?.Invoke();
            }

#if UNITY_EDITOR
            Debug.Log($"[UIManager] Closed: {panel.GetType().Name} (stack: {_panelStack.Count})");
#endif
        }

        /// <summary>
        /// <see cref="UIPanel.CanClose"/>가 true인 모든 패널을 닫습니다.
        /// </summary>
        public void CloseAll()
        {
            var remaining = new Stack<UIPanel>();

            while (_panelStack.Count > 0)
            {
                var panel = _panelStack.Pop();

                if (panel.CanClose)
                {
                    panel.OnClose();
                    OnPanelClosed?.Invoke(panel);
                    DestroyIfNeeded(panel);
                }
                else
                {
                    remaining.Push(panel);
                }
            }

            while (remaining.Count > 0)
            {
                _panelStack.Push(remaining.Pop());
            }

            if (_panelStack.Count == 0)
            {
                OnStackEmpty?.Invoke();
            }

#if UNITY_EDITOR
            Debug.Log($"[UIManager] CloseAll — 남은 패널 수: {_panelStack.Count}");
#endif
        }

        /// <summary>
        /// 뒤로가기 입력을 처리합니다.
        /// 최상단 패널의 <see cref="UIPanel.CloseOnBack"/>이 true이면 닫습니다.
        /// 모바일 뒤로가기 버튼과 연동하여 사용하세요.
        /// </summary>
        /// <returns>닫은 패널이 있으면 true</returns>
        public bool HandleBack()
        {
            if (_panelStack.Count == 0)
            {
                return false;
            }

            var top = _panelStack.Peek();

            if (!top.CloseOnBack)
            {
                return false;
            }

            Close();
            return true;
        }

        #endregion

        #region Public Methods — Overlay Layer

        /// <summary>
        /// 패널을 Overlay 레이어에 표시합니다.
        /// 미등록 패널은 <c>Resources/UI/</c> 경로에서 자동으로 로드 및 인스턴스화합니다.
        /// </summary>
        /// <typeparam name="T">표시할 오버레이 패널 타입</typeparam>
        public void ShowOverlay<T>() where T : UIPanel
        {
            var panel = GetOrCreate<T>(_overlayCanvas);

            if (panel.IsOpen)
            {
                Debug.LogWarning($"[UIManager] 이미 열려 있는 오버레이 패널입니다: {typeof(T).Name}");
                return;
            }

            _overlayList.Add(panel);
            panel.OnOpen();
            OnPanelOpened?.Invoke(panel);

#if UNITY_EDITOR
            Debug.Log($"[UIManager] Overlay Shown: {typeof(T).Name}");
#endif
        }

        /// <summary>
        /// Overlay 레이어에서 해당 타입의 패널을 숨깁니다.
        /// </summary>
        /// <typeparam name="T">숨길 오버레이 패널 타입</typeparam>
        public void HideOverlay<T>() where T : UIPanel
        {
            var panel = Get<T>();

            if (!_overlayList.Remove(panel))
            {
                return;
            }

            panel.OnClose();
            OnPanelClosed?.Invoke(panel);
            DestroyIfNeeded(panel);

#if UNITY_EDITOR
            Debug.Log($"[UIManager] Overlay Hidden: {typeof(T).Name}");
#endif
        }

        /// <summary>
        /// Overlay 레이어에 열려 있는 모든 패널을 숨깁니다.
        /// </summary>
        public void HideAllOverlays()
        {
            for (int i = _overlayList.Count - 1; i >= 0; i--)
            {
                var panel = _overlayList[i];
                panel.OnClose();
                OnPanelClosed?.Invoke(panel);
                DestroyIfNeeded(panel);
            }

            _overlayList.Clear();

#if UNITY_EDITOR
            Debug.Log("[UIManager] All overlays hidden.");
#endif
        }

        #endregion

        #region Public Methods — Reset

        /// <summary>
        /// 모든 패널을 파괴하고 레지스트리를 초기화합니다.
        /// BootScene 전환처럼 UI 상태를 완전히 리셋해야 할 때 호출하세요.
        /// </summary>
        public void ResetAll()
        {
            foreach (var panel in _panelRegistry.Values)
            {
                if (panel != null)
                {
                    Destroy(panel.gameObject);
                }
            }

            _panelStack.Clear();
            _overlayList.Clear();
            _panelRegistry.Clear();

#if UNITY_EDITOR
            Debug.Log("[UIManager] ResetAll — 모든 패널 파괴 및 레지스트리 초기화.");
#endif
        }

        #endregion

        #region Public Methods — Registry

        /// <summary>
        /// 타입으로 등록된 패널을 반환합니다.
        /// </summary>
        /// <typeparam name="T">조회할 패널 타입</typeparam>
        /// <returns>등록된 패널</returns>
        /// <exception cref="InvalidOperationException">패널이 등록되지 않은 경우</exception>
        public T Get<T>() where T : UIPanel
        {
            if (_panelRegistry.TryGetValue(typeof(T), out var panel))
            {
                return panel as T;
            }

            throw new InvalidOperationException($"[UIManager] 등록되지 않은 패널입니다: {typeof(T).Name}. Open()을 먼저 호출하세요.");
        }

        /// <summary>
        /// 해당 타입의 패널이 현재 열려 있는지 확인합니다.
        /// 등록되지 않은 패널은 false를 반환합니다.
        /// </summary>
        /// <typeparam name="T">확인할 패널 타입</typeparam>
        /// <returns>열려 있으면 true, 미등록이면 false</returns>
        public bool IsOpen<T>() where T : UIPanel
        {
            if (!_panelRegistry.TryGetValue(typeof(T), out var panel))
            {
                return false;
            }

            return panel.IsOpen;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// <see cref="UIPanel.DestroyOnClose"/>가 true인 패널을 레지스트리에서 제거하고 GameObject를 파괴합니다.
        /// 닫기 처리 이후에 호출하세요.
        /// </summary>
        /// <param name="panel">파괴 여부를 확인할 패널</param>
        private void DestroyIfNeeded(UIPanel panel)
        {
            if (!panel.DestroyOnClose)
            {
                return;
            }

            _panelRegistry.Remove(panel.GetType());
            Destroy(panel.gameObject);
        }

        /// <summary>
        /// 레지스트리에서 패널을 찾아 반환하거나, 없으면 <c>Resources/UI/</c>에서 로드 및 인스턴스화합니다.
        /// </summary>
        /// <typeparam name="T">패널 타입</typeparam>
        /// <param name="parent">인스턴스화 시 부모로 설정할 Canvas</param>
        /// <returns>등록된 또는 새로 생성된 패널</returns>
        /// <exception cref="InvalidOperationException">프리팹을 찾을 수 없거나 컴포넌트가 없는 경우</exception>
        private T GetOrCreate<T>(Canvas parent) where T : UIPanel
        {
            if (_panelRegistry.TryGetValue(typeof(T), out var existing))
            {
                return existing as T;
            }

            var prefab = Resources.Load<GameObject>($"{PanelResourcePath}/{typeof(T).Name}")
                ?? throw new InvalidOperationException($"[UIManager] 프리팹을 찾을 수 없습니다: Resources/{PanelResourcePath}/{typeof(T).Name}");

            var instance = Instantiate(prefab, parent != null ? parent.transform : transform);
            var panel = instance.GetComponent<T>()
                ?? throw new InvalidOperationException($"[UIManager] 프리팹에 {typeof(T).Name} 컴포넌트가 없습니다.");

            _panelRegistry[typeof(T)] = panel;
            return panel;
        }

        #endregion
    }
}
