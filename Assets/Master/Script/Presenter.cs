﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
/// <summary>
/// MV(R)PのPresenter名前入力を受けて接続命令を走らせる
/// </summary>
public class Presenter : MonoBehaviour
{
    [SerializeField]
    private InputField nameInput;
    [SerializeField]
    private Model model;
    [SerializeField]
    private Master master;

    private void Start()
    {
        PlayerData data = new PlayerData();
        nameInput
            .OnValueChangedAsObservable()
            .Subscribe(x => model.name.Value = x);

        model.name
            .Where(x => !string.IsNullOrEmpty(x))
            .Where(_ => Input.GetKeyDown(KeyCode.Space))
            .Subscribe(x => 
            {
                data.playerName = x;
                master.ConnectNetWork(data);
            });
    }
	
}
