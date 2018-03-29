﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 建物管理
/// </summary>
public class BulidingsManeger : MonoBehaviour
{
    //管理している建物のリスト
    public List<Transform> bulidingsTransform { get; private set; } = new List<Transform>();

    /// <summary>
    /// 建物を管理リストに登録する
    /// </summary>
    /// <param name="enterTransform">登録する建物</param>
    /// <param name="compornent">建物が持つコンポーネント</param>
    public void EnterList(Transform enterTransform,Component compornent)
    {
        //建物が建物のインターフェイスを持っていれば登録
        if (compornent is IBuilding)
        {
            bulidingsTransform.Add(enterTransform);
        }
    }
}