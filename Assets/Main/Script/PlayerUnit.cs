using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UniRx;
using UniRx.Triggers;

public class PlayerUnit : Photon.MonoBehaviour
{
    private Main main;
    [SerializeField]
    private Material[] playerColor = new Material[0];
    private void Start()
    {
        if (photonView.isMine)
        {
            main = GameObject.FindGameObjectWithTag("Main").GetComponent<Main>();
            if (main.playerData.playerId <= playerColor.Length) GetComponent<Renderer>().material = playerColor[main.playerData.playerId - 1];
            this.OnMouseDownAsObservable()
                .Where(_ => photonView.isMine)
                .Subscribe(_ => Destroy(gameObject));
        }
        
    }
}
