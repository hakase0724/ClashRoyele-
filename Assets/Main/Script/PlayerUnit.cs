using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UniRx;
using UniRx.Triggers;
using System.Linq;
using static StaticUse;

/// <summary>
/// ユニット
/// </summary>
public class PlayerUnit : Photon.MonoBehaviour,IUnit
{
    public float UnitHp { get; set; } = 10;
    public float UnitEnergy { get; set; } = 1;
    private GameObject stageScript => GameObject.FindGameObjectWithTag("Main");
    //識別番号
    private int identificationNumber = 0;
    private Rigidbody rb => GetComponent<Rigidbody>();
    private Animator anim => GetComponent<Animator>();
    private List<Transform> buildingTransforms = new List<Transform>();
    [SerializeField]
    private Color[] color = new Color[0];

    public void Move()
    {
        anim.enabled = true;
        //一番近い建物を探してその方向を向く
        transform.LookAt((CalcDistance(transform.position, stageScript.GetComponent<BulidingsManeger>().bulidingsTransform)));
        this.UpdateAsObservable()
            .Subscribe(_ => rb.velocity = transform.forward);
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

    }

    public void Damage(float damage)
    {

    }
}
