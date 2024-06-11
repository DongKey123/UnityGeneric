using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : LazySingleton<ObjectPoolManager>
{
    private Dictionary<Type, IPool> poolDictionary = null;

    ObjectPoolManager()
    {
        poolDictionary = new Dictionary<System.Type, IPool>();

        UnityEngine.SceneManagement.SceneManager.sceneUnloaded += ClearPools;
    }

    ~ObjectPoolManager()
    {
        poolDictionary.Clear();

        poolDictionary = null;
    }

    public ObjectPool<T> GetPool<T>(T prefab) where T : Component, IPoolable, new()
    {
        ObjectPool<T> pool = null;

        System.Type type = typeof(T);

        if (poolDictionary.ContainsKey(type))
        {
            return poolDictionary[type] as ObjectPool<T>;
        }

        pool = new ObjectPool<T>(prefab);
        poolDictionary.Add(type, pool as IPool);

        return pool;
    }

    private void ClearPools(UnityEngine.SceneManagement.Scene arg0)
    {
        poolDictionary.Clear();
    }
}
