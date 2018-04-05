using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using static StaticUse;

public class Piece : Photon.PunBehaviour
{
    private InstantiateData data => GameObject.FindGameObjectWithTag("Main").GetComponent<InstantiateData>();
    private Main main => GameObject.FindGameObjectWithTag("Main").GetComponent<Main>();

    [SerializeField]
    private Color[] color = new Color[2];
    private void Start()
    {
        this.OnMouseOverAsObservable()
            .Subscribe(_ => GetComponent<Renderer>().material.color = color[0]);

        this.OnMouseExitAsObservable()
            .Do(_=>Debug.Log("離れた"))
            .Subscribe(_ => GetComponent<Renderer>().material.color = color[1]);

        this.OnMouseDownAsObservable()
            .Subscribe(_ =>
            {
                if (!PhotonNetwork.inRoom) return;
                photonView.RPC("MyInstantiateRPC", PhotonTargets.All, data.prefabNumber, InputToEvent.inputHitPos, PhotonNetwork.player.ID, main.energy.Value);
            })
            .AddTo(gameObject);
    }

    /// <summary>
    /// オブジェクトを生成する
    /// </summary>
    /// <param name="pos">生成場所</param>
    /// <param name="id">生成者ID</param>
    /// <returns></returns>
    [PunRPC]
    protected virtual IEnumerator MyInstantiateRPC(int num, Vector3 pos, int id, float energy)
    {
        if (!data.IsInstantiateCheck(pos, id)) yield break;
        //生成待機時間
        const int waitFrame = 20;
        //生成者が自分ならwaitFrame分待機
        //相手なら即時生成
        if (IsSameId(id, PhotonNetwork.player.ID))
        {
            Observable.TimerFrame(waitFrame)
                .Subscribe(_ => MyInstantiate(data.Prefab(num), pos + new Vector3(0, 1, 0), id, energy));
        }
        else
        {
            MyInstantiate(data.Prefab(num), pos + new Vector3(0, 1, 0), id, energy);
        }
        yield return null;
    }

    /// <summary>
    /// オブジェクト生成メソッド
    /// </summary>
    /// <param name="game">生成するオブジェクト</param>
    /// <param name="pos">生成場所</param>
    /// <param name="id">生成者ID</param>
    protected virtual void MyInstantiate(GameObject game, Vector3 pos, int id, float energy)
    {
        var useEnergy = game.GetComponent(typeof(IUnit)) as IUnit;
        if (!main.IsUseEnergy(useEnergy.unitEnergy, id, energy)) return;
        GameObject gameObject /*= Instantiate(game, pos, Quaternion.identity)*/;
        if (IsSameId(id, PhotonNetwork.player.ID)) gameObject = Instantiate(game, pos, Quaternion.identity);
        else gameObject = Instantiate(game, new Vector3(-pos.x, pos.y, -pos.z), Quaternion.identity);
        var unit = gameObject.GetComponent(typeof(IUnit)) as IUnit;
        unit.MyColor(id);
    }
}
