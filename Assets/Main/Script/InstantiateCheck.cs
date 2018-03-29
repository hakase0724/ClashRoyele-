using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateCheck : MonoBehaviour
{
    private Vector3 inputPonit;

    public void Click()
    {
        Debug.Log(InputToEvent.inputHitPos + "Panel");
        inputPonit = InputToEvent.inputHitPos;
        Debug.Log("クリック！");
    }

    public bool IsInstantiateCheck(Vector3 vector)
    {
        if (inputPonit == vector) return true;
        else return false;
    }
}
