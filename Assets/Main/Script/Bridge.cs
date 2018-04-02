using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using static StaticUse;

/// <summary>
/// 橋
/// </summary>
public class Bridge : MonoBehaviour, IBuilding
{
    private Main main => GameObject.FindGameObjectWithTag("Main").GetComponent<Main>();
    private BulidingsManeger maneger => GameObject.FindGameObjectWithTag("Main").GetComponent<BulidingsManeger>();

    private void Start()
    {
        EnterTransform();
    }

    public void EnterTransform()
    {
        
    }

    public void ReleaseTransform()
    {
        
    }
}
