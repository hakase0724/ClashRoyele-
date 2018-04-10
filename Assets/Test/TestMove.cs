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
    public float unitEnergy { get; set; } = 1;
    public float unitSpeed { get; set; }
    public float maxUnitHp { get {return _UnitHp; } }
    public byte unitId { get; set; }

    private int targetPointa = 0;
    private TargetGet targetGet => Camera.main.GetComponent<TargetGet>();
    private NavMeshAgent nav => GetComponent<NavMeshAgent>();
    private Animator anim => GetComponent<Animator>();
    private Rigidbody rb => GetComponent<Rigidbody>();
    private Queue<GameObject> targetQueue = new Queue<GameObject>();
    //向かう場所左→右の順で格納している
    private List<Vector3> targets = new List<Vector3>();
    
    [SerializeField]
    private float _UnitHp;
    [SerializeField, Tooltip("自分と相手のときそれぞれの色")]
    private Color[] color = new Color[0];
    [SerializeField, Tooltip("攻撃力")]
    private float attackPower;
    [SerializeField, Tooltip("ロックオンする距離")]
    private float targetDistance;
    [SerializeField, Tooltip("ユニットの移動速度")]
    private float _UnitSpeed;

    private void Awake()
    {
        PhotonNetwork.OnEventCall += this.OnEvent;
    }
    private void OnEnable()
    {
        nav.enabled = false;
    }
    private  void Start()
    {
        nav.enabled = true;
        unitHp.Value = _UnitHp;
        //自分が味方か敵かで対象を変える
        if (isMine.Value) TargetPosSet(targetGet.mArray);
        else TargetPosSet(targetGet.eArray);
        //LeftOrRight();       
        unitSpeed = _UnitSpeed;
        //目的地に近づいたときに減速しないようにする
        nav.autoBraking = false;

        //動き出しを指定時間遅らせる
        int waitTime = 2;
        var startTimer = Observable.Timer(TimeSpan.FromSeconds(waitTime));

        this.UpdateAsObservable()
            .SkipUntil(startTimer)
            .Where(_ => !nav.pathPending)
            //対象との距離が0.5未満になったら到着したと判断する
            .Where(_ => nav.remainingDistance < 0.5f)
            .Subscribe(_ => Move())
            .AddTo(gameObject);

        unitHp
            .Where(x => x <= 0)
            .Subscribe(x => Death())
            .AddTo(gameObject);

        Observable.Interval(TimeSpan.FromSeconds(10))
            .Subscribe(_ => PhotonNetwork.RaiseEvent(unitId, transform.position, true, null))
            .AddTo(gameObject);
    }

    private void TargetPosSet(GameObject[] targetObjects)
    {
        foreach (var p in targetObjects)
        {
            targets.Add(p.transform.position);
        }
    }

    /// <summary>
    /// 左右どちらに行くか決定する
    /// </summary>
    private void LeftOrRight()
    {
        //左の最初の点
        float left = CalcDistance(transform.position, targets.First());
        //右の最初の点
        float right = CalcDistance(transform.position, targets.Last());
        //右のほうが近ければリストの中身を反転させる
        if (left > right) targets.Reverse();
    }

    private void OnEvent(byte evId, object content, int senderId)
    {
        if (PhotonNetwork.isMasterClient) return;      
        if (unitId != evId) return;
        var pos = (Vector3)content;
        if (CalcDistance(transform.position, pos) <= 5f) return;
        nav.Warp(-pos);
    }


    public void Move()
    {
        if (targets.Count <= 0) return;
        if (targetPointa >= targets.Count) return;
        if (targets[targetPointa] == null)
        {
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
        if (!isMine.Value) transform.rotation = Quaternion.Euler(0, 180, 0);    
    }

    public void Attack(float attack, GameObject attackTarget)
    {
        Debug.Log("攻撃");
        const int attackInterval = 1;
        var a = attackTarget.GetComponent(typeof(IUnit)) as IUnit;
        Observable.Interval(TimeSpan.FromSeconds(attackInterval))
            .TakeUntilDestroy(attackTarget)
            .Subscribe(_ => Attack(attack,a),() => Comp())
            .AddTo(gameObject);
    }

    /// <summary>
    /// 実際の攻撃処理
    /// </summary>
    /// <param name="attack">攻撃力</param>
    /// <param name="target">対象</param>
    private void Attack(float attack, IUnit target)
    {
        target.Damage(attack);
        anim.SetBool("Attack", true);
        nav.speed = 0f;
    }

    /// <summary>
    /// 敵を倒した時の処理
    /// </summary>
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
        Debug.Log("ダメージを受けた" + unitHp.Value);
    }

    public void Death()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// 対象との距離を計算しロックオンする距離内であればロックオンする
    /// </summary>
    /// <param name="target">対象</param>
    private void TargetLockOn(GameObject target)
    {
        Debug.Log("ターゲットロックオン");
        if (CalcDistance(transform.position, target.transform.position) > targetDistance * targetDistance)
        {
            Debug.Log("ロックオン範囲にいない");
            return;
        }
        if (targetQueue.Count <= 0)
        {
            Debug.Log("ターゲットへ向かう");
            GoToTarget(target);
            targetQueue.Enqueue(target);
        }
        else
        {
            Debug.Log("ターゲットを記憶");
            targetQueue.Enqueue(target);
        }
    }

    /// <summary>
    /// 対象に向かうことができる状態ならば向かう
    /// </summary>
    /// <param name="target">対象</param>
    private void GoToTarget(GameObject target)
    {
        Debug.Log("ターゲットへ向かう");
        if (nav.pathStatus != NavMeshPathStatus.PathInvalid) nav.destination = target.transform.position;
        Attack(attackPower, target);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("トリガーオン");
        var otherUnit = other.GetComponent(typeof(IUnit)) as IUnit;
        if (otherUnit == null) return;
        //同一生成者なら無視する
        if (otherUnit.isMine.Value == isMine.Value) return;
        TargetLockOn(other.gameObject);
        nav.speed = 0f;
    }
    /// <summary>
    /// 位置と体力を同期する
    /// マスタークライアントが基準
    /// </summary>
    /// <param name="pos">同期する位置</param>
    /// <param name="nowHp">同期する体力</param>
    [PunRPC]
    public void Sync(Vector3 pos,float nowHp)
    {
        if (PhotonNetwork.isMasterClient) return;
        if (CalcDistance(transform.position, pos) <= 10) return;
        nav.Warp(new Vector3(-pos.x,pos.y,-pos.z));
        unitHp.Value = nowHp;
    }
}


