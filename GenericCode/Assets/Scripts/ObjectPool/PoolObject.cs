using UnityEngine;
using System.Collections;

public abstract class PoolObject : MonoBehaviour , IPoolable
{
    public virtual void OnDespawnObject()
    {

    }

    public virtual void OnSpawnPoolObject()
    {

    }
}
