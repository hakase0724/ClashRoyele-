using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using static StaticUse;

/// <summary>
/// 生成可能エリア
/// </summary>
public class InstantiateCheck : Photon.MonoBehaviour
{
    private Vector3 inputPonit;

    public void Click()
    {
        inputPonit = InputToEvent.inputHitPos;
    }

    /// <summary>
    /// 生成可能な場所か判定し結果を返す
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool IsInstantiateCheck(Vector3 vector,int id)
    {
        if (!IsSameId(id, PhotonNetwork.player.ID)) return true;
        if (inputPonit == vector) return false;
        else return true;
    }
}
