﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using UniRx;
using static StaticUse;

/// <summary>
/// オブジェクトを生成する場所
/// </summary>
public class InstantiateFiled : Photon.PunBehaviour
{
    private Main main => GameObject.FindGameObjectWithTag("Main").GetComponent<Main>();
    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    private GameObject instantiateZone;
    private void OnClick()
    {
        if (!PhotonNetwork.inRoom) return;
        photonView.RPC("MyInstantiateRPC", PhotonTargets.All, InputToEvent.inputHitPos, PhotonNetwork.player.ID,main.energy.Value);
        Debug.Log(InputToEvent.inputHitPos + "Filed");
    }

    /// <summary>
    /// オブジェクトを生成する
    /// </summary>
    /// <param name="pos">生成場所</param>
    /// <param name="id">生成者ID</param>
    /// <returns></returns>
    [PunRPC]
    protected virtual IEnumerator MyInstantiateRPC(Vector3 pos,int id,float energy)
    {
        Debug.Log(!instantiateZone.GetComponent<InstantiateCheck>().IsInstantiateCheck(pos, id) + "確認結果");
        if(!instantiateZone.GetComponent<InstantiateCheck>().IsInstantiateCheck(pos,id)) yield break;
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
        if (!main.IsUseEnergy(useEnergy.UnitEnergy,id,energy)) return;
        GameObject gameObject = Instantiate(game, pos, Quaternion.identity);
        //if (IsSameId(id,PhotonNetwork.player.ID)) gameObject = Instantiate(game, pos, Quaternion.identity);
        //else gameObject = Instantiate(game, pos, Quaternion.Euler(0,180,0));
        var unit = gameObject.GetComponent(typeof(IUnit)) as IUnit;
        unit.MyColor(id);
    }
	
}
