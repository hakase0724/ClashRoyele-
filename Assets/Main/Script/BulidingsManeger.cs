﻿using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using static StaticUse;

/// <summary>
/// 建物管理
/// </summary>
public class BulidingsManeger : MonoBehaviour
{
    //管理している建物のリスト
    public List<Transform> myBulidingsTransform { get; private set; } = new List<Transform>();
    public List<Transform> enemyBulidingsTransform { get; private set; } = new List<Transform>();
    public List<Transform> bridgesTransform { get; private set; } = new List<Transform>();
    //自分が対象とする建物のリスト
    public List<GameObject> myBulidings { get; private set; } = new List<GameObject>();
    //相手が対象とする建物のリスト
    public List<GameObject> enemyBulidings { get; private set; } = new List<GameObject>();
    //橋のリスト
    public List<GameObject> bridges { get; private set; } = new List<GameObject>();

    private void Start()
    {
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.E))
            .Subscribe(_ =>
            {
                foreach (var e in enemyBulidingsTransform)
                {
                    Debug.Log("敵:" + e + ",座標:" + e.position);
                }
            });
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.M))
            .Subscribe(_ =>
            {
                foreach (var e in myBulidingsTransform)
                {
                    Debug.Log("味方:" + e + ",座標:" + e.position);
                }
            });
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.B))
            .Subscribe(_ =>
            {
                foreach (var e in bridgesTransform)
                {
                    Debug.Log("味方:" + e + ",座標:" + e.position);
                }
            });
    }
    /// <summary>
    /// 建物を管理リストに登録する
    /// </summary>
    /// <param name="enter">登録する建物</param>
    /// <param name="compornent">建物が持つコンポーネント</param>
    public void EnterList(GameObject enter, Component compornent)
    {
        //建物が建物のインターフェイスを持っていれば登録
        if (compornent is IBuilding)
        {
            bridges.Add(enter);
        }
    }

    /// <summary>
    /// 建物を管理リストに登録する
    /// </summary>
    /// <param name="enter">登録する建物</param>
    /// <param name="compornent">建物が持つコンポーネント</param>
    public void EnterList(GameObject enter, Component compornent, int identificationNumber)
    {
        //建物が建物のインターフェイスを持っていれば登録
        if (compornent is IBuilding)
        {
            if (identificationNumber == 0) enemyBulidings.Add(enter);
            else myBulidings.Add(enter);
        }
    }

    public void ReleaseList(GameObject release)
    {
        if (myBulidingsTransform.Select(x => x).Any(x => x == release)) myBulidings.Remove(release);
        if (enemyBulidingsTransform.Select(x => x).Any(x => x == release)) enemyBulidings.Remove(release);
    }
    ///// <summary>
    ///// 建物を管理リストに登録する
    ///// </summary>
    ///// <param name="enterTransform">登録する建物</param>
    ///// <param name="compornent">建物が持つコンポーネント</param>
    //public void EnterList(Transform enterTransform, Component compornent)
    //{
    //    //建物が建物のインターフェイスを持っていれば登録
    //    if (compornent is IBuilding)
    //    {
    //        bridgesTransform.Add(enterTransform);
    //    }
    //}

    ///// <summary>
    ///// 建物を管理リストに登録する
    ///// </summary>
    ///// <param name="enterTransform">登録する建物</param>
    ///// <param name="compornent">建物が持つコンポーネント</param>
    //public void EnterList(Transform enterTransform, Component compornent, int identificationNumber)
    //{
    //    //建物が建物のインターフェイスを持っていれば登録
    //    if (compornent is IBuilding)
    //    {
    //        if (identificationNumber == 0) enemyBulidingsTransform.Add(enterTransform);
    //        else myBulidingsTransform.Add(enterTransform);
    //    }
    //}

    //public void ReleaseList(Transform releaseTransform)
    //{
    //    if (myBulidingsTransform.Select(x => x).Any(x => x == releaseTransform)) myBulidingsTransform.Remove(releaseTransform);
    //    if (enemyBulidingsTransform.Select(x => x).Any(x => x == releaseTransform)) enemyBulidingsTransform.Remove(releaseTransform);
    //}
}
