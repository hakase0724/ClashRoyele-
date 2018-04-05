using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
using static StaticUse;

public class BuiidInstantiate : InstantiateFiled
{
    [SerializeField]
    private GameObject[] myBuildings = new GameObject[0];

    private void Start()
    {       
        for(int i = 0;i < myBuildings.Length; i++)
        {
            photonView.RPC("MyInstantiateRPC", PhotonTargets.All, i, myBuildings[i].transform.position, PhotonNetwork.player.ID, main.energy.Value);
        }
    }
    [PunRPC]
    protected override IEnumerator MyInstantiateRPC(int num, Vector3 pos, int id, float energy)
    {
        //生成待機時間
        const int waitFrame = 10;
        //生成者が自分ならwaitFrame分待機
        //相手なら即時生成
        //if(Camera.main.GetComponent<CameraRotation>().IsRotated) pos = new Vector3(pos.x, pos.y, -pos.z);
        if (IsSameId(id, PhotonNetwork.player.ID))
        {
            Observable.TimerFrame(waitFrame)
                .Subscribe(_ => MyInstantiate(myBuildings[num], pos, id, energy));
        }
        else
        {
            pos = new Vector3(pos.x, pos.y, -pos.z);
            MyInstantiate(myBuildings[num], pos, id, energy);
        }
        yield return null;
    }

}
