using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UniRx;
using UniRx.Triggers;
using static StaticUse;

public class PlayerUnit : Photon.MonoBehaviour,IUnit
{
    public float UnitHp { get; set; }
    private Rigidbody rb => GetComponent<Rigidbody>();
    [SerializeField]
    private Color[] color = new Color[0];

    public void Move()
    {
        this.UpdateAsObservable()
            .Subscribe(_ => rb.velocity = Vector3.forward);
    }

    public void MyColor(int id)
    {
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
        int colorNumber;
        if (IsSameId(id,PhotonNetwork.player.ID)) colorNumber = 0;
        else colorNumber = 1;
        foreach (Renderer renderer in renderers)
        {
            //プレイヤーIDを配列インデックスに合わせて色を変える
            renderer.material.color = color[colorNumber];
        }
        Move();
    }

    public void Attack(float attack)
    {

    }

    public void Damage(float damage)
    {

    }
}
