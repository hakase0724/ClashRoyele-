using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using static StaticUse;

public class InstantiateCheck : Photon.MonoBehaviour
{
    private Vector3 inputPonit;

    public void Click()
    {
        Debug.Log(InputToEvent.inputHitPos + "Panel");
        inputPonit = InputToEvent.inputHitPos;
        Debug.Log("クリック！");
    }

    public bool IsInstantiateCheck(Vector3 vector,int id)
    {
        if (!IsSameId(id, PhotonNetwork.player.ID)) return true;
        if (inputPonit == vector) return true;
        else return false;
    }
}
