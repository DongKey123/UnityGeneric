using System.Collections.Generic;

namespace SurvivalGame.Data
{
    /// <summary>건물 하나의 재료 비용입니다.</summary>
    [System.Serializable]
    public class BuildingCost
    {
        public int item_id;
        public int count;
    }

    /// <summary>
    /// 건물 데이터입니다.
    /// Building.json 테이블에서 로드됩니다.
    /// </summary>
    public class BuildingData
    {
        public int              building_id;
        public string           name;
        public string           prefab_path;
        public int              grid_width;
        public int              grid_height;
        public List<BuildingCost> costs;
    }
}
