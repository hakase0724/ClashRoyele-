using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaInstantiate : MonoBehaviour
{
    [SerializeField]
    private GameObject[] area = new GameObject[0];

    private void Start()
    {
        Area();
    }

    private void Area()
    {
        if (Camera.main.GetComponent<CameraRotation>().IsRotated) area[0].SetActive(true);
        else area[1].SetActive(true);
    }
}
