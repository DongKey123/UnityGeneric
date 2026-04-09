using System;

namespace IdleGame.Data
{
    /// <summary>레벨별 필요 경험치 및 스탯 증가량 데이터입니다.</summary>
    [Serializable]
    public class LevelData
    {
        /// <summary>레벨</summary>
        public int level;

        /// <summary>다음 레벨까지 필요 경험치</summary>
        public int required_exp;

        /// <summary>이 레벨에서 증가하는 최대 체력</summary>
        public int hp_increase;

        /// <summary>이 레벨에서 증가하는 공격력</summary>
        public int atk_increase;

        /// <summary>이 레벨에서 증가하는 방어력</summary>
        public int def_increase;
    }
}
