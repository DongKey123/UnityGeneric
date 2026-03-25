using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core.ObjectPool
{
    /// <summary>
    /// ObjectPoolManager에서 타입 무관하게 풀을 관리하기 위한 내부 인터페이스입니다.
    /// </summary>
    internal interface IObjectPool
    {
        void Clear();
    }

    /// <summary>
    /// MonoBehaviour 컴포넌트를 재사용하는 제네릭 오브젝트 풀입니다.
    /// 풀이 비어있으면 자동으로 새 오브젝트를 생성합니다.
    /// </summary>
    public class ObjectPool<T> : IObjectPool where T : MonoBehaviour, IPoolable
    {
        #region Fields

        private readonly T _prefab;
        private readonly Transform _parent;
        private readonly Queue<T> _pool;

        #endregion

        #region Properties

        /// <summary>
        /// 현재 풀에 대기 중인 오브젝트 수입니다.
        /// </summary>
        public int CountInactive => _pool.Count;

        #endregion

        #region Constructor

        /// <summary>
        /// 오브젝트 풀을 생성합니다.
        /// </summary>
        /// <param name="prefab">풀링할 프리팹</param>
        /// <param name="initialSize">초기 생성 수</param>
        /// <param name="parent">생성된 오브젝트의 부모 Transform (null이면 루트)</param>
        public ObjectPool(T prefab, int initialSize = 10, Transform parent = null)
        {
            _prefab = prefab;
            _parent = parent;
            _pool = new Queue<T>(initialSize);

            for (int i = 0; i < initialSize; i++)
            {
                Enqueue(Create());
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 풀에서 오브젝트를 꺼냅니다. 풀이 비어있으면 새로 생성합니다.
        /// </summary>
        public T Get()
        {
            T item = _pool.Count > 0 ? _pool.Dequeue() : Create();
            item.transform.SetParent(null);
            item.gameObject.SetActive(true);
            item.OnSpawn();
            return item;
        }

        /// <summary>
        /// 오브젝트를 풀로 반환합니다.
        /// </summary>
        public void Release(T item)
        {
            item.OnDespawn();
            Enqueue(item);
        }

        /// <summary>
        /// 풀에 대기 중인 오브젝트를 모두 파괴하고 풀을 비웁니다. 사용 중인 오브젝트는 호출자가 직접 처리해야 합니다.
        /// </summary>
        public void Clear()
        {
            while (_pool.Count > 0)
            {
                Object.Destroy(_pool.Dequeue().gameObject);
            }
        }

        #endregion

        #region Private Methods

        private T Create()
        {
            T item = Object.Instantiate(_prefab, _parent);
            item.gameObject.SetActive(false);
            return item;
        }

        private void Enqueue(T item)
        {
            item.transform.SetParent(_parent);
            item.gameObject.SetActive(false);
            _pool.Enqueue(item);
        }

        #endregion
    }
}
