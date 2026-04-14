using System.Collections;
using System.Collections.Generic;
using Framework.Core.DataManager;
using Framework.Core.ObjectPool;
using IdleGame.Data;
using UnityEngine;
using UnityEngine.AI;

namespace IdleGame.Battle
{
    /// <summary>
    /// 챕터 기준으로 몬스터 스폰 / 리스폰을 관리합니다.
    /// 항상 max_monster_count 수를 유지합니다.
    /// MainEntry에서 LoadChapter()를 호출하여 초기화하세요.
    /// </summary>
    public class StageManager : MonoBehaviour
    {
        #region Constants

        /// <summary>몬스터 프리팹 경로 형식 (Resources 기준, monster_id 치환)</summary>
        private const string PrefabPathFormat = "Game/Monsters/{0}";

        private const int SpawnRetryMax = 10;

        #endregion

        #region Private Fields

        private ChapterData             _chapterData;
        private PlayerController        _player;
        private Vector3                 _stageCenter;
        private List<MonsterController> _activeMonsters = new();
        private int                     _totalWeight;

        #endregion

        #region Public Methods

        /// <summary>
        /// 챕터를 로드하고 몬스터 스폰을 시작합니다.
        /// </summary>
        /// <param name="chapterId">로드할 챕터 ID</param>
        /// <param name="player">플레이어 참조</param>
        /// <param name="stageCenter">스폰 기준 중심 위치</param>
        public void LoadChapter(int chapterId, PlayerController player, Vector3 stageCenter)
        {
            _player      = player;
            _stageCenter = stageCenter;
            _chapterData = InGameDataManager.Instance.Get<ChapterData>(chapterId);

            _totalWeight = 0;
            foreach (var entry in _chapterData.monsters)
                _totalWeight += entry.spawn_weight;

            RegisterPools();
            FillMonsters();
        }

        /// <summary>
        /// 현재 챕터를 종료하고 모든 몬스터를 풀에 반환합니다.
        /// 챕터 전환 시 호출하세요.
        /// </summary>
        public void UnloadChapter()
        {
            StopAllCoroutines();

            foreach (var monster in _activeMonsters)
            {
                if (monster == null) continue;
                monster.OnDied -= OnMonsterDied;
                ObjectPoolManager.Instance.Release(PoolKey(monster.Data.monster_id), monster);
            }

            _activeMonsters.Clear();

            foreach (var entry in _chapterData.monsters)
                ObjectPoolManager.Instance.Remove(PoolKey(entry.monster_id));
        }

        #endregion

        #region Private Methods

        /// <summary>챕터에 등장하는 몬스터 종류별로 풀을 등록합니다.</summary>
        private void RegisterPools()
        {
            foreach (var entry in _chapterData.monsters)
            {
                string key    = PoolKey(entry.monster_id);
                string path   = string.Format(PrefabPathFormat, entry.monster_id);
                var    prefab = Resources.Load<MonsterController>(path);

                if (prefab == null)
                {
                    Debug.LogError($"[StageManager] 프리팹을 찾을 수 없습니다: {path}");
                    continue;
                }

                ObjectPoolManager.Instance.Register(key, prefab, _chapterData.max_monster_count);
            }
        }

        /// <summary>부족한 수만큼 몬스터를 채웁니다.</summary>
        private void FillMonsters()
        {
            int needed = _chapterData.max_monster_count - _activeMonsters.Count;
            for (int i = 0; i < needed; i++)
                SpawnOne();
        }

        /// <summary>몬스터 1마리를 스폰합니다.</summary>
        private void SpawnOne()
        {
            if (!TryGetSpawnPosition(out Vector3 spawnPos)) return;

            MonsterData data = PickMonsterData();
            if (data == null) return;

            var monster = ObjectPoolManager.Instance.Get<MonsterController>(PoolKey(data.monster_id));
            if (monster == null) return;

            monster.transform.position = spawnPos;
            monster.Initialize(data, _player);
            monster.OnDied += OnMonsterDied;

            _activeMonsters.Add(monster);
        }

        /// <summary>
        /// 스폰 위치를 결정합니다.
        /// 스테이지 중심 기준 spawn_radius 내 랜덤, 플레이어와 최소 거리 보장, NavMesh 위 유효 위치.
        /// </summary>
        private bool TryGetSpawnPosition(out Vector3 result)
        {
            float radius  = _chapterData.spawn_radius;
            float minDist = _chapterData.spawn_min_distance_from_player;

            for (int i = 0; i < SpawnRetryMax; i++)
            {
                Vector2 rand2D    = Random.insideUnitCircle * radius;
                Vector3 candidate = _stageCenter + new Vector3(rand2D.x, 0f, rand2D.y);

                if (_player != null)
                {
                    float distToPlayer = Vector3.Distance(candidate, _player.transform.position);
                    if (distToPlayer < minDist) continue;
                }

                if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                {
                    result = hit.position;
                    return true;
                }
            }

            result = Vector3.zero;
            Debug.LogWarning("[StageManager] 유효한 스폰 위치를 찾지 못했습니다. 다음 틱에 재시도합니다.");
            return false;
        }

        /// <summary>가중치 기반으로 MonsterData를 랜덤 선택합니다.</summary>
        private MonsterData PickMonsterData()
        {
            int roll = Random.Range(0, _totalWeight);
            int acc  = 0;

            foreach (var entry in _chapterData.monsters)
            {
                acc += entry.spawn_weight;
                if (roll < acc)
                    return InGameDataManager.Instance.Get<MonsterData>(entry.monster_id);
            }

            return null;
        }

        /// <summary>몬스터 사망 시 풀 반환 후 리스폰 예약합니다.</summary>
        private void OnMonsterDied(MonsterController monster)
        {
            monster.OnDied -= OnMonsterDied;
            _activeMonsters.Remove(monster);
            ObjectPoolManager.Instance.Release(PoolKey(monster.Data.monster_id), monster);

            StartCoroutine(RespawnAfterDelay(_chapterData.respawn_delay));
        }

        private IEnumerator RespawnAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            SpawnOne();
        }

        private static string PoolKey(int monsterId) => $"Monster_{monsterId}";

        #endregion
    }
}
