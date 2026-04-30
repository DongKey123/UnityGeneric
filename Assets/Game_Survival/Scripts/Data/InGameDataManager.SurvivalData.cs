using Framework.Core.DataManager;

namespace SurvivalGame.Data
{
    /// <summary>
    /// 서바이벌 게임 데이터 로더입니다.
    /// InGameDataManager를 사용하여 서바이벌 게임 전용 데이터를 로드합니다.
    /// </summary>
    public static class SurvivalDataLoader
    {
        #region Path Constants

        private const string PathSurvivalItem     = "Data/Item";
        private const string PathSurvivalResource = "Data/Resource";
        private const string PathSurvivalEnemy    = "Data/Enemy";
        private const string PathSurvivalBuilding = "Data/Building";
        private const string PathSurvivalRecipe   = "Data/Recipe";

        #endregion

        #region Public Methods

        /// <summary>
        /// 서바이벌 게임 데이터를 모두 로드합니다.
        /// SurvivalEntry 초기화 시 호출하세요.
        /// </summary>
        public static void LoadAll()
        {
            InGameDataManager.Instance.LoadAsDictionary<SurvivalItemData>(PathSurvivalItem,     x => x.item_id);
            InGameDataManager.Instance.LoadAsDictionary<ResourceData>    (PathSurvivalResource, x => x.resource_id);
            InGameDataManager.Instance.LoadAsDictionary<EnemyData>       (PathSurvivalEnemy,    x => x.enemy_id);
            InGameDataManager.Instance.LoadAsDictionary<BuildingData>    (PathSurvivalBuilding, x => x.building_id);
            InGameDataManager.Instance.LoadAsDictionary<RecipeData>      (PathSurvivalRecipe,   x => x.recipe_id);
        }

        #endregion
    }
}
