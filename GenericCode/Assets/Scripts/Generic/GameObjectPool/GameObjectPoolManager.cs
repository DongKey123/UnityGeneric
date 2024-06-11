using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPoolManager : LazySingleton<GameObjectPoolManager>
{
    private Dictionary<Type, IPool> poolDictionary = null;
    private const int defaultPoolSize = 10;

    GameObjectPoolManager()
    {
        poolDictionary = new Dictionary<System.Type, IPool>();

        UnityEngine.SceneManagement.SceneManager.sceneUnloaded += ClearPools;
    }

    ~GameObjectPoolManager()
    {
        poolDictionary.Clear();

        poolDictionary = null;
    }

    public GameObjectPool<T> GetPool<T>(T prefab,int poolSize = defaultPoolSize) where T : Component, IPoolable, new()
    {
        GameObjectPool<T> pool = null;

        System.Type type = typeof(T);

        if (poolDictionary.ContainsKey(type))
        {
            return poolDictionary[type] as GameObjectPool<T>;
        }

        pool = new GameObjectPool<T>(prefab, poolSize);
        poolDictionary.Add(type, pool as IPool);

        return pool;
    }

    private void ClearPools(UnityEngine.SceneManagement.Scene arg0)
    {
        poolDictionary.Clear();
    }
}
