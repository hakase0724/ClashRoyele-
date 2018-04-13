using UniRx;
using UniRx.Triggers;
using UnityEngine;
using static StaticUse;

/// <summary>
/// マスターシーンの処理
/// </summary>
public class Master : Photon.PunBehaviour
{
    [SerializeField]
    private GameObject inputCanvas;
    [SerializeField]
    private GameObject waitCanvas;
    [SerializeField]
    private GameObject waitCanvas2;

    private void Awake()
    {
        Time.timeScale = 1;
        if (PhotonNetwork.inRoom)
        {
            inputCanvas.SetActive(false);
            waitCanvas2.SetActive(true);
        }
    }

    private void Start()
    {
        const int playMemberNum = 2;

        this.UpdateAsObservable()
            .Select(x => PhotonNetwork.playerList.Length)
            .Where(x => x >= playMemberNum)
            .Subscribe(_ => SceneLoad("Main"));
    }
    
    /// <summary>
    /// プレイヤーデータを受け取りネットワーク接続を開始する
    /// </summary>
    /// <param name="data"></param>
    public void ConnectNetWork(PlayerData data)
    {
        inputCanvas.SetActive(false);
        waitCanvas.SetActive(true);
        PhotonNetwork.playerName = data.playerName;
        PhotonNetwork.ConnectUsingSettings("0." + SceneManagerHelper.ActiveSceneBuildIndex);
        //同期間隔
        const int sendRateValue = 30;
        const int sendRateOnSerializeValue = 30;
        PhotonNetwork.sendRate = sendRateValue;
        PhotonNetwork.sendRateOnSerialize = sendRateOnSerializeValue;
    }

    //ロビーに入った時、Roomを探して入る
    new private void OnJoinedLobby() => PhotonNetwork.JoinRandomRoom();

    //マスターサーバーへ接続した時、Roomを探して入る
    public override void OnConnectedToMaster() => PhotonNetwork.JoinRandomRoom();

    //Room参加失敗時、名前なしRoom作成し入る
    private void OnPhotonRandomJoinFailed() => PhotonNetwork.CreateRoom(null);

    //Photon接続状態をGUIに表示する
    private void OnGUI() => GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
}
