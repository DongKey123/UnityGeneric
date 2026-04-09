using System;
using System.Collections.Generic;

namespace IdleGame.Data
{
    /// <summary>챕터(필드) 정보 데이터입니다.</summary>
    [Serializable]
    public class ChapterData
    {
        /// <summary>챕터 고유 ID</summary>
        public int chapter_id;

        /// <summary>챕터 이름</summary>
        public string name;

        /// <summary>해금 최소 레벨 (0 = 제한 없음)</summary>
        public int unlock_level;

        /// <summary>해금 처치 수 조건 (이전 챕터 몬스터 기준)</summary>
        public int unlock_kill_count;

        /// <summary>방치 보상 — 시간당 골드 (온라인 기준 100%)</summary>
        public int idle_gold_per_hour;

        /// <summary>방치 보상 — 시간당 경험치 (온라인 기준 100%)</summary>
        public int idle_exp_per_hour;

        /// <summary>등장 몬스터 목록 (monster_id + 스폰 가중치)</summary>
        public List<MonsterSpawnEntry> monsters;
    }

    /// <summary>챕터 내 몬스터 스폰 항목입니다.</summary>
    [Serializable]
    public class MonsterSpawnEntry
    {
        /// <summary>MonsterData.monster_id 참조</summary>
        public int monster_id;

        /// <summary>스폰 가중치 (전체 합산 기준 상대 비율)</summary>
        public int spawn_weight;
    }
}
