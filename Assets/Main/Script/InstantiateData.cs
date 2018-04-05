using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateData : MonoBehaviour
{
    private int _prefabNumber;

    [SerializeField]
    private GameObject[] prefabs = new GameObject[0];
    [SerializeField]
    private InstantiateCheck instantiateCheck;

    public GameObject Prefab(int id)
    {
        return prefabs[id];
    }

    public int prefabNumber
    {
        get
        {
            return _prefabNumber;
        }
    }
    public void UnitChange(int num)
    {
        _prefabNumber = num;
    }

    public bool IsInstantiateCheck(Vector3 pos ,int id)
    {
        return instantiateCheck.IsInstantiateCheck(pos, id);
    }
}
