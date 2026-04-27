using Framework.Core.DataManager;
using Framework.Core.EventBus;
using SurvivalGame.Data;
using SurvivalGame.Farming;
using SurvivalGame.Inventories;
using SurvivalGame.Player;
using UnityEngine;

namespace SurvivalGame.Battle
{
    /// <summary>
    /// 적을 스폰하고 사망 시 드롭을 처리하는 매니저입니다.
    /// EnemyData 테이블의 prefab_path로 프리팹을 로드하여 스폰합니다.
    /// 원점(0,0,0) 기준 SpawnRadius 반경 내 랜덤 위치에 배치합니다.
    /// 맵 시스템 완성 후 스폰 로직을 교체하세요.
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        #region Constants

        private const float SpawnRadius = 25f;

        #endregion

        #region Inspector

        [SerializeField] private int _spawnCountPerType = 3;

        #endregion

        #region Public Methods

        /// <summary>SurvivalEntry에서 호출합니다.</summary>
        public void Spawn(PlayerController player, Inventory playerInventory)
        {
            var allEnemies = InGameDataManager.Instance.GetAll<EnemyData>();

            foreach (var enemyData in allEnemies)
            {
                var prefab = Resources.Load<GameObject>(enemyData.prefab_path);
                if (prefab == null)
                {
                    Debug.LogWarning($"[EnemySpawner] 프리팹 없음: Resources/{enemyData.prefab_path}");
                    continue;
                }

                for (int i = 0; i < _spawnCountPerType; i++)
                {
                    var obj   = Instantiate(prefab, GetRandomPosition(), Quaternion.identity);
                    var enemy = obj.GetComponent<Enemy>();
                    if (enemy == null)
                    {
                        Debug.LogWarning($"[EnemySpawner] Enemy 컴포넌트 없음: {prefab.name}");
                        continue;
                    }

                    enemy.Initialize(enemyData, player, playerInventory);
                    SubscribeDrop(enemy, enemyData, playerInventory);
                }
            }
        }

        #endregion

        #region Private Methods

        private void SubscribeDrop(Enemy enemy, EnemyData data, Inventory inventory)
        {
            EventBus.Subscribe<EnemyDiedEvent>(OnEnemyDied);

            void OnEnemyDied(EnemyDiedEvent e)
            {
                if (e.Source != enemy) return;

                EventBus.Unsubscribe<EnemyDiedEvent>(OnEnemyDied);

                if (data.drop_item_id <= 0) return;

                var itemData = InGameDataManager.Instance.Get<SurvivalItemData>(data.drop_item_id);
                if (itemData == null) return;

                int count = Random.Range(data.drop_count_min, data.drop_count_max + 1);
                inventory.TryAdd(itemData, count);

                EventBus.Publish(new ResourceHarvestedEvent
                {
                    ItemName = itemData.name,
                    Count    = count
                });
            }
        }

        private Vector3 GetRandomPosition()
        {
            Vector2 circle = Random.insideUnitCircle * SpawnRadius;
            return new Vector3(circle.x, 0f, circle.y);
        }

        #endregion
    }
}
