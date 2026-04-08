using UnityEngine;

namespace Framework.UI
{
    /// <summary>
    /// CommonPopupManager가 관리하는 팝업의 추상 기반 클래스입니다.
    /// UIPanel과 달리 Canvas를 공유하며, SetActive로 표시/숨김을 처리합니다.
    /// </summary>
    public abstract class PopupBase : MonoBehaviour
    {
        #region Properties

        /// <summary>팝업이 현재 열려 있는지 여부입니다.</summary>
        public bool IsOpen { get; private set; }

        #endregion

        #region Unity Lifecycle

        protected virtual void Awake()
        {
            gameObject.SetActive(false);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// CommonPopupManager.Show() 시 CommonPopupManager에 의해 호출됩니다.
        /// GameObject를 활성화하고 <see cref="OnOpened"/>를 호출합니다.
        /// </summary>
        public void OnOpen()
        {
            IsOpen = true;
            gameObject.SetActive(true);
            OnOpened();
        }

        /// <summary>
        /// CommonPopupManager.Close() 시 CommonPopupManager에 의해 호출됩니다.
        /// GameObject를 비활성화하고 <see cref="OnClosed"/>를 호출합니다.
        /// </summary>
        public void OnClose()
        {
            IsOpen = false;
            gameObject.SetActive(false);
            OnClosed();
        }

        #endregion

        #region Protected Methods

        /// <summary>팝업이 열릴 때 호출됩니다. 서브클래스에서 override하여 초기화 로직을 구현하세요.</summary>
        protected virtual void OnOpened() { }

        /// <summary>팝업이 닫힐 때 호출됩니다. 서브클래스에서 override하여 정리 로직을 구현하세요.</summary>
        protected virtual void OnClosed() { }

        #endregion
    }
}
