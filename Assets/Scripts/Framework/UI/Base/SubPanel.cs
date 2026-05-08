using UnityEngine;

namespace Framework.UI
{
    /// <summary>
    /// UIPanel의 자식으로 배치되는 서브 패널의 추상 기반 클래스입니다.
    /// <para>
    /// UIPanel이 Canvas 단위의 독립 패널이라면, SubPanel은 부모 UIPanel의 Canvas를 공유하는
    /// 하위 UI 구획입니다. UIManager가 관리하지 않으며 부모 UIPanel이 직접 Show/Hide를 호출합니다.
    /// </para>
    /// </summary>
    public abstract class SubPanel : MonoBehaviour
    {
        #region Properties

        /// <summary>현재 표시 중인지 여부입니다.</summary>
        public bool IsVisible { get; private set; }

        #endregion

        #region Unity Lifecycle

        protected virtual void Awake()
        {
            gameObject.SetActive(false);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// SubPanel을 표시합니다.
        /// </summary>
        public void Show()
        {
            IsVisible = true;
            gameObject.SetActive(true);
            OnShown();
        }

        /// <summary>
        /// SubPanel을 숨깁니다.
        /// </summary>
        public void Hide()
        {
            IsVisible = false;
            gameObject.SetActive(false);
            OnHidden();
        }

        /// <summary>
        /// 데이터가 변경되었을 때 부모가 호출하여 표시를 갱신합니다.
        /// SubPanel이 표시 중이 아닐 때 호출되어도 다음 Show() 시 반영됩니다.
        /// </summary>
        public void Refresh()
        {
            OnRefresh();
        }

        #endregion

        #region Protected Methods

        /// <summary>Show() 시 호출됩니다. 등장 연출·초기화 로직을 구현하세요.</summary>
        protected virtual void OnShown() { }

        /// <summary>Hide() 시 호출됩니다. 퇴장 연출·정리 로직을 구현하세요.</summary>
        protected virtual void OnHidden() { }

        /// <summary>Refresh() 시 호출됩니다. 데이터 갱신 로직을 구현하세요.</summary>
        protected virtual void OnRefresh() { }

        #endregion
    }
}
