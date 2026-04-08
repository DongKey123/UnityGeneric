namespace Framework.UI
{
    /// <summary>토스트 메시지 요청 데이터입니다.</summary>
    public class ToastRequest
    {
        /// <summary>표시할 메시지</summary>
        public string Message { get; }

        /// <summary>토스트 타입</summary>
        public ToastType Type { get; }

        /// <summary>표시 유지 시간 (초)</summary>
        public float Duration { get; }

        /// <param name="message">표시할 메시지</param>
        /// <param name="type">토스트 타입</param>
        /// <param name="duration">표시 유지 시간 (초)</param>
        public ToastRequest(string message, ToastType type, float duration)
        {
            Message = message;
            Type = type;
            Duration = duration;
        }
    }
}
