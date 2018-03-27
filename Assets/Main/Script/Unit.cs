using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UniRx;
using UniRx.Triggers;

public class Unit : Photon.MonoBehaviour
{
    private void Start()
    {
        this.OnMouseDownAsObservable()
            .Where(_ => photonView.isMine)
            .Subscribe(_ => Destroy(gameObject));
    }
}
