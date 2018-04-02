﻿using System.Collections;
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
    //次に狙う対象
    private GameObject nextTarget;
    private List<GameObject> targets = new List<GameObject>();
    private IDisposable updateStream;
    private IDisposable intervalStream;
    private float unitSpeed;
    private Rigidbody rb => GetComponent<Rigidbody>();
    private Animator anim => GetComponent<Animator>();
    [SerializeField]
    private Color[] color = new Color[0];

    private void Start()
    {
        targets = stageScript.GetComponent<BulidingsManeger>().wayPoints;
        anim.enabled = true;
        nextTarget = SearchMinDistance(CalcDistance());
        unitSpeed = 5f;
    }

    public void Move()
    {     
        updateStream = this.UpdateAsObservable()
            .Where(_=>nextTarget != null)
            .Subscribe(_ =>
            {
                transform.LookAt(nextTarget.transform);
                rb.velocity = transform.forward * unitSpeed;
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
    private void OnCollisionEnter(Collision other)
    {
        var otherComponent = other.gameObject.GetComponent(typeof(IUnit)) as IUnit;
        if (otherComponent == null) return;
        updateStream.Dispose();
    }

    private  void OnCollisionStay(Collision other)
    {
        var otherComponent = other.gameObject.GetComponent(typeof(IUnit)) as IUnit;
        if (otherComponent == null) return;
        Debug.Log("右へ回避");
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
        Debug.Log(nextTarget.name + "," + nextTarget.transform.position);
    }
}
