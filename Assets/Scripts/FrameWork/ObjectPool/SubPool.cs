using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SLua;
//对象池管理的单个池子。管理着同一个prefab的多个实例
[CustomLuaClass]
public class SubPool
{
    private GameObject objPrefab;
    public GameObject ObjPrefab
    {
        get { return objPrefab; }
    }
    //分配过的，现在加载在内存中的所有实例，不管对象现在是被回收状态（用active=false来标记），还是使用中状态（用active=true来标记）
    private List<GameObject> objInstances;
    public SubPool(GameObject objPrefab)
    {
        this.objPrefab = objPrefab;
        objInstances = new List<GameObject>();
    }
    //分配
    public GameObject Spawn(out bool isNewInstance)
    {
        foreach (GameObject go in objInstances)
        {
            if (go.activeInHierarchy == false)
            {
                //找到一个池中可分配物
                go.SetActive(true);           
                isNewInstance = false;
                return go;
            }
        }
        GameObject newGo = GameObject.Instantiate(objPrefab);
        objInstances.Add(newGo);
        isNewInstance = true;
        return newGo;
    }
    //回收
    public void UnSpawn(GameObject wantToUnspawnGo)
    {

        foreach (GameObject go in objInstances)
        {
            if (wantToUnspawnGo == go)
            {
                //回收之前先清洗干净
                //这里我给一个默认实现：重置pos，rotation。以后有问题来这里改
                go.transform.position = Vector3.zero;
                go.transform.rotation = Quaternion.identity;
                //go.SendMessage("OnUnSpawn", SendMessageOptions.DontRequireReceiver);
                //标记为false，就回收了
                go.SetActive(false);
            }
        }
    }
}
