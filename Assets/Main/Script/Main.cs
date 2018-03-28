using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;
using static StaticUse;

public class Main : Photon.MonoBehaviour
{
    public FloatReactiveProperty energy { get; private set; } = new FloatReactiveProperty(3);
    public PlayerData playerData { get; private set; }
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

    public bool IsUseEnergy(float useEnergy,int id)
    {
        if (energy.Value < useEnergy) return false;
        else if (IsSameId(id, PhotonNetwork.player.ID))
        {
            energy.Value -= useEnergy;
            return true;
        }
        else return true;
    }
}
