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
        photonView.RPC("RPCTest", PhotonTargets.All, InputToEvent.inputHitPos);
    }

    [PunRPC]
    private IEnumerator RPCTest(Vector3 pos)
    {
        Debug.Log("RPC" + PhotonNetwork.player.ID);
        if (photonView.isMine)
        {
            Observable.IntervalFrame(1)
                .Subscribe(_=> Instantiate(prefab, pos + new Vector3(0, 1, 0), Quaternion.identity));
        }
        else if(!photonView.isMine) Instantiate(prefab, pos + new Vector3(0,1,0), Quaternion.identity);
        yield return null;
    }
	
}
