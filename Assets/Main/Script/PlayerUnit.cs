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
        Vector3 vector;
        if (identificationNumber == 0)
        {
            transform.Rotate(new Vector3(0, 180, 0));
            vector = new Vector3(0, 0, -1);
        }
        else
        {
            //transform.Rotate(new Vector3(0, 180, 0));
            vector = new Vector3(0, 0, 1);
        }
        if (Camera.main.transform.rotation.z < 180)
        {
            transform.Rotate(new Vector3(0, 180, 0));
            vector *= -1;
        }
        this.UpdateAsObservable()
            .Subscribe(_ => rb.velocity = vector);
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
