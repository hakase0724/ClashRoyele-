using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StaticUse;

/// <summary>
/// 建物管理
/// </summary>
public class BulidingsManeger : MonoBehaviour
{
    //管理している建物のリスト
    public List<Transform> myBulidingsTransform { get; private set; } = new List<Transform>();
    public List<Transform> enemyBulidingsTransform { get; private set; } = new List<Transform>();
    public void EnterList(Transform enterTransform, Component compornent)
    {
        //建物が建物のインターフェイスを持っていれば登録
        if (compornent is IBuilding)
        {
            myBulidingsTransform.Add(enterTransform);
            enemyBulidingsTransform.Add(enterTransform);
        }
    }

    /// <summary>
    /// 建物を管理リストに登録する
    /// </summary>
    /// <param name="enterTransform">登録する建物</param>
    /// <param name="compornent">建物が持つコンポーネント</param>
    public void EnterList(Transform enterTransform, Component compornent, int id)
    {
        //建物が建物のインターフェイスを持っていれば登録
        if (compornent is IBuilding)
        {
            if (IsSameId(id, PhotonNetwork.player.ID)) enemyBulidingsTransform.Add(enterTransform);
            else myBulidingsTransform.Add(enterTransform);
        }
    }

    public void ReleaseList(Transform releaseTransform)
    {
        myBulidingsTransform.Remove(releaseTransform);
        Debug.Log(releaseTransform + ":削除");
    }
}
