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
    public enum Stetas
    {
        Normal,
        Attack
    }
    private Stetas stetas;
    private GameObject target;
    private RootStetas.LRStetas myStetas;
    private List<RootStetas> myRoot = new List<RootStetas>();
    private List<RootStetas> myLeftRoot = new List<RootStetas>();
    private List<RootStetas> myRightRoot = new List<RootStetas>();
    private Queue<GameObject> targetEnemy = new Queue<GameObject>();
    private GameObject targetRemenber;

    private void Awake()
    {
        targetRemenber = null;
        stetas = Stetas.Normal;
        if (transform.position.x <= 0) myStetas = RootStetas.LRStetas.Left;
        else myStetas = RootStetas.LRStetas.Right;
    }

    private void Start()
    {
        myLeftRoot = GameObject.FindGameObjectWithTag("Main").GetComponent<TestManeger>().LeftRoot;
        myRightRoot = GameObject.FindGameObjectWithTag("Main").GetComponent<TestManeger>().RightRoot;

        transform
            .ObserveEveryValueChanged(x => x.position.x)
            .Subscribe(x => 
            {
                if (x <= 0) myStetas = RootStetas.LRStetas.Left;
                else myStetas = RootStetas.LRStetas.Right;
            })
            .AddTo(gameObject);

        this.UpdateAsObservable()
            .Select(x => myStetas)
            .DistinctUntilChanged()
            .Subscribe(x => 
            {
                switch (myStetas)
                {
                    case RootStetas.LRStetas.Left:
                        myRoot = myLeftRoot;
                        break;
                    case RootStetas.LRStetas.Right:
                        myRoot = myRightRoot;
                        break;
                }
            })
            .AddTo(gameObject);

        switch (myStetas)
        {
            case RootStetas.LRStetas.Left:
                myRoot = GameObject.FindGameObjectWithTag("Main").GetComponent<TestManeger>().LeftRoot;
                break;
            case RootStetas.LRStetas.Right:
                myRoot = GameObject.FindGameObjectWithTag("Main").GetComponent<TestManeger>().RightRoot;
                break;
        }
        var currentRoot = new IntReactiveProperty(0);
        currentRoot
            .Where(x => x < myRoot.Count)
            .Subscribe(x => target = myRoot[x].rootObject);

        this.UpdateAsObservable()
            .Where(_=>target != null)
            .Where(_=>target.activeInHierarchy)
            .Subscribe(_ =>
            {
                transform.LookAt(target.transform);
                transform.position = Vector3.MoveTowards(this.transform.position, target.transform.position, 0.05f);
            },ex =>  Debug.Log(ex + ",向いて移動"));

        this.UpdateAsObservable()
            .Where(_ => target != null)
            .Where(_=>target.transform.position == transform.position)
            .Subscribe(_ =>
            {
                currentRoot.Value += 1;
            }, ex => Debug.Log(ex + ",次のやつ更新"));

        this.UpdateAsObservable()
            .Where(_ => target == null)
            .Subscribe(_ => TargetLockOn());
    }

    private void TargetLockOn()
    {
        if (targetEnemy.Count >= 1)
        {
            target = targetEnemy.Dequeue();
        }
        else if (targetEnemy.Count <= 0)
        {
            target = targetRemenber;
            targetRemenber = null;
            stetas = Stetas.Normal;
        }
    }
    private bool IsLooked(GameObject other)
    {
        float angle = 90f;
        if (Vector3.Angle(other.transform.position - this.transform.position, transform.forward) <= angle) return true;
        else return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent(typeof(IBuilding)) as IBuilding == null) return;
        if (targetRemenber == null)
        {
            targetRemenber = target;
        }
        if (!IsLooked(other.gameObject)) return;
        switch (stetas)
        {
            case Stetas.Normal:
                target = other.gameObject;
                stetas = Stetas.Attack;
                break;
            case Stetas.Attack:
                targetEnemy.Enqueue(other.gameObject);
                break;
        }
    }
}
