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
public class PlayerUnit : Photon.MonoBehaviour,IUnit
{
    public FloatReactiveProperty UnitHp { get; set; } = new FloatReactiveProperty(10);
    public float UnitEnergy { get; set; } = 1;
    private GameObject stageScript => GameObject.FindGameObjectWithTag("Main");
    private CameraRotation camera => Camera.main.GetComponent<CameraRotation>();
    private GameObject targetObject;
    private IUnit target;
    private IDisposable updateStream;
    private IDisposable intervalStream;
    //識別番号 0=自分.1=自分以外
    private int identificationNumber = 0;
    private Rigidbody rb => GetComponent<Rigidbody>();
    private Animator anim => GetComponent<Animator>();
    private List<Transform> buildingTransforms = new List<Transform>();
    [SerializeField]
    private Color[] color = new Color[0];

    public void Move()
    {
        anim.enabled = true;
        List<Transform> targetTransforms = new List<Transform>();
        if (identificationNumber == 0) targetTransforms.AddRange(stageScript.GetComponent<BulidingsManeger>().myBulidingsTransform);
        else targetTransforms.AddRange(stageScript.GetComponent<BulidingsManeger>().enemyBulidingsTransform);
        var nextTarget = CalcDistance(transform, targetTransforms, camera.IsRotated);
        Debug.Log(nextTarget);
        //一番近い建物を探してその方向を向く
        updateStream = this.UpdateAsObservable()
            .Subscribe(_ => 
            {
                transform.LookAt(nextTarget);
                rb.velocity = transform.forward;
            });
    }

    public void MyColor(int id)
    {
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
        int colorNumber;
        //生成者が自分か相手か判別
        if (IsSameId(id,PhotonNetwork.player.ID)) colorNumber = 0;
        else colorNumber = 1;
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

    public void Attack(float attack)
    {
        const int attackInterval = 1;
        updateStream.Dispose();
        rb.velocity = Vector3.zero;
        intervalStream = Observable.Interval(System.TimeSpan.FromSeconds(attackInterval)).Subscribe(_=> target.Damage(attack));
        target.UnitHp
            .Where(x => x <= 0)
            .Subscribe(_ => 
            {
                intervalStream.Dispose();
                Move();
            })
            .AddTo(targetObject);
    }

    public void Damage(float damage)
    {

    }

    public void Death()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("相手:" + other.gameObject.name + "自分:" + gameObject.name);
        if(other.gameObject.GetComponent(typeof(IUnit)) as IUnit != null)
        {
            targetObject = other.gameObject;
            target = targetObject.GetComponent(typeof(IUnit)) as IUnit;
            Attack(10f);            
        }
    }
}
