using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.AI;

public class TestMove : MonoBehaviour
{
    [SerializeField]
    private GameObject game;
    [SerializeField]
    private GameObject target;

    private GameObject targetRemenber;

    private void Start()
    {
        this.UpdateAsObservable()
            .Where(_=>target.activeInHierarchy)
            .Subscribe(_ =>
            {
                transform.LookAt(target.transform);
                transform.position = Vector3.MoveTowards(this.transform.position, target.transform.position, 0.05f);
            });
        this.UpdateAsObservable()
            .Where(_=>target.transform.position == transform.position)
            .Subscribe(_ =>
            {
                if(target.GetComponent<Point>() != null) target = target.GetComponent<Point>().next;
            });
    }

    private void Update()
    {
        if (target == null)
        {
            target = targetRemenber;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent(typeof(IBuilding)) as IBuilding == null) return;
        targetRemenber = target;
        target = other.gameObject;
    }
}
