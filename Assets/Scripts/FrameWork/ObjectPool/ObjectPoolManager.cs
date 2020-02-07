using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SLua;
[CustomLuaClass]
public class ObjectPoolManager
{

    //用subPool的预制体来索引subPool
    private static Dictionary<GameObject, SubPool> pools = new Dictionary<GameObject, SubPool>();

    public static GameObject Spawn(GameObject prefab, out bool isNewInstance)
    {
        SubPool targetPool = null;
        bool hadThisPool = pools.TryGetValue(prefab, out targetPool);
        if (hadThisPool)
        {
            GameObject go = targetPool.Spawn(out isNewInstance);
            return go;
        }
        else
        {
            SubPool newPool = new SubPool(prefab);
            pools.Add(newPool.ObjPrefab, newPool);
            GameObject go = newPool.Spawn(out isNewInstance);
            return go;
        }


    }

    public static void UnSpawn(GameObject goInstance)
    {

        string goInstanceName = goInstance.name.Substring(0, goInstance.name.Count() - 7);
        foreach (GameObject prefab in pools.Keys)
        {

            if (prefab.name == goInstanceName)
            {
                pools[prefab].UnSpawn(goInstance);
            }
        }
    }

    private static void UnSpawnAll()
    {
        pools.Clear();
    }

    //当场景跳转，请清理对象池的所有instance，否则保留了空引用
    public static void Clear()
    {
        UnSpawnAll();
    }

}
