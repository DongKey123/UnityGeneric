using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> : IPool where T : Component , IPoolable  ,new()
{
    private Stack<T> pool = null;
    private T originPrefab = default;

    private const int defaultPoolSize = 10;

    public ObjectPool(T prefab, int poolSize = defaultPoolSize)
    {
        pool = new Stack<T>();
        
        originPrefab = prefab;
        InitializePool(poolSize);
    }

    private void InitializePool(int size)
    {
        for ( int i =0; i < size; i++)
        {
            T obj = Object.Instantiate<T>(originPrefab);
            obj.gameObject.SetActive(false);
            pool.Push(obj);
        }
    }

    private T GetObject()
    {
        T obj;
        if (pool.Count > 0)
        {
            obj = pool.Pop();
            obj.OnSpawnPoolObject();

            return obj;
        }

        obj = Object.Instantiate<T>(originPrefab);
        return obj;
    }

    private void ReturnObject(T obj)
    {
        obj.OnDespawnObject();
        obj.gameObject.SetActive(false);

        pool.Push(obj);
    }
}
