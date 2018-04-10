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
            var sendPos = syncPos.ToArray();
            //データの送信
            stream.SendNext(sendPos);
            foreach(var p in sendPos)
            {
                Debug.Log("送信：" + p);
            }
           
        }
        else
        {
            //データの受信
            var receivePos = (Vector3[])stream.ReceiveNext();
            foreach (var p in receivePos)
            {
                Debug.Log("受信：" + p);
            }
        }
    }
}
