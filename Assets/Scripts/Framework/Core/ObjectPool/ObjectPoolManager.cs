using System.Collections.Generic;
using UnityEngine;
using Framework.Core.Singleton;

namespace Framework.Core.ObjectPool
{
    /// <summary>
    /// 이름(key) 기반으로 오브젝트 풀을 중앙에서 관리하는 매니저입니다.
    /// 같은 프리팹이라도 이름을 다르게 지정해 독립적인 풀로 관리할 수 있습니다.
    /// 씬에 배치하지 않아도 자동 생성되며, 씬 전환 후에도 유지됩니다.
    /// </summary>
    public class ObjectPoolManager : PersistentMonoSingleton<ObjectPoolManager>
    {
        #region Fields

        private readonly Dictionary<string, IObjectPool> _pools = new();
        private readonly Dictionary<string, GameObject> _containers = new();

        #endregion

        #region Public Methods

        /// <summary>
        /// 풀을 등록합니다. 같은 이름이 이미 등록된 경우 무시합니다.
        /// </summary>
        /// <param name="key">풀 식별 이름</param>
        /// <param name="prefab">풀링할 프리팹</param>
        /// <param name="initialSize">초기 생성 수</param>
        public void Register<T>(string key, T prefab, int initialSize = 10) where T : MonoBehaviour, IPoolable
        {
            if (_pools.ContainsKey(key))
            {
                Debug.LogWarning($"[ObjectPoolManager] '{key}' 풀이 이미 등록되어 있습니다.");
                return;
            }

            var container = new GameObject($"{key}Pool");
            container.transform.SetParent(transform);
            _pools[key] = new ObjectPool<T>(prefab, initialSize, container.transform);
            _containers[key] = container;
        }

        /// <summary>
        /// 풀에서 오브젝트를 꺼냅니다.
        /// </summary>
        /// <param name="key">풀 식별 이름</param>
        /// <returns>풀에서 꺼낸 오브젝트. 등록되지 않은 키면 null 반환</returns>
        public T Get<T>(string key) where T : MonoBehaviour, IPoolable
        {
            if (_pools.TryGetValue(key, out var pool))
            {
                return ((ObjectPool<T>)pool).Get();
            }

            Debug.LogWarning($"[ObjectPoolManager] '{key}' 풀이 등록되어 있지 않습니다. Register()를 먼저 호출하세요.");
            return null;
        }

        /// <summary>
        /// 오브젝트를 풀로 반환합니다.
        /// </summary>
        /// <param name="key">풀 식별 이름</param>
        /// <param name="item">반환할 오브젝트</param>
        public void Release<T>(string key, T item) where T : MonoBehaviour, IPoolable
        {
            if (_pools.TryGetValue(key, out var pool))
            {
                ((ObjectPool<T>)pool).Release(item);
                return;
            }

            Debug.LogWarning($"[ObjectPoolManager] '{key}' 풀이 등록되어 있지 않습니다.");
        }

        /// <summary>
        /// 특정 풀을 제거합니다.
        /// </summary>
        /// <param name="key">풀 식별 이름</param>
        public void Remove(string key)
        {
            if (!_pools.TryGetValue(key, out var pool))
            {
                Debug.LogWarning($"[ObjectPoolManager] '{key}' 풀이 등록되어 있지 않습니다.");
                return;
            }

            pool.Clear();
            _pools.Remove(key);

            if (_containers.TryGetValue(key, out var container))
            {
                Destroy(container);
                _containers.Remove(key);
            }
        }

        /// <summary>
        /// 모든 풀을 제거합니다. 풀에 대기 중인 오브젝트만 파괴되며, 사용 중인 오브젝트는 호출자가 직접 처리해야 합니다.
        /// </summary>
        public void ClearAll()
        {
            foreach (var pool in _pools.Values)
            {
                pool.Clear();
            }

            foreach (var container in _containers.Values)
            {
                Destroy(container);
            }

            _pools.Clear();
            _containers.Clear();
        }

        #endregion
    }
}
