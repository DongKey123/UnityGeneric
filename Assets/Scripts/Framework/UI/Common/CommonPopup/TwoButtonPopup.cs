using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI
{
    /// <summary>
    /// 확인/취소 버튼 두 개짜리 팝업입니다.
    /// 삭제, 구매 등 사용자 선택이 필요한 경우에 사용합니다.
    /// </summary>
    public class TwoButtonPopup : PopupBase
    {
        #region Fields

        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _messageText;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private TMP_Text _confirmButtonText;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private TMP_Text _cancelButtonText;

        private Action _onConfirm;
        private Action _onCancel;
        private Action _onClose;

        #endregion

        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();
            _confirmButton.onClick.AddListener(OnConfirmClicked);
            _cancelButton.onClick.AddListener(OnCancelClicked);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 팝업 내용을 설정합니다. CommonPopupManager.ShowTwoButton()에 의해 호출됩니다.
        /// </summary>
        /// <param name="title">팝업 제목</param>
        /// <param name="message">팝업 본문 메시지</param>
        /// <param name="confirmText">확인 버튼 텍스트</param>
        /// <param name="cancelText">취소 버튼 텍스트</param>
        /// <param name="onConfirm">확인 버튼 클릭 콜백</param>
        /// <param name="onCancel">취소 버튼 클릭 콜백</param>
        /// <param name="onClose">팝업 닫기 액션 (CommonPopupManager가 주입)</param>
        public void Setup(string title, string message, string confirmText, string cancelText,
            Action onConfirm, Action onCancel, Action onClose)
        {
            _titleText.text = title;
            _messageText.text = message;
            _confirmButtonText.text = confirmText;
            _cancelButtonText.text = cancelText;
            _onConfirm = onConfirm;
            _onCancel = onCancel;
            _onClose = onClose;
        }

        #endregion

        #region Event Handlers

        private void OnConfirmClicked()
        {
            _onClose?.Invoke();
            _onConfirm?.Invoke();
        }

        private void OnCancelClicked()
        {
            _onClose?.Invoke();
            _onCancel?.Invoke();
        }

        #endregion
    }
}
