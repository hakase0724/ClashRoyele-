using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class CenterPoint : TestPoint
{
    [SerializeField]
    private bool isEnemy;


    private void OnDestroy()
    {

    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (isEnemy) return;
    }
}
