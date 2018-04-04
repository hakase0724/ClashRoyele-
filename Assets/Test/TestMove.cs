using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.AI;
using static StaticUse;

public class TestMove : MonoBehaviour,IUnit
{
    public BoolReactiveProperty isMine { get; set; } = new BoolReactiveProperty();
    public FloatReactiveProperty unitHp { get; set; } = new FloatReactiveProperty();
    public float unitEnergy { get; set; }
    public float unitSpeed { get; set; }

    private int targetPointa = 0;
    private NavMeshAgent nav => GetComponent<NavMeshAgent>();
    private Animator anim => GetComponent<Animator>();
    private Rigidbody rb => GetComponent<Rigidbody>();
    private IDisposable updateStream;
    private IDisposable attackStream;
    private Subject<Unit> stopStream = new Subject<Unit>();
    [SerializeField]
    private GameObject[] targets = new GameObject[0];
    [SerializeField]
    private GameObject left;
    [SerializeField]
    private GameObject right;
    [SerializeField]
    private bool _IsMine;
    [SerializeField]
    private float _UnitHp;

    private void Awake()
    {
        
    }
    private  void Start()
    {
        isMine.Value = _IsMine;
        unitHp.Value = _UnitHp;
        unitSpeed = 3f;
        nav.autoBraking = false;
        LeftOrRight();
        Move();
        this.UpdateAsObservable()
            .Where(_ => !nav.pathPending)
            .Where(_ => nav.remainingDistance < 0.5f)
            .Subscribe(_ => Move())
            .AddTo(gameObject);
    }

    private void LeftOrRight()
    {
        if (CalcDistance(transform.position, left.transform.position) <= CalcDistance(transform.position, right.transform.position))
        {
            targets[0] = left;
        }
        else
        {
            targets[0] = right;
        }
    }

    public void Move()
    {
        if (targets.Length <= 0) return;
        if (targetPointa >= targets.Length) return;
        if (targets[targetPointa] == null) return;
        nav.destination = targets[targetPointa].transform.position;
        targetPointa++;
    }

    public void MyColor(int id)
    {
        
    }

    public void Attack(float attack, GameObject attackTarget)
    {
        const int attackInterval = 1;
        var a = attackTarget.GetComponent(typeof(IUnit)) as IUnit;
        attackStream = Observable.Interval(TimeSpan.FromSeconds(attackInterval))
            .TakeUntilDestroy(attackTarget)
            .Subscribe(_ => Attack(attack,a),() => Comp(attackStream))
            .AddTo(gameObject);
    }

    private void Attack(float attack, IUnit target)
    {
        target.Damage(attack);
        anim.SetBool("Attack", true);
        nav.speed = 0f;
    }

    private void Comp(IDisposable stream)
    {
        nav.destination = targets[targetPointa].transform.position;
        anim.SetBool("Attack", false);
        nav.speed = unitSpeed;
    }

    public void Damage(float damage)
    {
        unitHp.Value -= damage;
    }

    public void Death()
    {
        Destroy(gameObject);
    }

    private void TargetLockOn(GameObject target)
    {
        float targetDistance = 5f;
        const float angle = 30f;
        if (CalcDistance(transform.position, target.transform.position) > targetDistance * targetDistance) return;
        //if (!IsLooked(target.transform.position, transform.position, transform.forward, angle)) return;       
        GoToTarget(target);
    }

    private void GoToTarget(GameObject target)
    {
        nav.destination = target.transform.position;
        Attack(10f, target);
    }

    private void OnTriggerEnter(Collider other)
    {
        var otherUnit = other.GetComponent(typeof(IUnit)) as IUnit;
        if (otherUnit == null) return;
        //if (otherUnit.isMine.Value == isMine.Value) return;
        TargetLockOn(other.gameObject);
    }
}


