using System;

namespace SurvivalGame.Data
{
    /// <summary>자원 오브젝트 데이터입니다.</summary>
    [Serializable]
    public class ResourceData
    {
        /// <summary>자원 고유 ID</summary>
        public int resource_id;

        /// <summary>채집 시 획득하는 아이템 ID</summary>
        public int item_id;

        /// <summary>자원 이름</summary>
        public string name;

        /// <summary>최대 내구도 (채집 횟수)</summary>
        public int durability_max;

        /// <summary>채집 완료 후 재생성까지 대기 시간 (초)</summary>
        public float respawn_time;

        /// <summary>1회 채집 시 최소 획득 수량</summary>
        public int drop_count_min;

        /// <summary>1회 채집 시 최대 획득 수량</summary>
        public int drop_count_max;

        /// <summary>프리팹 경로 (Resources 기준)</summary>
        public string prefab_path;
    }
}
