using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UniRx;
using UniRx.Triggers;

public class PlayerUnit : Photon.MonoBehaviour,IUnit
{
    public float UnitHp { get; set; }
    private Rigidbody rb => GetComponent<Rigidbody>();
    [SerializeField]
    private Material[] playerColor = new Material[0];

    public void Move()
    {
        this.UpdateAsObservable()
            .Subscribe(_ => rb.velocity = Vector3.forward);
    }

    public void MyColor(int id)
    {
        if (id == PhotonNetwork.player.ID) GetComponent<Renderer>().material = playerColor[0];
        else if (id != PhotonNetwork.player.ID) GetComponent<Renderer>().material = playerColor[1];
        Move();
    }

    public void Attack(float attack)
    {

    }

    public void Damage(float damage)
    {

    }
}
