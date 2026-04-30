using System.Collections.Generic;

namespace SurvivalGame.Data
{
    /// <summary>크래프팅 레시피 재료 하나를 나타냅니다.</summary>
    [System.Serializable]
    public class RecipeIngredient
    {
        public int item_id;
        public int count;
    }

    /// <summary>
    /// 크래프팅 레시피 데이터입니다.
    /// Recipe.json 테이블에서 로드됩니다.
    /// </summary>
    public class RecipeData
    {
        /// <summary>레시피 고유 ID</summary>
        public int    recipe_id;

        /// <summary>레시피 이름 (UI 표시용)</summary>
        public string name;

        /// <summary>결과 아이템 ID</summary>
        public int    result_item_id;

        /// <summary>결과 아이템 수량</summary>
        public int    result_count;

        /// <summary>필요 작업대 타입 (0 = 맨손, 1 = 기본 작업대)</summary>
        public int    workbench_type;

        /// <summary>필요 재료 목록</summary>
        public List<RecipeIngredient> ingredients;
    }
}
