using SurvivalGame.Defines;

namespace SurvivalGame.Core
{
    /// <summary>
    /// 재화 수량이 변경됐을 때 발행됩니다.
    /// UI에서 구독하여 표시값을 갱신하세요.
    /// </summary>
    public struct CurrencyChangedEvent
    {
        /// <summary>변경된 재화 종류</summary>
        public CurrencyType Type;

        /// <summary>변경 후 수량</summary>
        public int Amount;

        /// <summary>변경량 (양수: 획득, 음수: 소비)</summary>
        public int Delta;
    }
}
