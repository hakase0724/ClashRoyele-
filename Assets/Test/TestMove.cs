using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.AI;
using static StaticUse;

public class TestMove : MonoBehaviour
{
    private Animator anim => GetComponent<Animator>();
    [SerializeField]
    private GameObject target;
    void Start()
    {
        var nav = GetComponent<NavMeshAgent>();
        nav.SetDestination(target.transform.position);
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.S))
            .Subscribe(_ => 
            {
                anim.enabled = false;
                nav.Stop();
            }
            );
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.R))
            .Subscribe(_ =>
            {
                anim.enabled = true;
                nav.Resume();
            }
            );
    }
}


