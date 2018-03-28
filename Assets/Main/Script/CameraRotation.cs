using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StaticUse;
using Photon;

public class CameraRotation : Photon.MonoBehaviour
{
    private void Start()
    {
        if (!PhotonNetwork.isMasterClient) transform.Rotate(new Vector3(0, 0, 180));
    }
}
