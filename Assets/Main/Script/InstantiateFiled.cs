using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

/// <summary>
/// オブジェクトを生成する場所
/// </summary>
public class InstantiateFiled : Photon.MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;
    private void OnClick()
    {
        if (!PhotonNetwork.inRoom) return;
        PhotonNetwork.Instantiate(prefab.name, InputToEvent.inputHitPos, Quaternion.identity, 0);
        photonView.RPC("RPCTest", PhotonTargets.All);
    }

    [PunRPC]
    private void RPCTest()
    {
        Debug.Log("RPC" + PhotonNetwork.player.ID);
    }
	
}
