using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class Main : Photon.MonoBehaviour
{
    private PlayerData playerData = new PlayerData();

    private void Start()
    {
        Debug.Log("ルームに入った");
        Debug.Log(PhotonNetwork.playerName);
        playerData.playerName = PhotonNetwork.playerName;
        playerData.playerId = PhotonNetwork.player.ID;
        Debug.Log(playerData.playerName + "," + playerData.playerId);
        Debug.Log("ルーム処理が終わった");
    }
}
