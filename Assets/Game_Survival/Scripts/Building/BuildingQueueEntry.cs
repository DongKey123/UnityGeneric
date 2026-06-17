using SurvivalGame.Data;

namespace SurvivalGame.Building
{
    public enum BuildingState { Pending, Placed }

    /// <summary>건물 제작 큐의 항목 하나입니다. 배치 대기 → 설치됨 상태로 전이됩니다.</summary>
    public class BuildingQueueEntry
    {
        public BuildingState  State          { get; private set; }
        public BuildingData   Data           { get; }
        public PlacedBuilding PlacedInstance { get; private set; }

        public BuildingQueueEntry(BuildingData data)
        {
            Data  = data;
            State = BuildingState.Pending;
        }

        public void MarkPlaced(PlacedBuilding instance)
        {
            State           = BuildingState.Placed;
            PlacedInstance  = instance;
        }

        public void MarkPending()
        {
            State          = BuildingState.Pending;
            PlacedInstance = null;
        }
    }
}
