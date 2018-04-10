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
    /// <summary>
    /// ユニットの情報を格納
    /// </summary>
    public class MyData
    {
        public Vector3 myPos;
        public float myHp;
        public bool animBool = false;

        public MyData(Vector3 pos,float hp)
        {
            myPos = pos;
            myHp = hp;
        }

        public void DataSet(Vector3 pos, float hp)
        {
            myPos = pos;
            myHp = hp;
        }

        public void DataSet(Vector3 pos, float hp,bool animBool)
        {
            myPos = pos;
            myHp = hp;
            this.animBool = animBool;
        }
    }
    private MyData myData;
    //アニメーションのbool値
    private bool animBool = false;
    public BoolReactiveProperty isMine { get; set; } = new BoolReactiveProperty();
    public FloatReactiveProperty unitHp { get; set; } = new FloatReactiveProperty();
    public float unitEnergy { get; set; } = 1;
    public float unitSpeed { get; set; }
    public float maxUnitHp { get {return _UnitHp; } }
    public byte unitId { get; set; }

    //対象を格納している配列のポインタ
    private int targetPointa = 0;
    private TargetGet targetGet => Camera.main.GetComponent<TargetGet>();
    private NavMeshAgent nav => GetComponent<NavMeshAgent>();
    private Animator anim => GetComponent<Animator>();
    //攻撃対象を格納する
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
        myData = new MyData(transform.position, _UnitHp);
    }
    private void OnEnable()
    {
        //生成位置によって瞬間移動するのを防ぐ　詳しい原因は不明暫定対応　要調査
        nav.enabled = false;
        unitSpeed = _UnitSpeed;
        unitHp.Value = _UnitHp;
    }
    private  void Start()
    {
        nav.enabled = true;       
        //自分が味方か敵かで対象を変える
        if (isMine.Value) TargetPosSet(targetGet.mArray);
        else TargetPosSet(targetGet.eArray);
        LeftOrRight();            
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

        //対応するオブジェクトと同期をする　現在は3秒に一回同期している
        Observable.Interval(TimeSpan.FromSeconds(3))
            .Subscribe(_ => PhotonNetwork.RaiseEvent(unitId, transform.position, true, null))
            .AddTo(gameObject);
    }

    private void RaiseEvent()
    {
        myData.DataSet(transform.position, unitHp.Value,animBool);
        PhotonNetwork.RaiseEvent(unitId, myData, true, null);
    }

    /// <summary>
    /// 対象のGameObject型配列からVector3型リストを作成する
    /// </summary>
    /// <param name="targetObjects">対象のGameObject型配列</param>
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

    /// <summary>
    /// PUNのRaiseEventのdelegateに登録する関数
    /// </summary>
    /// <param name="evId">識別番号</param>
    /// <param name="content">通信の中身</param>
    /// <param name="senderId">送ってきた相手の番号</param>
    private void OnEvent(byte evId, object content, int senderId)
    {
        if (PhotonNetwork.isMasterClient) return;      
        if (unitId != evId) return;
        var data = (MyData)content;
        nav.Warp(-data.myPos);
        unitHp.Value = data.myHp;
        anim.SetBool("Attack", data.animBool);
    }


    public void Move()
    {
        if (targets.Count <= 0) return;
        if (targetPointa >= targets.Count) return;
        if (targets[targetPointa] == null)
        {
            animBool = true;
            anim.SetBool("Attack", animBool);
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
        const int attackInterval = 1;
        var a = attackTarget.GetComponent(typeof(IUnit)) as IUnit;
        Observable.Interval(TimeSpan.FromSeconds(attackInterval))
            .TakeUntilDestroy(attackTarget)
            .Do(_=>Debug.Log(attackTarget))
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
        animBool = true;
        anim.SetBool("Attack", animBool);
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
            animBool = false;
            anim.SetBool("Attack", animBool);
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

    /// <summary>
    /// 対象との距離を計算しロックオンする距離内であればロックオンする
    /// </summary>
    /// <param name="target">対象</param>
    private void TargetLockOn(GameObject target)
    {
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
        if (target == null) return;
        if (nav.pathStatus != NavMeshPathStatus.PathInvalid) nav.destination = target.transform.position;
        Attack(attackPower, target);
    }

    private void OnTriggerEnter(Collider other)
    {
        var otherUnit = other.GetComponent(typeof(IUnit)) as IUnit;
        if (otherUnit == null) return;
        //同一生成者なら無視する
        if (otherUnit.isMine.Value == isMine.Value) return;
        TargetLockOn(other.gameObject);
        nav.speed = 0f;
    }
}


