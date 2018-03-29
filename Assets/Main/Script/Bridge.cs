using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 橋
/// </summary>
public class Bridge : MonoBehaviour, IBuilding
{
    private void Start()
    {
        EnterTransform();
    }

    public void EnterTransform()
    {
        GameObject.FindGameObjectWithTag("Main").GetComponent<BulidingsManeger>().EnterList(this.transform, this);
    }
}
