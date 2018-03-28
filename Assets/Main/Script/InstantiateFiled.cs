using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

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
        photonView.RPC("RPCTest", PhotonTargets.Others, InputToEvent.inputHitPos);
        PhotonNetwork.Instantiate(prefab.name, InputToEvent.inputHitPos, Quaternion.identity, 0);
    }

    [PunRPC]
    private IEnumerator RPCTest(Vector3 pos)
    {
        Debug.Log("RPC" + PhotonNetwork.player.ID);
        PhotonNetwork.Instantiate(prefab.name, pos, Quaternion.identity, 0);
        yield return null;
    }
	
}
