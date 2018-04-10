using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDataSync :Photon.MonoBehaviour
{
    [SerializeField]
    private GameObject[] test = new GameObject[0];

    private List<Vector3> syncPos = new List<Vector3>();

	// Use this for initialization
	void Start ()
    {
        foreach(var t in test)
        {
            syncPos.Add(t.transform.position);
        }
	}

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //データの送信
            stream.SendNext(syncPos);
            foreach(var p in syncPos)
            {
                Debug.Log("送信：" + syncPos);
            }
           
        }
        else
        {
            //データの受信
            this.syncPos = (List<Vector3>)stream.ReceiveNext();
            foreach (var p in syncPos)
            {
                Debug.Log("受信：" + syncPos);
            }
        }
    }
}
