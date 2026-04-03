using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI
{
    /// <summary>
    /// UIManager가 관리하는 UI 패널의 추상 기반 클래스입니다.
    /// Canvas 단위로 패널을 구성하며, Canvas와 GraphicRaycaster를 자동으로 관리합니다.
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public abstract class UIPanel : MonoBehaviour
    {
        #region Fields

        protected Canvas _canvas;
        protected GraphicRaycaster _raycaster;

        #endregion

        #region Properties

        /// <summary>패널이 현재 열려 있는지 여부입니다.</summary>
        public bool IsOpen { get; private set; }

        /// <summary>
        /// false이면 UIManager.Close() 및 UIManager.CloseAll()에서 이 패널을 건너뜁니다.
        /// 확인 팝업처럼 사용자가 명시적으로 응답해야 하는 패널에서 override하세요.
        /// </summary>
        public virtual bool CanClose => true;

        /// <summary>
        /// false이면 UIManager.HandleBack()에서 이 패널을 건너뜁니다.
        /// 뒤로가기로 닫히면 안 되는 패널에서 override하세요.
        /// </summary>
        public virtual bool CloseOnBack => true;

        /// <summary>
        /// true이면 닫힐 때 GameObject를 파괴하고 레지스트리에서 제거합니다.
        /// 온보딩, 패치노트처럼 한 번만 열리는 무거운 패널에서 override하세요.
        /// </summary>
        public virtual bool DestroyOnClose => false;

        #endregion

        #region Unity Lifecycle

        protected virtual void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _raycaster = GetComponent<GraphicRaycaster>();

            _canvas.enabled = false;
            _raycaster.enabled = false;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// UIManager.Open() 시 UIManager에 의해 호출됩니다.
        /// Canvas와 GraphicRaycaster를 활성화하고 <see cref="OnOpened"/>를 호출합니다.
        /// </summary>
        public void OnOpen()
        {
            IsOpen = true;
            _canvas.enabled = true;
            _raycaster.enabled = true;
            OnOpened();
        }

        /// <summary>
        /// UIManager.Close() 시 UIManager에 의해 호출됩니다.
        /// Canvas와 GraphicRaycaster를 비활성화하고 <see cref="OnClosed"/>를 호출합니다.
        /// </summary>
        public void OnClose()
        {
            IsOpen = false;
            _canvas.enabled = false;
            _raycaster.enabled = false;
            OnClosed();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// 패널이 열릴 때 호출됩니다. 서브클래스에서 override하여 등장 연출, 초기화 로직을 구현하세요.
        /// </summary>
        protected virtual void OnOpened() { }

        /// <summary>
        /// 패널이 닫힐 때 호출됩니다. 서브클래스에서 override하여 퇴장 연출, 정리 로직을 구현하세요.
        /// </summary>
        protected virtual void OnClosed() { }

        #endregion
    }
}
