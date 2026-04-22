using IdleGame.Data;

namespace Framework.Core.DataManager
{
    /// <summary>
    /// InGameDataManager 게임 전용 확장입니다.
    /// ExcelClassGenerator 자동 생성 파일과 동일한 partial class 패턴입니다.
    /// 새 데이터 추가 시 LoadAll()에 한 줄만 추가하면 됩니다.
    /// </summary>
    public partial class InGameDataManager
    {
        #region Path Constants

        private const string PathChapter = "Game/Data/Chapter";
        private const string PathMonster = "Game/Data/Monster";
        private const string PathItem    = "Game/Data/Item";
        private const string PathLevel   = "Game/Data/Level";
        private const string PathSkill   = "Game/Data/Skill";

        #endregion

        #region Public Methods

        /// <summary>
        /// 모든 게임 데이터를 캐시에 로드합니다.
        /// BootScene 초기화 시 한 번 호출하세요. 이후 Get&lt;T&gt;(id) 로 접근합니다.
        /// </summary>
        public void LoadAll()
        {
            LoadAsDictionary<ChapterData>(PathChapter, x => x.chapter_id);
            LoadAsDictionary<MonsterData>(PathMonster, x => x.monster_id);
            LoadAsDictionary<ItemData>(PathItem, x => x.item_id);
            LoadAsDictionary<LevelData>(PathLevel, x => x.level);
            LoadAsDictionary<SkillData>(PathSkill, x => x.skill_id);
        }

        #endregion
    }
}
