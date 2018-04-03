using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UniRx;
using UniRx.Triggers;
using System.Linq;
using static StaticUse;
using System;

/// <summary>
/// ユニット
/// </summary>
public class PlayerUnit : Photon.MonoBehaviour, IUnit
{
    public enum Stetas
    {
        Normal,//通常
        Attack //攻撃
    }   
    public BoolReactiveProperty isMine { get; set; } = new BoolReactiveProperty();
    public BoolReactiveProperty isAlive { get; set; } = new BoolReactiveProperty(true);
    public FloatReactiveProperty UnitHp { get; set; } = new FloatReactiveProperty(100);
    public float UnitEnergy { get; set; } = 1;
    //自身の状態
    private Stetas stetas;
    //自分が左右どちらにいるか
    private RootStetas.LRStetas myStetas;
    //自分の進行ルート
    private List<RootStetas> myRoot = new List<RootStetas>();
    private Queue<GameObject> targetEnemy = new Queue<GameObject>();
    private GameObject targetRemenber;
    private IntReactiveProperty currentRoot = new IntReactiveProperty(1);
    private GameObject stageScript => GameObject.FindGameObjectWithTag("Main");
    //次に狙う対象
    private GameObject target;
    private List<GameObject> targets = new List<GameObject>();
    private List<IDisposable> updateStreams = new List<IDisposable>();
    //ユニットの移動速度
    private float unitSpeed;
    private Rigidbody rb => GetComponent<Rigidbody>();
    private Animator anim => GetComponent<Animator>();

    [SerializeField,Tooltip("自分と相手のときそれぞれの色")]
    private Color[] color = new Color[0];
    [SerializeField,Tooltip("ユニットの体力")]
    private float hp = 100;

    private void Awake()
    {
        UnitHp.Value = hp;
        targetRemenber = null;
        anim.enabled = true;
        stetas = Stetas.Normal;      
    }

    private void Start()
    {
        if (transform.position.x <= 0) myStetas = RootStetas.LRStetas.Left;
        else myStetas = RootStetas.LRStetas.Right;
        //自分のステータスによってルートを決める
        if (isMine.Value)
        {
            switch (myStetas)
            {
                case RootStetas.LRStetas.Left:
                    myRoot = stageScript.GetComponent<BulidingsManeger>().LeftRoot;
                    break;
                case RootStetas.LRStetas.Right:
                    myRoot = stageScript.GetComponent<BulidingsManeger>().RightRoot;
                    break;
            }
        }

        if (!isMine.Value)
        {
            switch (myStetas)
            {
                case RootStetas.LRStetas.Left:
                    myRoot = stageScript.GetComponent<BulidingsManeger>().LeftEnemyRoot;
                    break;
                case RootStetas.LRStetas.Right:
                    myRoot = stageScript.GetComponent<BulidingsManeger>().RightEnemyRoot;
                    break;
            }
        }

        UnitHp
           .Where(x => x <= 0)
           .Subscribe(x => Death());

        currentRoot
            .Where(x => x < myRoot.Count)
            .Subscribe(x => target = myRoot[x].rootObject);
    }
    /// <summary>
    /// ターゲットをロックオンしなおす
    /// </summary>
    private void TargetLockOn()
    {
        if (targetEnemy.Count >= 1)
        {
            target = targetEnemy.Dequeue();
        }
        else if (targetEnemy.Count <= 0)
        {
            //以前まで狙っていたターゲットがいた場合そのターゲットをロックオンする
            if (targetRemenber == null) targetRemenber = myRoot[currentRoot.Value].rootObject;
            target = targetRemenber;
            targetRemenber = null;
            stetas = Stetas.Normal;
        }
    }

    public void Move()
    {       
        //到達したと判断する距離
        var reachDistance = 0.5f;
        unitSpeed = 0.05f;
        updateStreams.Add
            (
            //ターゲットがいて、生きていればターゲットへ向かう
            this.UpdateAsObservable()
                .Where(_ => target != null)
                .Where(_ => target.activeInHierarchy)
                .Subscribe(_ =>
                {
                    transform.LookAt(target.transform);
                    transform.position = Vector3.MoveTowards(this.transform.position, target.transform.position, unitSpeed);
                }, ex => Debug.Log(ex + ",向いて移動"))
            );

        updateStreams.Add
            (
            //ターゲットに到達したとき次のターゲットへ移行する
            this.UpdateAsObservable()
                .Where(_ => target != null)
                .Where(_ => CalcDistance(transform.position,target.transform.position) <= reachDistance)
                .Subscribe(_ =>
                {
                    currentRoot.Value += 1;
                }, ex => Debug.Log(ex + ",次のやつ更新"))
            );

        updateStreams.Add
            (
            //ターゲットがいなくなった時ターゲットをロックオンしなおす
             this.UpdateAsObservable()
                .Where(_ => target == null)
                .Subscribe(_ => TargetLockOn())
            ); 
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
        //動き出しを指定時間遅らせる
        const int waitTime = 2;
        Observable.Timer(System.TimeSpan.FromSeconds(waitTime))
            .Subscribe(_ => Move());
    }

    public void Attack(float attack, GameObject attackTarget)
    {
        //攻撃間隔
        const int attackInterval = 1;
        //攻撃対象のIUnit付きコンポーネントを取得
        var attackTargetInterface = attackTarget.GetComponent(typeof(IUnit)) as IUnit;
        if (attackTargetInterface.isMine.Value == isMine.Value) return;
        //攻撃時には動きを止める
        AllDispose(updateStreams);
        rb.velocity = Vector3.zero;
        //攻撃間隔ごとに攻撃し相手のDamageメソッドを呼び出す
        var intervalStream = Observable.Interval(System.TimeSpan.FromSeconds(attackInterval))
            .Subscribe(_ => attackTargetInterface.Damage(attack))
            .AddTo(gameObject);
        //対象の体力を監視し体力がなくなったら攻撃をやめ、移動する
        attackTargetInterface.UnitHp
            .Where(x => x <= 0)
            .Subscribe(_ =>
            {
                intervalStream.Dispose();
                TargetLockOn();
                Move();
            })
            .AddTo(gameObject);
    }

    public void Damage(float damage)
    {
        UnitHp.Value -= damage;
    }

    public void Death()
    {
        isAlive.Value = false;
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        var otherUnit = other.gameObject.GetComponent(typeof(IUnit)) as IUnit;
        if (otherUnit == null) return;
        if (otherUnit.isMine.Value == isMine.Value) return;
        if (targetRemenber == null) targetRemenber = target;
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
    private bool IsLooked(GameObject other)
    {
        float angle = 90f;
        if (Vector3.Angle(other.transform.position - this.transform.position, transform.forward) <= angle) return true;
        else return false;
    }


    private void OnCollisionEnter(Collision other)
    {
        var otherComponent = other.gameObject.GetComponent(typeof(IUnit)) as IUnit;
        if (otherComponent == null) return;
        if (otherComponent.isMine.Value != isMine.Value) Attack(10f, other.gameObject);
        else AllDispose(updateStreams);
    }

    private void OnCollisionStay(Collision other)
    {
        var otherComponent = other.gameObject.GetComponent(typeof(IUnit)) as IUnit;
        if (otherComponent == null) return;
        if (otherComponent.isMine.Value == isMine.Value)
        {
            switch (myStetas)
            {
                case RootStetas.LRStetas.Left:
                    rb.velocity = transform.right;
                    break;
                case RootStetas.LRStetas.Right:
                    rb.velocity = -transform.right;
                    break;
            }
        }      
    }

    private void OnCollisionExit(Collision other)
    {
        var otherComponent = other.gameObject.GetComponent(typeof(IUnit)) as IUnit;
        if (otherComponent == null) return;
        if (otherComponent.isMine.Value == isMine.Value) Move();
    }
}
