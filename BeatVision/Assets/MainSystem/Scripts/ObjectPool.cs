using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private Transform parentTrans;
    [SerializeField] private GameObject poolObject;
    private List<GameObject> poolObjList;
    [SerializeField] private int initMaxCount;

    void Awake()
    {
        parentTrans = transform;
        CreatePool();
    }

    private void CreatePool()
    {
        poolObjList = new List<GameObject>();
        for (int i = 0; i < initMaxCount; i++)
        {
            GameObject newObj = CreateNewObject();
            newObj.SetActive(false);
            poolObjList.Add(newObj);
        }
    }

    public GameObject GetObject()
    {
        foreach (GameObject obj in poolObjList)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        GameObject newObj = CreateNewObject();
        poolObjList.Add(newObj);

        newObj.SetActive(true);
        return newObj;
    }


    private GameObject CreateNewObject()
    {
        GameObject newObj = Instantiate(poolObject);
        newObj.transform.parent = parentTrans;

        return newObj;
    }
}
