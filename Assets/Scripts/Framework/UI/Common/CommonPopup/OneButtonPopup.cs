using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI
{
    /// <summary>
    /// 확인 버튼 하나짜리 팝업입니다.
    /// 알림, 안내 메시지처럼 단순 확인만 필요한 경우에 사용합니다.
    /// </summary>
    public class OneButtonPopup : PopupBase
    {
        #region Fields

        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _messageText;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private TMP_Text _confirmButtonText;

        private Action _onConfirm;
        private Action _onClose;

        #endregion

        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();
            _confirmButton.onClick.AddListener(OnConfirmClicked);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 팝업 내용을 설정합니다. CommonPopupManager.ShowOneButton()에 의해 호출됩니다.
        /// </summary>
        /// <param name="title">팝업 제목</param>
        /// <param name="message">팝업 본문 메시지</param>
        /// <param name="buttonText">확인 버튼 텍스트</param>
        /// <param name="onConfirm">확인 버튼 클릭 콜백</param>
        /// <param name="onClose">팝업 닫기 액션 (CommonPopupManager가 주입)</param>
        public void Setup(string title, string message, string buttonText, Action onConfirm, Action onClose)
        {
            _titleText.text = title;
            _messageText.text = message;
            _confirmButtonText.text = buttonText;
            _onConfirm = onConfirm;
            _onClose = onClose;
        }

        #endregion

        #region Event Handlers

        private void OnConfirmClicked()
        {
            _onClose?.Invoke();
            _onConfirm?.Invoke();
        }

        #endregion
    }
}
