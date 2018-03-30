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
    public BoolReactiveProperty isMine { get; set; } = new BoolReactiveProperty();
    public BoolReactiveProperty isAlive { get; set; } = new BoolReactiveProperty(true);
    public FloatReactiveProperty UnitHp { get; set; } = new FloatReactiveProperty(10);
    public float UnitEnergy { get; set; } = 1;
    private GameObject stageScript => GameObject.FindGameObjectWithTag("Main");
    private CameraRotation camera => Camera.main.GetComponent<CameraRotation>();
    private GameObject targetObject;
    private Transform nextTargetTransform;
    private IUnit target;
    private IDisposable updateStream;
    private IDisposable intervalStream;
    //識別番号 0=自分.1=自分以外
    private int identificationNumber = 0;
    private Rigidbody rb => GetComponent<Rigidbody>();
    private Animator anim => GetComponent<Animator>();
    [SerializeField]
    private Color[] color = new Color[0];

    public void Move()
    {
        anim.enabled = true;
        //対象のリスト
        List<GameObject> targetList = new List<GameObject>();
        if (targetObject == null) targetList.AddRange(stageScript.GetComponent<BulidingsManeger>().bridges);
        else if (identificationNumber == 0) targetList.AddRange(stageScript.GetComponent<BulidingsManeger>().myBulidings);
        else targetList.AddRange(stageScript.GetComponent<BulidingsManeger>().enemyBulidings);
        //一番近い建物を探してその方向を向く
        targetObject = CalcDistance(gameObject, targetList, camera.IsRotated);
        target = targetObject.GetComponent(typeof(IUnit)) as IUnit;
        var t = TargetLock();
        updateStream = this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                transform.LookAt(targetObject.transform);
                rb.velocity = transform.forward;
            });
    }

    private GameObject TargetLock()
    {
        Debug.Log("TargetLock Called!");
        const float viewAngle = 60.0f;
        Debug.Log("向き宣言");
        Vector3 viewVector = Vector3.forward;
        
        //対象のリスト
        List<GameObject> targetList = new List<GameObject>();
        Debug.Log("リスト更新");
        if (targetObject == null) targetList.AddRange(stageScript.GetComponent<BulidingsManeger>().bridges);
        else if (identificationNumber == 0) targetList.AddRange(stageScript.GetComponent<BulidingsManeger>().myBulidings);
        else targetList.AddRange(stageScript.GetComponent<BulidingsManeger>().enemyBulidings);
        Debug.Log("リスト制作");
        var lockOnList =
            (from lockOn in targetList
             where Vector3.Angle((lockOn.transform.position - this.transform.position).normalized, viewVector) <= viewAngle
             select lockOn).ToList();
        Debug.Log("ログ表示");
        if (lockOnList.Count <= 0)
        {
            Debug.Log("ロックオンしたオブジェクトの数" + lockOnList.Count);
            return null;
        }
        foreach (var l in lockOnList)
        {
            Debug.Log("ロックオンしたオブジェクト" + l);
        }
        return null;
             
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
        identificationNumber = colorNumber;
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
        //攻撃間隔
        const int attackInterval = 1;
        //攻撃対象のIUnit付きコンポーネントを取得
        var attackTargetInterface = attackTarget.GetComponent(typeof(IUnit)) as IUnit;
        //攻撃時には動きを止める
        updateStream.Dispose();
        rb.velocity = Vector3.zero;
        //攻撃間隔ごとに攻撃し相手のDamageメソッドを呼び出す
        intervalStream = Observable.Interval(System.TimeSpan.FromSeconds(attackInterval))
            .Subscribe(_ => attackTargetInterface.Damage(attack));
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
        UnitHp.Value -= damage;
    }

    public void Death()
    {
        isAlive.Value = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        //対象のIUnit付きコンポーネントを取得
        var otherUnit = other.gameObject.GetComponent(typeof(IUnit)) as IUnit;
        //ユニットでなければ
        if (otherUnit == null) return;
        //相手と自分の生成者が同一であれば
        if (otherUnit.isMine.Value == isMine.Value) return;
        //相手の体力があれば
        if (otherUnit.UnitHp.Value > 0) Attack(10f, other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        //対象のIUnit付きコンポーネントを取得
        var otherUnit = other.gameObject.GetComponent(typeof(IUnit)) as IUnit;
        //ユニットでなければ
        if (otherUnit == null) return;
        //相手と自分の生成者が同一であれば
        if (otherUnit.isMine.Value == isMine.Value) return;
        Move();
    }
}
