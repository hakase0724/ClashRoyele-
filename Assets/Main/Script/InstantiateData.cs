using System.Collections;
using UniRx;
using UnityEngine;
using static StaticUse;

/// <summary>
/// オブジェクトを生成する
/// </summary>
public class InstantiateData :Photon.MonoBehaviour
{
    //生成するプレハブの配列インデックス
    private int _prefabNumber;
    private Main main => GetComponent<Main>();
    //生成時ユニットに割り当てるid
    //100から割り当てる
    private byte unitId = 100;

    [SerializeField]
    private GameObject[] prefabs = new GameObject[0];

    /// <summary>
    /// IDに対応するgameObjectを返す
    /// </summary>
    /// <param name="id">生成するプレハブの配列インデックス</param>
    /// <returns></returns>
    public GameObject Prefab(int id)
    {
        return prefabs[id];
    }

    //生成するプレハブの配列インデックス(外部公開用)
    public int prefabNumber
    {
        get
        {
            return _prefabNumber;
        }
    }
    /// <summary>
    /// 生成するプレハブの配列インデックスを変更する
    /// </summary>
    /// <param name="num">変更する生成するプレハブの配列インデックス</param>
    public void UnitChange(int num)
    {
        _prefabNumber = num;
    }

    /// <summary>
    /// エネルギーを確認し生成が可能であれば生成させる
    /// </summary>
    /// <param name="num"></param>
    /// <param name="pos"></param>
    /// <param name="id"></param>
    /// <param name="energy"></param>
    public void MyInstantiate(int num, Vector3 pos, int id, float energy)
    {
        var useEnergy = Prefab(num).GetComponent(typeof(IUnit)) as IUnit;
        if (!main.IsUseEnergy(useEnergy.unitEnergy, id, energy)) return;
        photonView.RPC("MyInstantiateRPC", PhotonTargets.All, num, pos, id, energy);
    }

    /// <summary>
    /// 生成命令を受け取り命令者によって生成タイミングを変更する
    /// </summary>
    /// <param name="num">生成するプレハブの配列インデックス</param>
    /// <param name="pos">生成座標</param>
    /// <param name="id">生成者のID</param>
    /// <param name="energy">使用エネルギー</param>
    /// <returns></returns>
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
    /// ユニットを生成
    /// </summary>
    /// <param name="game">生成するgameObject</param>
    /// <param name="pos">生成座標</param>
    /// <param name="id">生成者のID</param>
    /// <param name="energy">使用エネルギー</param>
    private void MyInstantiate(GameObject game, Vector3 pos, int id, float energy)
    {
        GameObject gameObject;
        //生成者によって生成位置を変える
        if (IsSameId(id, PhotonNetwork.player.ID)) gameObject = Instantiate(game, pos, Quaternion.identity);
        else gameObject = Instantiate(game, new Vector3(-pos.x, pos.y, -pos.z), Quaternion.identity);
        var unit = gameObject.GetComponent(typeof(IUnit)) as IUnit;
        //ユニットIDを付与する
        unit.unitId = unitId;
        //ユニットIDを一つ進める
        unitId++;
        //ユニットの色を変える
        unit.MyColor(id);
    }
}
