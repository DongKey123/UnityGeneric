using Framework.Core.DataManager;
using SurvivalGame.Data;
using SurvivalGame.Inventories;
using UnityEngine;

namespace SurvivalGame.Farming
{
    /// <summary>
    /// 자원 오브젝트를 스폰하는 매니저입니다.
    /// ResourceData 테이블의 prefab_path로 프리팹을 로드하여 스폰합니다.
    /// 원점(0,0,0) 기준 SpawnRadius 반경 내 랜덤 위치에 배치합니다.
    /// 맵 시스템 완성 후 스폰 로직을 교체하세요.
    /// </summary>
    public class ResourceSpawner : MonoBehaviour
    {
        #region Constants

        private const float SpawnRadius = 20f;

        #endregion

        #region Inspector

        [SerializeField] private int _spawnCountPerType = 3;

        #endregion

        #region Public Methods

        /// <summary>SurvivalEntry에서 인벤토리를 넘겨 스폰을 시작합니다.</summary>
        public void Spawn(Inventory inventory)
        {
            var allResources = InGameDataManager.Instance.GetAll<ResourceData>();

            foreach (var resourceData in allResources)
            {
                var prefab = Resources.Load<GameObject>(resourceData.prefab_path);
                if (prefab == null)
                {
                    Debug.LogWarning($"[ResourceSpawner] 프리팹 없음: Resources/{resourceData.prefab_path}");
                    continue;
                }

                var itemData = InGameDataManager.Instance.Get<SurvivalItemData>(resourceData.item_id);
                if (itemData == null)
                {
                    Debug.LogWarning($"[ResourceSpawner] ItemData 없음 — item_id: {resourceData.item_id}");
                    continue;
                }

                for (int i = 0; i < _spawnCountPerType; i++)
                {
                    var obj = Instantiate(prefab, GetRandomPosition(), Quaternion.identity);
                    var resource = obj.GetComponent<ResourceObject>();
                    resource.Initialize(resourceData, itemData, inventory);
                }
            }
        }

        #endregion

        #region Private Methods

        private Vector3 GetRandomPosition()
        {
            Vector2 circle = Random.insideUnitCircle * SpawnRadius;
            return new Vector3(circle.x, 0f, circle.y);
        }

        #endregion
    }
}
