using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Triggers;
using System.Linq;


/// <summary>
/// 自分がいろいろな所で使うメソッドをまとめる
/// </summary>
public static class StaticUse
{
    /// <summary>
    /// 指定シーンが読み込まれていないときに指定シーンを追加読み込みする
    /// </summary>
    /// <param name="sceneName">マスターシーンの名前</param>
    public static void SceneLoad(string sceneName)
    {
        var master = SceneManager.GetSceneByName(sceneName);
        if (master.isLoaded) return;
        else if (!master.isLoaded) SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// 通信で受け取ったIDと自分のIDを比較し結果を返す
    /// </summary>
    /// <param name="receivedId">通信で受け取ったID</param>
    /// <param name="myId">自分のID</param>
    /// <returns></returns>
    public static bool IsSameId(int receivedId,int myId)
    {
        bool result = false;
        if (receivedId == myId) result = true;
        else result = false;
        return result;
    }

    /// <summary>
    /// 距離を計算し最も近い建物のTransformを返す
    /// </summary>
    /// <param name="myPos">自分の場所</param>
    /// <param name="transformList">建物のTransformのリスト</param>
    /// <returns></returns>
    public static Transform CalcDistance(Vector3 myPos, List<Transform> transformList)
    {
        List<float> distances = new List<float>();
        foreach (var b in transformList)
        {
            var d = (myPos - b.transform.position).sqrMagnitude;
            Debug.Log(d + "," + b);
            distances.Add(d);
        }
        var maxIdx = distances
            .Select((val, idx) => new { V = val, I = idx })
            .Aggregate((max, working) => (max.V < working.V) ? max : working).I;
        return transformList[maxIdx];
    }
}
