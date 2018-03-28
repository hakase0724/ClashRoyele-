using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Triggers;
using static StaticUse;

/// <summary>
/// マスターシーンの処理
/// </summary>
public class Master : Photon.PunBehaviour
{
    private PlayerData playerData = new PlayerData();
    
    /// <summary>
    /// プレイヤーデータを受け取りネットワーク接続を開始する
    /// </summary>
    /// <param name="data"></param>
    public void ConnectNetWork(PlayerData data)
    {
        playerData = data;
        PhotonNetwork.playerName = data.playerName;
        PhotonNetwork.ConnectUsingSettings("0." + SceneManagerHelper.ActiveSceneBuildIndex);
        //同期間隔
        const int sendRateValue = 30;
        const int sendRateOnSerializeValue = 30;
        PhotonNetwork.sendRate = sendRateValue;
        PhotonNetwork.sendRateOnSerialize = sendRateOnSerializeValue;
    }

    //ロビーに入った時、Roomを探して入る
    private void OnJoinedLobby() => PhotonNetwork.JoinRandomRoom();

    //マスターサーバーへ接続した時、Roomを探して入る
    public override void OnConnectedToMaster() => PhotonNetwork.JoinRandomRoom();

    //Room参加失敗時、名前なしRoom作成し入る
    private void OnPhotonRandomJoinFailed() => PhotonNetwork.CreateRoom(null);

    //Room参加成功時、メインシーンをロードする
    private void OnJoinedRoom() => SceneLoad("Main");

    //Photon接続状態をGUIに表示する
    private void OnGUI() => GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());

}
