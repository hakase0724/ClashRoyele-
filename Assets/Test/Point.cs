using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
    public RootStetas rootStetas { get; private set; }
    [SerializeField]
    private RootStetas.LRStetas myLRStetas;
    [SerializeField]
    private int rootNumber;

    private void Awake()
    {
        rootStetas = new RootStetas(myLRStetas, rootNumber,gameObject);
    }
    private void Start()
    {
        if (Camera.main.GetComponent<CameraRotation>().IsRotated) transform.position = new Vector3(transform.position.x, transform.position.y, -transform.position.z);
        GameObject.FindGameObjectWithTag("Main").GetComponent<BulidingsManeger>().InsertRoot(rootStetas);
    }
}
