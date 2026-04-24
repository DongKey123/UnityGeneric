namespace SurvivalGame.Farming
{
    /// <summary>플레이어가 자원 오브젝트 채집 범위에 진입했을 때 발행됩니다.</summary>
    public struct HarvestRangeEnteredEvent
    {
        /// <summary>범위 안에 들어온 자원 오브젝트</summary>
        public ResourceObject Resource;
    }

    /// <summary>플레이어가 자원 오브젝트 채집 범위에서 벗어났을 때 발행됩니다.</summary>
    public struct HarvestRangeExitedEvent
    {
        /// <summary>범위를 벗어난 자원 오브젝트</summary>
        public ResourceObject Resource;
    }

    /// <summary>채집이 완료되어 아이템이 드롭될 때 발행됩니다.</summary>
    public struct ResourceHarvestedEvent
    {
        /// <summary>획득한 아이템 이름</summary>
        public string ItemName;

        /// <summary>획득한 수량</summary>
        public int Count;
    }
}
