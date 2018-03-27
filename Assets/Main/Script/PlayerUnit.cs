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
    private void Start()
    {
        if (photonView.isMine) GetComponent<Renderer>().material = playerColor[0];
        else if (!photonView.isMine)
        {
            GetComponent<Renderer>().material = playerColor[1];
            rb.isKinematic = true;
        }
        Move();
    }

    public void Move()
    {
        this.UpdateAsObservable()
            .Subscribe(_ => rb.velocity = Vector3.forward);
    }

    public void Attack(float attack)
    {

    }

    public void Damage(float damage)
    {

    }

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //データの送信
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(rb.velocity);
          
        }
        else
        {
            //データの受信
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
            rb.velocity = (Vector3)stream.ReceiveNext();          
        }
    }
}
