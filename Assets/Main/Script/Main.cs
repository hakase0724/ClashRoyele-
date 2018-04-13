using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using static StaticUse;

public class Main : Photon.MonoBehaviour
{
    //ユニットを出すのに使用するエネルギー
    public FloatReactiveProperty energy { get; private set; } = new FloatReactiveProperty(3);
    //プレイヤーデータをやり取りするための領域
    public PlayerData playerData { get; private set; }
    public IntReactiveProperty enemyCount { get; private set; } = new IntReactiveProperty(3);
    //エネルギーを表示するバー
    [SerializeField]
    private Slider slider;
    [SerializeField]
    private GameObject warningCanvas;
    private void Awake()
    {
        playerData = new PlayerData();
    }

    private void Start()
    {
        //ルーム内メンバーの規定数
        const int playMemberNum = 2;

        //ルーム内のメンバーが規定数を下回ったらエラーとしてタイトルシーンに戻る
        this.UpdateAsObservable()
            .Select(x => PhotonNetwork.playerList.Length)
            .Where(x => x < playMemberNum)
            .Do(_ => warningCanvas.SetActive(true))
            .Where(_ => Input.anyKeyDown)
            .Subscribe(_ => SceneLoad("Master"));

        playerData.playerName = PhotonNetwork.playerName;
        playerData.playerId = PhotonNetwork.player.ID;
        float energyUpRate = 0.001f;
        Observable.IntervalFrame(1)
            .Subscribe(_ => energy.Value += energyUpRate);

        energy
            .Where(_=>slider.value <= 1)
            //現在のenergyの値を10分の１にしてゲージに反映させる
            .Subscribe(x => slider.value = x / 10);

        //タワーの数が0を下回ったら終了処理を行う
        enemyCount
            .Buffer(2,1)
            .Select(x => x.Last())
            .Where(x => x <= 0)
            .Subscribe(_ => photonView.RPC("End", PhotonTargets.All))
            .AddTo(gameObject);
    }

    /// <summary>
    /// 終了処理を同期させる
    /// </summary>
    [PunRPC]
    public void End()
    {
        Time.timeScale = 0;
        this.UpdateAsObservable()
            .Where(_ => Input.anyKeyDown)
            .Subscribe(_ =>
            {
                SceneLoad("Master");
                //ルームから退出
                PhotonNetwork.LeaveRoom();
                //サーバーとの接続を切断する
                PhotonNetwork.Disconnect();
            });
       
    }

    /// <summary>
    /// エネルギーと操作者によってオブジェクトの生成可否を判定する
    /// </summary>
    /// <param name="useEnergy">生成に使用するエネルギー</param>
    /// <param name="id">操作者識別ID</param>
    /// <param name="enemyEnergy">通信相手のエネルギー</param>
    /// <returns></returns>
    public bool IsUseEnergy(float useEnergy,int id,float enemyEnergy)
    {
        //操作者が自分であれば
        if (IsSameId(id, PhotonNetwork.player.ID))
        {
            //エネルギーが足りていれば
            if (energy.Value < useEnergy) return false;
            else
            {
                energy.Value -= useEnergy;
                return true;
            }
        }
        //相手のエネルギーが足りていなければ
        else if (enemyEnergy < useEnergy) return false;
        else return true;
    }

    /// <summary>
    /// 落ちたタワーの数を更新する
    /// </summary>
    /// <param name="value">タワーの数</param>
    public void EnemyCount(int value)
    {
        enemyCount.Value += value;
    }
}
