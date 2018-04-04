using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class Piece : MonoBehaviour
{

    [SerializeField]
    private Color[] color = new Color[2];
    private void Start()
    {
        this.OnMouseOverAsObservable()
            .Subscribe(_ => GetComponent<Renderer>().material.color = color[0]);
        this.OnMouseExitAsObservable()
            .Do(_=>Debug.Log("離れた"))
            .Subscribe(_ => GetComponent<Renderer>().material.color = color[1]);
    }
}
