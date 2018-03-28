﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UniRx;
using static StaticUse;

/// <summary>
/// オブジェクトを生成する場所
/// </summary>
public class InstantiateFiled : Photon.PunBehaviour
{
    [SerializeField]
    private Main main;
    [SerializeField]
    private GameObject prefab;
    private void OnClick()
    {
        if (!PhotonNetwork.inRoom) return;       
        photonView.RPC("RPCTest", PhotonTargets.All, InputToEvent.inputHitPos, PhotonNetwork.player.ID,main.energy.Value);
    }

    /// <summary>
    /// オブジェクトを生成する
    /// </summary>
    /// <param name="pos">生成場所</param>
    /// <param name="id">生成者ID</param>
    /// <returns></returns>
    [PunRPC]
    private IEnumerator RPCTest(Vector3 pos,int id,float energy)
    {
        //生成待機時間
        const int waitFrame = 10;
        //生成者が自分ならwaitFrame分待機
        //相手なら即時生成
        if (IsSameId(id,PhotonNetwork.player.ID))
        {
            Observable.TimerFrame(waitFrame)
                .Subscribe(_=> MyInstantiate(prefab, pos + new Vector3(0, 1, 0),id, energy));
        }
        else MyInstantiate(prefab, pos + new Vector3(0, 1, 0), id,energy);
        yield return null;
    }

    /// <summary>
    /// オブジェクト生成メソッド
    /// </summary>
    /// <param name="game">生成するオブジェクト</param>
    /// <param name="pos">生成場所</param>
    /// <param name="id">生成者ID</param>
    private void MyInstantiate(GameObject game,Vector3 pos,int id,float energy)
    {
        var useEnergy = game.GetComponent(typeof(IUnit)) as IUnit;
        Debug.Log(main);
        Debug.Log(useEnergy.UnitEnergy);
        if (!main.IsUseEnergy(useEnergy.UnitEnergy,id,energy)) return;
        var gameObject = Instantiate(game, pos, Quaternion.identity);
        var unit = gameObject.GetComponent(typeof(IUnit)) as IUnit;
        unit.MyColor(id);
    }
	
}
