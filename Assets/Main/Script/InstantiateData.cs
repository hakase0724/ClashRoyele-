using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using static StaticUse;

public class InstantiateData :Photon.MonoBehaviour
{
    private int _prefabNumber;
    private Main main => GetComponent<Main>();
    //生成時ユニットに割り当てるid
    //0から割り当てる
    private byte unitId = 100;

    [SerializeField]
    private GameObject[] prefabs = new GameObject[0];
    [SerializeField]
    private InstantiateCheck instantiateCheck;

    public GameObject Prefab(int id)
    {
        return prefabs[id];
    }

    public int prefabNumber
    {
        get
        {
            return _prefabNumber;
        }
    }
    public void UnitChange(int num)
    {
        _prefabNumber = num;
    }

    public bool IsInstantiateCheck(Vector3 pos ,int id)
    {
        return instantiateCheck.IsInstantiateCheck(pos, id);
    }

    public void MyInstantiate(int num, Vector3 pos, int id, float energy)
    {
        var useEnergy = Prefab(num).GetComponent(typeof(IUnit)) as IUnit;
        if (!main.IsUseEnergy(useEnergy.unitEnergy, id, energy)) return;
        photonView.RPC("MyInstantiateRPC", PhotonTargets.All, num, pos, id, energy);
    }

    [PunRPC]
    private IEnumerator MyInstantiateRPC(int num, Vector3 pos, int id, float energy)
    {
        //生成待機時間
        const int waitFrame = 20;
        //生成者が自分ならwaitFrame分待機
        //相手なら即時生成
        if (IsSameId(id, PhotonNetwork.player.ID))
        {
            Observable.TimerFrame(waitFrame)
                .Subscribe(_ => MyInstantiate(Prefab(num), pos + new Vector3(0, 1, 0), id, energy));
        }
        else
        {
            MyInstantiate(Prefab(num), pos + new Vector3(0, 1, 0), id, energy);
        }
        yield return null;
    }

    /// <summary>
    /// オブジェクト生成メソッド
    /// </summary>
    /// <param name="game">生成するオブジェクト</param>
    /// <param name="pos">生成場所</param>
    /// <param name="id">生成者ID</param>
    private void MyInstantiate(GameObject game, Vector3 pos, int id, float energy)
    {
        GameObject gameObject;
        if (IsSameId(id, PhotonNetwork.player.ID)) gameObject = Instantiate(game, pos, Quaternion.identity);
        else gameObject = Instantiate(game, new Vector3(-pos.x, pos.y, -pos.z), Quaternion.identity);
        var unit = gameObject.GetComponent(typeof(IUnit)) as IUnit;
        unit.unitId = unitId;
        unitId++;
        unit.MyColor(id);
    }
}
