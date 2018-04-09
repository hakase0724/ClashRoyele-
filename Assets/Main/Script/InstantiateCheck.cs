using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UniRx;
using UniRx.Triggers;
using static StaticUse;

/// <summary>
/// 生成可能エリア
/// </summary>
public class InstantiateCheck : Photon.MonoBehaviour
{
    private Vector3 inputPoint;

    public BoolReactiveProperty isOverWraping { get; private set; } = new BoolReactiveProperty(false);

    private void Start()
    {
        this.OnMouseDownAsObservable()
            .Subscribe(_=> isOverWraping.Value = true);
    }

    public void Click()
    {
        Debug.Log("クリック");
        var point = Input.mousePosition;
        Debug.Log("クリックした点" + point);
        inputPoint = Camera.main.ScreenToWorldPoint(point);
        Debug.Log("実際の座標：" + inputPoint);
    }

    /// <summary>
    /// 生成可能な場所か判定し結果を返す
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool IsInstantiateCheck(Vector3 vector,int id)
    {
        //if (!IsSameId(id, PhotonNetwork.player.ID)) return true;
        Debug.Log("パネル上の点：" + inputPoint + "生成点：" + vector);
        if (inputPoint == vector) return false;
        else return true;
    }
}
