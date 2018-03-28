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
        photonView.RPC("RPCTest", PhotonTargets.All, InputToEvent.inputHitPos);
    }

    [PunRPC]
    private IEnumerator RPCTest(Vector3 pos)
    {
        Debug.Log("RPC" + PhotonNetwork.player.ID);
        Instantiate(prefab, pos, Quaternion.identity);
        yield return null;
    }
	
}
