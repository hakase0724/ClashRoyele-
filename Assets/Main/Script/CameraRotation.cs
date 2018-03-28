using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StaticUse;
using Photon;

public class CameraRotation : Photon.MonoBehaviour
{
    public bool IsRotated { get; private set; } = false;
    private void Start()
    {
        if (!PhotonNetwork.isMasterClient)
        {
            transform.Rotate(new Vector3(0, 0, 180));
            IsRotated = true;
        }
    }
}
