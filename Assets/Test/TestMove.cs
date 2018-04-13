using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AI;
using static StaticUse;

public class TestMove : Photon.MonoBehaviour, IUnit
{
    /// <summary>
    /// ユニットの情報を格納
    /// </summary>
    public class MyData
    {
        //ユニットの位置
        public Vector3 myPos;
        //ユニットの体力
        public float myHp;
        //ユニットのアニメーションフラグ
        public bool animBool = false;
        public MyData(Vector3 pos, float hp, bool animBool)
        {
            myPos = pos;
            myHp = hp;
            this.animBool = animBool;
        }
        public MyData(Vector3 pos, float hp)
        {
            myPos = pos;
            myHp = hp;
        }
        public void DataSet(Vector3 pos, float hp, bool animBool)
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
    public float maxUnitHp { get { return _UnitHp; } }
    public byte unitId { get; set; }

    //対象を格納している配列のポインタ
    private int targetPointa = 0;
    private TargetGet targetGet => Camera.main.GetComponent<TargetGet>();
    private NavMeshAgent nav;
    private Animator anim => GetComponent<Animator>();
    //攻撃対象を格納する
    private List<GameObject> targetList = new List<GameObject>();
    //向かう場所左→右の順で格納している
    private List<Vector3> targets = new List<Vector3>();
    //オブジェクトの生存状態
    private bool isAlive = true;

    [SerializeField, Tooltip("自分の体力")]
    private float _UnitHp;
    [SerializeField, Tooltip("自分と相手のときそれぞれの色")]
    private Color[] color = new Color[0];
    [SerializeField, Tooltip("攻撃力")]
    private float attackPower;
    [SerializeField, Tooltip("ロックオンする距離")]
    private float targetDistance;
    [SerializeField, Tooltip("ユニットの移動速度")]
    private float _UnitSpeed;
    [SerializeField, Tooltip("生成後の待機時間")]
    private int waitTime;

    private void Awake()
    {
        PhotonNetwork.OnEventCall += this.OnEvent;
        myData = new MyData(transform.position, _UnitHp);
        nav = GetComponent<NavMeshAgent>();
    }
    private void OnEnable()
    {
        //生成位置によって瞬間移動するのを防ぐ　詳しい原因は不明暫定対応　要調査
        nav.enabled = false;
        unitSpeed = _UnitSpeed;
        unitHp.Value = _UnitHp;
    }
    private void Start()
    {
        //PhotonViewのviewIDを手動割り当て(俗にいう違法建築)
        GetComponent<PhotonView>().viewID = unitId;
        nav.enabled = true;
        //自分が味方か敵かで対象を変える
        if (isMine.Value) targets.AddRange(targetGet.mArray);
        else targets.AddRange(targetGet.eArray);
        LeftOrRight();
        //目的地に近づいたときに減速しないようにする
        nav.autoBraking = false;
        //動き出しを指定時間遅らせる
        var startTimer = Observable.Timer(TimeSpan.FromSeconds(waitTime));
        this.UpdateAsObservable()
            .SkipUntil(startTimer)
            //navが有効でなければ
            .Where(_=> nav != null)
            //経路検索が終わっていなければ
            .Where(_ => !nav.pathPending)
            //対象との距離が0.5未満になったら到着したと判断する
            .Where(_ => nav.remainingDistance < 0.5f)
            .Subscribe(_ => Move())
            .AddTo(gameObject);

        unitHp
            .Where(x => x <= 0)
            .Subscribe(x => Death())
            .AddTo(gameObject);

        const int syncTime = 2;
        //対応するオブジェクトと同期をする　
        //現在は2秒に一回同期している 
        //マスタークライアントのみ送信する
        Observable.Interval(TimeSpan.FromSeconds(syncTime))
            .Where(_ => PhotonNetwork.isMasterClient)
            .Subscribe(_ => RaiseEvent())
            .AddTo(gameObject);
    }

    /// <summary>
    /// 同期するために必要なデータを送る
    /// </summary>
    private void RaiseEvent()
    {
        //myDataにデータをセット
        myData.DataSet(transform.position, unitHp.Value, animBool);
        //データを送るため送るデータをobject型の配列に格納
        //送るデータは4種類
        object[] content = new object[4];
        content[0] = myData.myPos;
        content[1] = myData.myHp;
        content[2] = myData.animBool;
        content[3] = targetPointa;
        PhotonNetwork.RaiseEvent(unitId, content, true, null);
    }

    /// <summary>
    /// PUNのRaiseEventのdelegateに登録する関数
    /// </summary>
    /// <param name="evId">識別番号</param>
    /// <param name="content">通信の中身</param>
    /// <param name="senderId">送ってきた相手の番号</param>
    private void OnEvent(byte evId, object content, int senderId)
    {
        //マスタークライアントであれば何もしない
        if (PhotonNetwork.isMasterClient) return;
        //受け取ったIDが自分のものと異なれば違うオブジェクトとして何もしない
        if (unitId != evId) return;
        //受け取ったデータをobject型の配列に変換
        object[] obj = (object[])content;
        //変換した配列の中身をMyData型に変換
        var data = new MyData((Vector3)obj[0], (float)obj[1], (bool)obj[2]);
        //Nullチェック
        if (!isAlive) return;
        if (nav == null) return;
        //受け取ったデータを元に自分の情報を更新する
        //距離の差が2より大きい場合のみ場所を合わせる
        if (CalcDistance(transform.position, -data.myPos) >= 2f) nav.Warp(-data.myPos);
        //受け取ったデータとそのデータに対応する自分のデータが異なるとき更新する
        //体力
        if (unitHp.Value != data.myHp) unitHp.Value = data.myHp;
        //アニメーションフラグ
        if (animBool != data.animBool) AnimChange("Attack", data.animBool);
        //対象配列のポインタ
        if (targetPointa != (int)obj[3]) targetPointa = (int)obj[3];
    }

    /// <summary>
    /// アニメーションを切り替えてNavMeshAgentのspeedを更新する
    /// </summary>
    /// <param name="animName">アニメーション名</param>
    /// <param name="animBool">アニメーションのオンオフ</param>
    private void AnimChange(string animName, bool animBool)
    {
        if (!anim.enabled) return;
        anim.SetBool(animName, animBool);
        if (animBool) nav.speed = 0f;
        else nav.speed = unitSpeed;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
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

    public void Move()
    {
        if(isMove()) targetPointa++;
    }

    /// <summary>
    /// 移動する
    /// 移動できなければfalseを返す
    /// </summary>
    /// <returns></returns>
    private bool isMove()
    {
        //ターゲットが存在しない
        if (targets.Count <= 0) return false;
        //ナビが存在しない
        if (nav == null) return false;
        //ポインタが配列の末以上を指している
        if (targetPointa >= targets.Count)
        {
            animBool = true;
            AnimChange("Attack", animBool);
            return false;
        }
        //それぞれのコンポーネントのenableがfalseであればtrueにする
        if (!anim.enabled) anim.enabled = true;
        if (!nav.enabled) nav.enabled = true;
        //目的地へ移動
        nav.destination = targets[targetPointa];
        //ユニットの速度が自身の速度と異なっていたら自身の速度にする
        if (nav.speed != unitSpeed) nav.speed = unitSpeed;
        //移動アニメーションを再生できないフラグであればできるようにする
        if (animBool)
        {
            animBool = false;
            AnimChange("Attack", animBool);
        }
        return true;
    }

    public void MyColor(int id)
    {
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
        //自分の色情報を格納
        Color myColor;
        //生成者が自分か相手か判別
        if (IsSameId(id, PhotonNetwork.player.ID))
        {
            myColor = color.First();
            isMine.Value = true;
        }
        else
        {
            myColor = color.Last();
            isMine.Value = false;
        }
        foreach (Renderer renderer in renderers) renderer.material.color = myColor;
        //生成者が自分でなければ向きを反転させる
        if (!isMine.Value) transform.rotation = Quaternion.Euler(0, 180, 0);
    }

    public void Attack(float attack, GameObject attackTarget)
    {
        const int attackInterval = 1;
        var a = attackTarget.GetComponent(typeof(IUnit)) as IUnit;
        //attackIntervalで設定した時間間隔で攻撃する
        Observable.Interval(TimeSpan.FromSeconds(attackInterval))
            .TakeUntilDestroy(attackTarget)
            .Subscribe(_ => Attack(attack, a), () => Comp())
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
        AnimChange("Attack", animBool);
    }

    /// <summary>
    /// 敵を倒した時の処理
    /// </summary>
    private void Comp()
    {
        if (targetList.Any()) targetList.RemoveAt(0);
        if (targetList.Any())
        {
            var attackList = targetList
                .Where(x => x != null)
                .Where(x => CalcDistance(transform.position, x.transform.position) <= targetDistance);
            if (attackList.Any()) GoToTarget(attackList.First());
            else isMove();
            targetList = attackList.ToList();
        }
        else isMove();
    }

    public void Damage(float damage)
    {
        unitHp.Value -= damage;
    }

    public void Death()
    {
        //死ぬタイミングを合わせるためにRPC
        photonView.RPC("DeathSync", PhotonTargets.Others);
        RendererDisaible();
        anim.enabled = false;
        isAlive = false;
        nav = null;
        //1秒後に自身をDestroyする
        Observable.Timer(TimeSpan.FromSeconds(1))
            .Subscribe(_ => Destroy(gameObject))
            .AddTo(gameObject);
    }
    /// <summary>
    /// レンダラーを消して見えないようにする
    /// </summary>
    private void RendererDisaible()
    {
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers) renderer.enabled = false;
    }

    [PunRPC]
    public void DeathSync()
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
        if (!targetList.Any()) GoToTarget(target);
        targetList.Add(target);
    }

    /// <summary>
    /// 対象に向かうことができる状態ならば向かう
    /// </summary>
    /// <param name="target">対象</param>
    private void GoToTarget(GameObject target)
    {
        if (target == null) return;
        if (nav == null) return;
        if (nav.pathStatus != NavMeshPathStatus.PathInvalid) nav.destination = target.transform.position;
        Attack(attackPower, target);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (CheckIUnit(other) == null) return;
        TargetLockOn(other.gameObject);
        if (nav != null) nav.speed = 0f;
    }

    /// <summary>
    /// IUnitの有無を確認しなければnullを返す
    /// </summary>
    /// <param name="other">OnTriggerでわたされるCollider</param>
    /// <returns></returns>
    private GameObject CheckIUnit(Collider other)
    {
        var unit = other.GetComponent(typeof(IUnit)) as IUnit;
        if (unit == null) return null;
        //同一生成者なら無視する
        if (unit.isMine.Value == isMine.Value) return null;
        return other.gameObject;
    }
}


