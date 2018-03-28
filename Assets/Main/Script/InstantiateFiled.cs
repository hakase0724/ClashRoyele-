using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UniRx;

/// <summary>
/// オブジェクトを生成する場所
/// </summary>
public class InstantiateFiled : Photon.PunBehaviour
{
    [SerializeField]
    private GameObject prefab;
    private void OnClick()
    {
        if (!PhotonNetwork.inRoom) return;       
        photonView.RPC("RPCTest", PhotonTargets.All, InputToEvent.inputHitPos, PhotonNetwork.player.ID);
    }

    [PunRPC]
    private IEnumerator RPCTest(Vector3 pos,int id)
    {
        Debug.Log("RPC" + PhotonNetwork.player.ID);
        if (id == PhotonNetwork.player.ID)
        {
            Observable.TimerFrame(10)
                .Subscribe(_=> Instantiate(prefab, pos + new Vector3(0, 1, 0), Quaternion.identity));
        }
        else if(id != PhotonNetwork.player.ID) Instantiate(prefab, pos + new Vector3(0,1,0), Quaternion.identity);
        yield return null;
    }
	
}
