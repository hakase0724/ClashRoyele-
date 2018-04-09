using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Photon;
using static StaticUse;

public class Piece : PunBehaviour
{
    private InstantiateData data => GameObject.FindGameObjectWithTag("Main").GetComponent<InstantiateData>();
    private Main main => GameObject.FindGameObjectWithTag("Main").GetComponent<Main>();

    public bool isInstantiate { get; private set; } = true;

    [SerializeField]
    private Color[] color = new Color[2];
    private void Start()
    {
        if (transform.position.z >= 0) isInstantiate = false;
        this.OnMouseOverAsObservable()
            .Where(_ => isInstantiate)
            .Subscribe(_ => GetComponent<Renderer>().material.color = color[0]);

        this.OnMouseExitAsObservable()
            .Where(_ => isInstantiate)
            .Subscribe(_ => GetComponent<Renderer>().material.color = color[1]);

        this.OnMouseDownAsObservable()
            .Where(_ => isInstantiate)
            .Subscribe(_ =>
            {
                if (!PhotonNetwork.inRoom) return;
                data.MyInstantiate(data.prefabNumber, transform.position, PhotonNetwork.player.ID, main.energy.Value);
            })
            .AddTo(gameObject);
    }
}
