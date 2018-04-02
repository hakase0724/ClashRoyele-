using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class TestMove : MonoBehaviour
{
    public bool isMine { get; set; }
    private TestManeger maneger => GameObject.FindGameObjectWithTag("Main").GetComponent<TestManeger>();
    private List<GameObject> targets = new List<GameObject>();
    private GameObject target;
    // Use this for initialization
    void Start ()
    {
        isMine = true;
        targets = maneger.objects;
        target = SearchMinDistance(CalcDistance());
        const float unitSpeed = 10f;  
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                transform.LookAt(target.transform);
                GetComponent<Rigidbody>().velocity = transform.forward * unitSpeed;
            })
            .AddTo(gameObject);
    }

    private List<float> CalcDistance()
    {
        List<float> distances = new List<float>();
        float distance = 0f;
        foreach (var t in targets)
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

    public void NextSet(GameObject next)
    {
        target = next;
        Debug.Log(target.name);
    }
}
