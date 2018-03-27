using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class SyncMove : Photon.MonoBehaviour
{
    private Rigidbody rb => GetComponent<Rigidbody>();
    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //データの送信
            stream.SendNext(transform.position);
            //stream.SendNext(transform.rotation);
            stream.SendNext(rb.velocity);

        }
        else
        {
            //データの受信
            transform.position = (Vector3)stream.ReceiveNext();
            //transform.rotation = (Quaternion)stream.ReceiveNext();
            rb.velocity = (Vector3)stream.ReceiveNext();
        }
    }

}
