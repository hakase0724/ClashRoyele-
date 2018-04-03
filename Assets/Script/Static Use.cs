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
    /// 指定シーンが読み込まれていないときに指定シーンをロードする
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
    public static Transform CalcDistance(Transform myPos, List<Transform> transformList,bool isAhead)
    {
        if (transformList.Count <= 0) return myPos;
        //計算した距離を入れるリスト
        List<float> distances = new List<float>();
        List<Transform> target = new List<Transform>();
        //距離を計算しリストに格納
        foreach (var b in transformList)
        {
            float distance;
            distance = (myPos.position - b.transform.position).sqrMagnitude;
            target.Add(b);
            distances.Add(distance);
        }
        //最小距離のインデックスを検索する https://qiita.com/Go-zen-chu/items/b546d01fd14ca818d00d ←ここからとったものを改造
        float ignoreDistance = 2f;
        var minIdx = distances
            .Select((val, idx) => new { V = val, I = idx })
            .Where((min,working) => min.V > ignoreDistance * ignoreDistance)
            .Aggregate((min, working) => (min.V < working.V) ? min : working).I;
        return transformList[minIdx];
    }

    /// <summary>
    /// 距離を計算し最も近い建物のTransformを返す
    /// </summary>
    /// <param name="me">自分の場所</param>
    /// <param name="targetList">建物のTransformのリスト</param>
    /// <returns></returns>
    public static GameObject CalcDistance(GameObject me, List<GameObject> targetList, bool isAhead)
    {
        if (targetList.Count <= 0) return me;
        //計算した距離を入れるリスト
        List<float> distances = new List<float>();
        //距離を計算しリストに格納
        foreach (var b in targetList)
        {
            float distance;
            distance = (me.transform.position - b.transform.position).sqrMagnitude;
            distances.Add(distance);
        }
        //最小距離のインデックスを検索する https://qiita.com/Go-zen-chu/items/b546d01fd14ca818d00d ←ここからとったものを改造
        float ignoreDistance = 2f;
        var minIdx = distances
            .Select((val, idx) => new { V = val, I = idx })
            .Where((min, working) => min.V > ignoreDistance * ignoreDistance)
            .Aggregate((min, working) => (min.V < working.V) ? min : working).I;
        return targetList[minIdx];
    }

    public static void AllDispose(List<IDisposable> disposeList)
    {
        foreach(var d in disposeList)
        {
            d.Dispose();
        }
    }

    public static float CalcDistance(Vector3 myPos, Vector3 targetPos)
    {
        return (myPos - targetPos).sqrMagnitude;
    }
}
