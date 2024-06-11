using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool<T> : IPool where T : Component , IPoolable  ,new()
{
    private Stack<T> pool = null;
    private T originPrefab = default;

    public GameObjectPool(T prefab, int poolSize)
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

    public T GetObject()
    {
        T obj;
        if (pool.Count > 0)
        {
            obj = pool.Pop();
            obj.gameObject.SetActive(true);
            obj.OnSpawnPoolObject();

            return obj;
        }

        obj = Object.Instantiate<T>(originPrefab);
        return obj;
    }

    public void ReturnObject(T obj)
    {
        obj.OnDespawnObject();
        obj.gameObject.SetActive(false);

        pool.Push(obj);
    }
}
