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
    [SerializeField]
    private float hp;
    public BoolReactiveProperty isMine { get; set; } = new BoolReactiveProperty();
    public BoolReactiveProperty isAlive { get; set; } = new BoolReactiveProperty(true);
    public FloatReactiveProperty UnitHp { get; set; } = new FloatReactiveProperty(100);
    public float UnitEnergy { get; set; } = 1;
    private GameObject stageScript => GameObject.FindGameObjectWithTag("Main");
    //次に狙う対象
    private GameObject nextTarget;
    private List<GameObject> targets = new List<GameObject>();
    private IDisposable updateStream;
    private float unitSpeed;
    private Rigidbody rb => GetComponent<Rigidbody>();
    private Animator anim => GetComponent<Animator>();
    [SerializeField]
    private Color[] color = new Color[0];

    private void Awake()
    {
        UnitHp.Value = hp;
        if (Camera.main.GetComponent<CameraRotation>().IsRotated) Quaternion.EulerAngles(0, 180, 0);
    }

    private void Start()
    {
        targets = stageScript.GetComponent<BulidingsManeger>().wayPoints;
        anim.enabled = true;
        nextTarget = SearchMinDistance(CalcDistance());
        unitSpeed = 1f;
        UnitHp
            .Where(x => x <= 0)
            .Subscribe(x => Death());
    }

    public void Move()
    {     
        updateStream = this.UpdateAsObservable()
            .Where(_=>nextTarget != null)
            .Subscribe(_ =>
            {
                transform.LookAt(nextTarget.transform);
                transform.Rotate(new Vector3(0, transform.rotation.y, 0));
                transform.position = Vector3.MoveTowards(this.transform.position, nextTarget.transform.position, unitSpeed);
                //rb.velocity = transform.forward * unitSpeed;
            })
            .AddTo(gameObject);
    }

    public void Move(GameObject enemy)
    {
        updateStream.Dispose();
        updateStream = this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                transform.LookAt(enemy.transform);
                transform.Rotate(new Vector3(0, transform.rotation.y, 0));
                transform.position = Vector3.MoveTowards(this.transform.position, nextTarget.transform.position, unitSpeed);
                //rb.velocity = transform.forward * unitSpeed;
            })
            .AddTo(gameObject);
    }
    #region 
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
    #endregion
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
        const int waitTime = 1;
        Observable.Timer(System.TimeSpan.FromSeconds(waitTime))
            .Subscribe(_ => Move());
    }

    public void Attack(float attack,GameObject attackTarget)
    {
        Debug.Log(attackTarget);
        //攻撃間隔
        const int attackInterval = 1;
        //攻撃対象のIUnit付きコンポーネントを取得
        var attackTargetInterface = attackTarget.GetComponent(typeof(IUnit)) as IUnit;
        if (attackTargetInterface.isMine.Value == isMine.Value)
        {
            Debug.Log("自分には攻撃しない");
            return;
        }
        //攻撃時には動きを止める
        updateStream.Dispose();
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
                Move();
            })
            .AddTo(gameObject);
    }

    public void Damage(float damage)
    {
        Debug.Log("ダメージを受ける");
        UnitHp.Value -= damage;
    }

    public void Death()
    {
        isAlive.Value = false;
        Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        //対象のIUnit付きコンポーネントを取得
        var otherUnit = other.gameObject.GetComponent(typeof(IUnit)) as IUnit;
        //ユニットでなければ
        if (otherUnit == null) return;
        //相手と自分の生成者が同一であれば
        if (otherUnit.isMine.Value == isMine.Value) return;
        if (!IsLooked(other.gameObject))
        {
            Debug.Log("見えていない");
            return;
        }
        //相手の体力があれば
        if (otherUnit.UnitHp.Value > 0) Move(other.gameObject);
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
        if (other.gameObject == gameObject) return;
        if (otherComponent.isMine.Value != isMine.Value) Attack(10f, other.gameObject);
        else updateStream.Dispose();
    }

    private  void OnCollisionStay(Collision other)
    {
        var otherComponent = other.gameObject.GetComponent(typeof(IUnit)) as IUnit;
        if (otherComponent == null) return;
        if (otherComponent.isMine.Value == isMine.Value) rb.velocity = transform.right;
    }

    private void OnCollisionExit(Collision other)
    {
        var otherComponent = other.gameObject.GetComponent(typeof(IUnit)) as IUnit;
        if (otherComponent == null) return;
        if (otherComponent.isMine.Value == isMine.Value) Move();
    }

    public void NextSet(GameObject next)
    {
        nextTarget = next;
    }
}
