using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class Main : Photon.MonoBehaviour
{
    public PlayerData playerData { get; private set; }

    private void Awake() => playerData = new PlayerData();

    private void Start()
    {
        playerData.playerName = PhotonNetwork.playerName;
        playerData.playerId = PhotonNetwork.player.ID;
    }
}
