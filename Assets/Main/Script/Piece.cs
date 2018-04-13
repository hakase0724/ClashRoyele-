using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Photon;
using static StaticUse;

/// <summary>
/// フィールドの各ブロックの処理
/// </summary>
public class Piece : PunBehaviour
{
    private InstantiateData data => GameObject.FindGameObjectWithTag("Main").GetComponent<InstantiateData>();
    private Main main => GameObject.FindGameObjectWithTag("Main").GetComponent<Main>();

    //生成可能なブロックであるか
    public bool isInstantiate { get; private set; } = true;

    [SerializeField]
    private Color[] color = new Color[2];
    private void Start()
    {
        //自分の座標によって生成可否を決める
        if (transform.position.z >= 0) isInstantiate = false;

        //生成可能であればマウスが乗った時自身の色を白にする
        this.OnMouseOverAsObservable()
            .Where(_ => isInstantiate)
            .Subscribe(_ => GetComponent<Renderer>().material.color = color[0]);

        //生成可能であればマウスがはなれた時自身の色を元に戻す
        this.OnMouseExitAsObservable()
            .Where(_ => isInstantiate)
            .Subscribe(_ => GetComponent<Renderer>().material.color = color[1]);

        //生成可能であればクリックされたときオブジェクトを生成する
        this.OnMouseDownAsObservable()
            .Where(_ => isInstantiate)
            .Subscribe(_ =>
            {
                //フォトンのルームに入っていなければ何もしない
                if (!PhotonNetwork.inRoom) return;
                data.MyInstantiate(data.prefabNumber, transform.position, PhotonNetwork.player.ID, main.energy.Value);
            })
            .AddTo(gameObject);
    }
}
