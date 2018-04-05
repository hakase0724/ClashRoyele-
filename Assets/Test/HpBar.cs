﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;
using System.Linq;

public class HpBar : MonoBehaviour
{
    [SerializeField]
    private Slider slider;
    [SerializeField]
    private Image bar;
    [SerializeField]
    private Color[] colors = new Color[2];
	
	private void Start ()
    {
        var rootComponent = transform.root.GetComponent(typeof(IUnit)) as IUnit;
        var MaxHp = rootComponent.maxUnitHp;

        if (rootComponent.isMine.Value) bar.color = colors.First();
        else bar.color = colors.Last();

        rootComponent.unitHp
            .Subscribe(x => slider.value = x / MaxHp)
            .AddTo(gameObject);

        this.UpdateAsObservable()
            .Subscribe(_ => transform.rotation = Quaternion.Euler(0, 0, 0))
            .AddTo(gameObject);
    }
}