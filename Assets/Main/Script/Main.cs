using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;
using static StaticUse;

public class Main : Photon.MonoBehaviour
{
    //ユニットを出すのに使用するエネルギー
    public FloatReactiveProperty energy { get; private set; } = new FloatReactiveProperty(3);
    //プレイヤーデータをやり取りするための領域
    public PlayerData playerData { get; private set; }
    //エネルギーを表示するバー
    [SerializeField]
    private Slider slider;
    private void Awake()
    {
        playerData = new PlayerData();
    }

    private void Start()
    {
        playerData.playerName = PhotonNetwork.playerName;
        playerData.playerId = PhotonNetwork.player.ID;
        float energyUpRate = 0.001f;
        Observable.IntervalFrame(1)
            .Subscribe(_ => energy.Value += energyUpRate);

        energy
            .Where(_=>slider.value <= 1)
            //現在のenergyの値を10分の１にしてゲージに反映させる
            .Subscribe(x => slider.value = x / 10);
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
}
