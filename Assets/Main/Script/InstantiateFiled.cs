using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using UniRx;
using UniRx.Triggers;
using System.Linq;
using static StaticUse;

/// <summary>
/// オブジェクトを生成する場所
/// </summary>
public class InstantiateFiled : Photon.PunBehaviour
{
    private Main main => GameObject.FindGameObjectWithTag("Main").GetComponent<Main>();
    private int prefabNumber;
    private int buildingNumber;
    [SerializeField]
    private GameObject[] prefab = new GameObject[0];
    [SerializeField]
    private GameObject instantiateZone;
    [SerializeField]
    private GameObject[] myBuildings = new GameObject[0];

    private void Awake()
    {
        prefabNumber = 0;
    }

    private void Start()
    {
        List<GameObject> buildings = new List<GameObject>();
        if (Camera.main.GetComponent<CameraRotation>().IsRotated)
        {
            buildings = (from b in myBuildings select b).Skip(3).Take(3).ToList();
        }
        else
        {
            buildings = (from b in myBuildings select b).Take(3).ToList();
        }
        foreach(var b in buildings)
        {
            photonView.RPC("MyInstantiateRPC", PhotonTargets.All, prefabNumber, b.transform.position, PhotonNetwork.player.ID, main.energy.Value);
        }
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.C))
            .Subscribe(_ => photonView.RPC("RPCTest", PhotonTargets.All));
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.M))
            .Subscribe(_ => Debug.Log(PhotonNetwork.playerList.Length));
    }

    private void OnClick()
    {
        if (!PhotonNetwork.inRoom) return;
        photonView.RPC("MyInstantiateRPC", PhotonTargets.All,prefabNumber, InputToEvent.inputHitPos, PhotonNetwork.player.ID,main.energy.Value);
    }

    [PunRPC]
    private void RPCTest()
    {
        Debug.Log("RPCCall!" + PhotonNetwork.player.ID);
    }

    /// <summary>
    /// オブジェクトを生成する
    /// </summary>
    /// <param name="pos">生成場所</param>
    /// <param name="id">生成者ID</param>
    /// <returns></returns>
    [PunRPC]
    protected virtual IEnumerator MyInstantiateRPC(int num,Vector3 pos,int id,float energy)
    {
        Debug.Log(!instantiateZone.GetComponent<InstantiateCheck>().IsInstantiateCheck(pos, id) + "確認結果");
        if(!instantiateZone.GetComponent<InstantiateCheck>().IsInstantiateCheck(pos,id)) yield break;
        //生成待機時間
        const int waitFrame = 10;
        //生成者が自分ならwaitFrame分待機
        //相手なら即時生成
        if (IsSameId(id, PhotonNetwork.player.ID))
        {
            Debug.Log("自分で生成");
            Observable.TimerFrame(waitFrame)
                .Subscribe(_ => MyInstantiate(prefab[num], pos + new Vector3(0, 1, 0), id, energy));
        }
        else
        {
            Debug.Log("相手が生成");
            MyInstantiate(prefab[num], pos + new Vector3(0, 1, 0), id, energy);
        }
        yield return null;
    }

    /// <summary>
    /// オブジェクト生成メソッド
    /// </summary>
    /// <param name="game">生成するオブジェクト</param>
    /// <param name="pos">生成場所</param>
    /// <param name="id">生成者ID</param>
    protected virtual void MyInstantiate(GameObject game,Vector3 pos,int id,float energy)
    {
        var useEnergy = game.GetComponent(typeof(IUnit)) as IUnit;
        if (!main.IsUseEnergy(useEnergy.UnitEnergy,id,energy)) return;
        GameObject gameObject = Instantiate(game, pos, Quaternion.identity);
        var unit = gameObject.GetComponent(typeof(IUnit)) as IUnit;
        unit.MyColor(id);
    }
	
    public void UnitChange(int num)
    {
        prefabNumber = num;
    }
}
