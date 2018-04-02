﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UniRx;
using UniRx.Triggers;
using System.Linq;

public class NavTest : MonoBehaviour
{
    public float Speed = 1.0f;
    private ReactiveProperty<GameObject> target = new ReactiveProperty<GameObject>();
    private List<GameObject> targets = new List<GameObject>();
    private NavMeshAgent nav => GetComponent<NavMeshAgent>();
	// Use this for initialization
	void Start ()
    {
        var maneger = GameObject.FindGameObjectWithTag("Main").GetComponent<TestManeger>();
        targets = maneger.objects;
        target.Value = SearchMinDistance(CalcDistance());
        target
            .Subscribe(x => nav.SetDestination(x.transform.position));

        this.UpdateAsObservable()
            .Where(_ => target.Value == null)
            .Subscribe(_ => target.Value = SearchMinDistance(CalcDistance()));

        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.T))
            .Subscribe(_ => Debug.Log(target.Value));      
    }

    private List<float> CalcDistance()
    {
        List<float> distances = new List<float>();
        float distance = 0f;
        foreach(var t in targets)
        {
            distance = (this.transform.position - t.transform.position).sqrMagnitude;
            distances.Add(distance);
        }
        return distances;
    }

    private float CalcDistance(Vector3 pos)
    {
        return (this.transform.position - pos).sqrMagnitude;
    }

    private GameObject SearchMinDistance(List<float> distances)
    {
        float ignoreDistance = 2f;
        var minIdx = distances
            .Select((val, idx) => new { V = val, I = idx })
            .Where((min, working) => min.V > ignoreDistance * ignoreDistance)
            .Aggregate((min, working) => (min.V < working.V) ? min : working).I;
        return targets[minIdx];
    }

    public void TriggerOn(GameObject other)
    {
        if (targets.Select(x => x).Any(x => x == other))
        {
            Debug.Log(other.name);
            target.Value = SearchMinDistance(CalcDistance());
        }       
    }

    public void Attack(GameObject other)
    {
        if (other.tag == "Enemy") target.Value = other;
    }
}