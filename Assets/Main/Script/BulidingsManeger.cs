using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulidingsManeger : MonoBehaviour
{
    public List<Transform> bulidingsTransform { get; private set; } = new List<Transform>();

    public void EnterList(Transform enterTransform,Component compornent)
    {
        if (compornent is IBuilding)
        {
            bulidingsTransform.Add(enterTransform);
            Debug.Log("登録" + enterTransform);
        }
    }
}
