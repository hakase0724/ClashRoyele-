using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class TestCreate : Photon.MonoBehaviour
{
    public GameObject obj;

    void Start()
    {

    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();

        //左クリックした場合
        if (Input.GetMouseButtonDown(0))
        {
            //レイを投げて何かのオブジェクトに当たった場合
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("生成位置" + hit.point);
                //レイが当たった位置(hit.point)にオブジェクトを生成する
                PhotonNetwork.Instantiate(obj.name, hit.point, Quaternion.identity,0);
                //Instantiate(obj, hit.point, Quaternion.identity);
            }
        }
    }
}
