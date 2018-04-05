using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.AI;
using UnityEngine.UI;
using static StaticUse;

public class TestMove : Photon.MonoBehaviour,IUnit
{
    public BoolReactiveProperty isMine { get; set; } = new BoolReactiveProperty();
    public FloatReactiveProperty unitHp { get; set; } = new FloatReactiveProperty();
    public float unitEnergy { get; set; }
    public float unitSpeed { get; set; }
    public float maxUnitHp { get {return _UnitHp; } }

    private int targetPointa = 0;
    private TargetGet targetGet => Camera.main.GetComponent<TargetGet>();
    private NavMeshAgent nav => GetComponent<NavMeshAgent>();
    private Animator anim => GetComponent<Animator>();
    private Rigidbody rb => GetComponent<Rigidbody>();
    private Queue<GameObject> targetQueue = new Queue<GameObject>();
    private List<Vector3> targets = new List<Vector3>();
    
    [SerializeField]
    private float _UnitHp;
    [SerializeField, Tooltip("自分と相手のときそれぞれの色")]
    private Color[] color = new Color[0];

    private void OnEnable()
    {
        nav.enabled = false;
    }
    private  void Start()
    {
        nav.enabled = true;
        unitHp.Value = _UnitHp;
        //自分が味方か敵かで対象を変える
        if (isMine.Value) targets.AddRange(targetGet.mine);
        else targets.AddRange(targetGet.enemys);
        LeftOrRight();       
        unitSpeed = 3f;
        //目的地に近づいたときに減速しないようにする
        nav.autoBraking = false;

        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.T))
            .Subscribe(_ =>
            {
                if (targets.Count <= 0) Debug.Log("ないよ");
                foreach (var m in targets)
                {
                    Debug.Log(m);
                }
            });

        //動き出しを指定時間遅らせる
        const int waitTime = 2;
        var startTimer = Observable.Timer(TimeSpan.FromSeconds(waitTime));

        this.UpdateAsObservable()
            .SkipUntil(startTimer)
            .Where(_ => !nav.pathPending)
            .Where(_ => nav.remainingDistance < 0.5f)
            .Subscribe(_ => Move())
            .AddTo(gameObject);

        unitHp
            .Where(x => x <= 0)
            .Subscribe(x => Death())
            .AddTo(gameObject);

        ////位置同期間隔（秒）
        //int syncTime = 3;
        //Observable.Interval(TimeSpan.FromSeconds(syncTime))
        //    .Subscribe(_ => photonView.RPC(("Sync"), PhotonTargets.AllViaServer, transform.position,unitHp.Value))
        //    .AddTo(gameObject);
    }
    /// <summary>
    /// 左右どちらに行くか決定する
    /// </summary>
    private void LeftOrRight()
    {
        float left = CalcDistance(transform.position, targets[0]);
        float right = CalcDistance(transform.position, targets[targets.Count - 1]);
        if (left > right) targets.Reverse();
    }
    

    public void Move()
    {
        if (targets.Count <= 0) return;
        if (targetPointa >= targets.Count) return;
        if (targets[targetPointa] == null)
        {
            nav.Stop();
            anim.SetBool("Attack", true);
            return;
        }
        if (!anim.enabled) anim.enabled = true;
        if (!nav.enabled) nav.enabled = true;
        nav.destination = targets[targetPointa];
        targetPointa++;
    }

    public void MyColor(int id)
    {
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
        int colorNumber;
        //生成者が自分か相手か判別
        if (IsSameId(id, PhotonNetwork.player.ID))
        {
            colorNumber = 0;
            isMine.Value = true;
        }
        else
        {
            colorNumber = 1;
            isMine.Value = false;
        }
        //判別結果に応じて識別番号を更新
        foreach (Renderer renderer in renderers)
        {
            renderer.material.color = color[colorNumber];
        }       
    }

    public void Attack(float attack, GameObject attackTarget)
    {
        const int attackInterval = 1;
        var a = attackTarget.GetComponent(typeof(IUnit)) as IUnit;
        Observable.Interval(TimeSpan.FromSeconds(attackInterval))
            .TakeUntilDestroy(attackTarget)
            .Subscribe(_ => Attack(attack,a),() => Comp())
            .AddTo(gameObject);
    }

    private void Attack(float attack, IUnit target)
    {
        target.Damage(attack);
        anim.SetBool("Attack", true);
        nav.speed = 0f;
    }

    private void Comp()
    {
        if (targetQueue.Count >= 1) targetQueue.Dequeue();
        if (targetQueue.Count >= 1) GoToTarget(targetQueue.Peek());
        else
        {
            if (targets.Count <= targetPointa) return;
            if (nav.pathStatus != NavMeshPathStatus.PathInvalid)
            {
                nav.destination = targets[targetPointa];
            }
            anim.SetBool("Attack", false);
            nav.speed = unitSpeed;
        }
       
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
        if (CalcDistance(transform.position, target.transform.position) > targetDistance * targetDistance) return;
        if (targetQueue.Count <= 0)
        {
            GoToTarget(target);
            targetQueue.Enqueue(target);
        }
        else
        {
            targetQueue.Enqueue(target);
        }
    }

    /// <summary>
    /// 対象に向かうことができる状態ならば向かう
    /// </summary>
    /// <param name="target">対象</param>
    private void GoToTarget(GameObject target)
    {
        if (nav.pathStatus != NavMeshPathStatus.PathInvalid)
        {
            nav.destination = target.transform.position;
        }       
        Attack(10f, target);
    }

    private void OnTriggerEnter(Collider other)
    {
        var otherUnit = other.GetComponent(typeof(IUnit)) as IUnit;
        if (otherUnit == null) return;
        if (otherUnit.isMine.Value == isMine.Value) return;
        TargetLockOn(other.gameObject);
        nav.speed = 0f;
    }

    [PunRPC]
    public void Sync(Vector3 pos,float nowHp)
    {
        if (PhotonNetwork.isMasterClient) return;
        nav.Warp(new Vector3(-pos.x,pos.y,-pos.z));
        unitHp.Value = nowHp;
    }
}


