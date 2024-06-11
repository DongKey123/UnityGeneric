using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolDemo : MonoBehaviour
{
    [SerializeField] private DemoObject demoObjectOrigin = null;
    private GameObjectPool<DemoObject> demoObjectPool = null;

    private WaitForSeconds spawnTime = new WaitForSeconds(0.1f);
    private WaitForSeconds despawnTime = new WaitForSeconds(0.2f);

    private Stack<DemoObject> demoObjects = null;

    private void Awake()
    {
        demoObjects = new Stack<DemoObject>();
        demoObjectPool = GameObjectPoolManager.getInstance.GetPool(demoObjectOrigin);
    }

    private void Start()
    {
        StartCoroutine(SpwanObject());
        StartCoroutine(DeSpwanObject());
    }

    IEnumerator SpwanObject()
    {
        while(true)
        {
            float xPos = UnityEngine.Random.Range(1f, 10f);
            float zPos = UnityEngine.Random.Range(1f, 10f);

            var spawnObject = demoObjectPool.GetObject();
            spawnObject.transform.position = new Vector3(xPos, 0, zPos);
            demoObjects.Push(spawnObject);
            yield return spawnTime;
        }
    }

    IEnumerator DeSpwanObject()
    {
        while (true)
        {
            if (demoObjects.Count > 0)
            {
                var despawnObject = demoObjects.Pop();
                demoObjectPool.ReturnObject(despawnObject);
                yield return despawnTime;
            }
            else
                yield return null;
        }
    }
}
