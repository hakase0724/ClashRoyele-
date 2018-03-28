using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UniRx;
using UniRx.Triggers;
using static StaticUse;

public class PlayerUnit : Photon.MonoBehaviour,IUnit
{
    public float UnitHp { get; set; } = 10;
    public float UnitEnergy { get; set; } = 1;
    private int identificationNumber = 0;
    private Rigidbody rb => GetComponent<Rigidbody>();
    private Animator anim => GetComponent<Animator>();
    [SerializeField]
    private Color[] color = new Color[0];

    public void Move()
    {
        anim.enabled = true;
        this.UpdateAsObservable()
            .Subscribe(_ => 
            {
                if (identificationNumber == 0) rb.velocity = -Vector3.forward;
                else rb.velocity = Vector3.forward;
            });
    }

    public void MyColor(int id)
    {
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
        int colorNumber;
        if (IsSameId(id,PhotonNetwork.player.ID)) colorNumber = 0;
        else colorNumber = 1;
        identificationNumber = colorNumber;
        foreach (Renderer renderer in renderers)
        {
            //プレイヤーIDを配列インデックスに合わせて色を変える
            renderer.material.color = color[colorNumber];
        }
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
